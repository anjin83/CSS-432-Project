using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

public class Lobby
{
    private String lobbyName;
    public Player[] playerList;
    private Int16[] optionList;
    private Player dealer;
    private GameState curGame;
    public bool lobbyFull;
    struct pidStrength
    {
        public Player player;
        public int handStrength;
    }

    private string clientResponse;

    // ManualResetEvent instances signal completion
    private ManualResetEvent thisConnectDone = new ManualResetEvent(false);
    private ManualResetEvent thisSendDone = new ManualResetEvent(false);
    private ManualResetEvent thisReceiveDone = new ManualResetEvent(false);

    public Lobby(String name)
    {
        playerList = new Player[8];
        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i] = null;
        }
        optionList = new Int16[3]; //Stores options for game
        lobbyName = name;
        lobbyFull = false;
        curGame = new GameState(playerList);
    }

    public Lobby(Player hostPlayer, String name)
    {
        playerList = new Player[8];
        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i] = null;
        }
        optionList = new Int16[3]; //Stores options for game
                                   //host = hostPlayer;
        playerList[0] = hostPlayer;
        lobbyName = name;
        lobbyFull = false;
        curGame = new GameState(playerList);
    }

    public string GetName()
    {
        return lobbyName;
    }

    public bool AddPlayer(Player newPlayer)
    {
        int iter = 0;
        while (iter < 8 && playerList[iter] != null) //find a free slot if available
            iter++;
        if (iter < 8)   //free spot found, add player to slot
        {
            playerList[iter] = newPlayer;
            curGame.updatePlayers(playerList);  //update gamestate player list
            return true;
        }
        else //lobby is full
        {
            lobbyFull = true;
            return false;
        }

    }

    public bool LeaveLobby(Player thePlayer)
    {
        int iter = 0;
        while (playerList[iter] != thePlayer && iter < playerList.Length)
        {
            iter++;
        }
        if (playerList[iter] == thePlayer)  //found the player, remove from list
        {
            playerList[iter] = null;
            if (lobbyFull) lobbyFull = false;
            return true;
        }
        else                                //player not found, return false
            return false;
    }

    public int NumPlayers()
    {
        int numPlayers = 0;
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i] != null)
                numPlayers++;
        }
        return numPlayers;
    }

    public int numPlayers()
    {
        int numPlayers = 0;
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i] != null) numPlayers++;
        }
        return numPlayers;
    }
	
	//Constructs a message with current player information for the server to send to clients
    public string listPlayers()
    {
        string players = "";
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i] != null)
            {
                players += playerList[i].getName();
                players += "\n";
            }
        }
        players += "<EOF>";
        return players;
    }

    public override string ToString()
    {
        //Return name, status, number of players
        string lobbyInfo = "";
        lobbyInfo += lobbyName + '\n';
        if (lobbyFull) lobbyInfo += "CLOSED\n";
        else lobbyInfo += "OPEN\n";
        lobbyInfo += numPlayers().ToString();
        lobbyInfo += "\n";
        return lobbyInfo;
    }
	
	//Logic for handling one round of poker
    public void startGame()
    {
        int oksReceived = 0;
        //to do, check if connection is still open
        Player[] currentPlayers = GetCurrentPlayers(playerList);

        int curDealer = 0;
        while (currentPlayers.Length > 1) //game logik
        {
            curGame.blind = 25;
            int curDealerIndex = 0;
            currentPlayers = GetCurrentPlayers(playerList);
            while (playerList[curDealer] == null)
                curDealer++;
            int dealerPid = playerList[curDealer].getPlayerID();

            while (dealerPid != currentPlayers[curDealerIndex].getPlayerID())
                curDealerIndex++;

            SendSetDealer(dealerPid);   //set the dealer
            SendUpdate("RAISE", currentPlayers[((curDealerIndex + 1) % currentPlayers.Length)].getPlayerID(), curGame.blind.ToString());  //get the blinds
            SendUpdate("RAISE", currentPlayers[((curDealerIndex + 1) % currentPlayers.Length)].getPlayerID(), (curGame.blind * 2).ToString());

            curGame.currentBet = (curGame.blind * 2);
            DealPlayerCards(currentPlayers);

            //Get Actions
            HandleActions(ref currentPlayers, ref curDealer);

            //cleanup
            SendCleanup();

            //Deal 3 to table
            SendDealTable(currentPlayers);
            SendDealTable(currentPlayers);
            SendDealTable(currentPlayers);

            //Get Actions
            HandleActions(ref currentPlayers, ref curDealer);

            if (currentPlayers.Length <= 1)
            {
                //Showdown
                SendWinner(currentPlayers);

                //reset
                SendReset();

                //next round
                curDealer++;
                continue;
            }

            //cleanup
            SendCleanup();

            //Deal 1 to table
            SendDealTable(currentPlayers);

            //Get Actions
            HandleActions(ref currentPlayers, ref curDealer);

            if (currentPlayers.Length <= 1)
            {
                //Showdown
                SendWinner(currentPlayers);

                //reset
                SendReset();

                //next round
                curDealer++;
                continue;
            }

            //cleanup
            SendCleanup();

            //Deal 1 to table
            SendDealTable(currentPlayers);

            //Get actions
            HandleActions(ref currentPlayers, ref curDealer);

            if (currentPlayers.Length <= 1)
            {
                //Showdown
                SendWinner(currentPlayers);

                //reset
                SendReset();

                //next round
                curDealer++;
                continue;
            }

            //Showdown
            SendWinner(currentPlayers);

            //reset
            SendReset();

            //next round
            curDealer++;

        }
    }

    //Create an array of players that have not folded or quit the current round
    Player[] GetCurrentPlayers(Player[] allPlayers)
    {
        int counter = 0;
        for (int i = 0; i < allPlayers.Length; i++) //add check for connection later
        {
            if (allPlayers[i] != null && allPlayers[i].GetChips() > 0)
                counter++;
        }
        Player[] currentPlayers = new Player[counter];
        counter = 0;

        for (int i = 0; i < allPlayers.Length; i++) //add check for connection later
        {
            if (allPlayers[i] != null && allPlayers[i].GetChips() > 0)
                currentPlayers[counter++] = allPlayers[i];
        }
        return currentPlayers;
    }

	//Sends dealer information to client
    private void SendSetDealer(int pid)
    {
        string message = "SETDEALER\n" + pid.ToString() + "\n<EOF>";
        SendAll(message);
    }

	//Sends a message to the client about a card being delt so the client can update its game state
    private void SendCard(Socket playerSock, Card theCard, int PID)
    {
        string message = "DEAL\n" + PID.ToString() + "\n" + theCard.ReturnSuite().ToString() + "\n" + theCard.ReturnValue().ToString() + "\n<EOF>";
        Send(playerSock, message);
        thisSendDone.WaitOne();
    }

	//Request an action from each player still currently playing in the round
    private void HandleActions(ref Player[] currentPlayers, ref int curDealer)
    {
        bool responseFromEndpoint = false;
        Player currentPlayerResponse = currentPlayers[(curDealer + 3) % currentPlayers.Length];
        Player endpoint = currentPlayers[(curDealer + 2) % currentPlayers.Length];
        while (currentPlayers.Length > 1 && !responseFromEndpoint)
        {
            //Get actions
            RequestAction(currentPlayerResponse.playerSocket, currentPlayers, currentPlayerResponse);
        }
    }
	
	//Sends message to client that the server is ready to accept a command from that client
    private void RequestAction(Socket playerSocket, Player[] curPlayers, Player curPlayer)
    {
        string message = "REQACT\n<EOF>";
        //send request to player
        Send(playerSocket, message);
        thisSendDone.WaitOne();

        message = "";
        //start timer
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        while (true)
        {
            if (playerSocket.Available != 0)    //message arrived from client
            {
                Receive(playerSocket);
                thisReceiveDone.WaitOne();
                message = "UPDATE\n" + clientResponse;
                ProcessMessage(curPlayers, playerSocket, curPlayer, clientResponse);
                clientResponse = "";
                break;
            }
            else if (stopWatch.ElapsedMilliseconds >= 10000)
            {
                if (curGame.currentBet == 0)
                {
                    message = "UPDATE\nCHECK\n" + curPlayer.getPlayerID().ToString() + "\n<EOF>";
                    break;
                }
                else
                {
                    message = "UPDATE\nFOLD\n" + curPlayer.getPlayerID().ToString() + "\n<EOF>";
                    int index = GetPlayerIndex(curPlayers, curPlayer);
                    RemoveIndices(curPlayers, index);
                    break;

                }
            }
        }
        SendAll(message);
    }

	//Processes player command that was parsed out of the client message
    private bool ProcessMessage(Player[] curPlayers, Socket playerSocket, Player thePlayer, String clientMessage)
    {
        string token = spliceString(ref clientMessage);
        if (token == "CHECK")
        {
            return false;
        }
        if (token == "FOLD")
        {
            int index = GetPlayerIndex(curPlayers, thePlayer);
            RemoveIndices(curPlayers, index);
            return false;
        }
        if (token == "CALL")
        {
            curGame.pot -= thePlayer.currentBet;
            thePlayer.currentBet = curGame.currentBet;
            thePlayer.SetChips(thePlayer.GetChips() - thePlayer.currentBet);
            curGame.pot += thePlayer.currentBet;
            return false;
        }
        if (token == "RAISE")
        {
            curGame.pot -= thePlayer.currentBet;
            spliceString(ref clientMessage);
            token = spliceString(ref clientMessage);
            int bet = int.Parse(token);
            thePlayer.currentBet = bet;
            curGame.currentBet = bet;
            thePlayer.SetChips(thePlayer.GetChips() - thePlayer.currentBet);
            curGame.pot += thePlayer.currentBet;
            return true;

        }
        return false;

    }
	
	//Sends message to client about the next table card dealt to the table
    private void SendDealTable(Player[] curPlayers)
    {
        curGame.table_Cards[curGame.tableCounter] = curGame.theDeck.Deal();

        string message = "DEAL\n-1\n" + curGame.table_Cards[curGame.tableCounter].ReturnSuite().ToString() + "\n" + curGame.table_Cards[curGame.tableCounter].ReturnSuite().ToString() + "\n<EOF>";
        SendAll(message);
    }
	
	//Sends message to client about the two cards that have been dealt to them
    private void DealPlayerCards(Player[] curPlayers)
    {
        for (int i = 0; i < curPlayers.Length; i++)
        {
            curPlayers[i].hand[0] = curGame.theDeck.Deal();
            curPlayers[i].hand[1] = curGame.theDeck.Deal();
            SendCard(curPlayers[i].playerSocket, curPlayers[i].hand[0], curPlayers[i].getPlayerID());
            SendCard(curPlayers[i].playerSocket, curPlayers[i].hand[1], curPlayers[i].getPlayerID());
        }
    }
	
	//Send message to client to clean up the game state
    private void SendCleanup()
    {
        string message = "CLEANUP\n<EOF>";
        SendAll(message);
    }

    private int GetPlayerIndex(Player[] curPlayers, Player thePlayer)
    {

        for (int i = 0; i < curPlayers.Length; i++)
        {
            if (curPlayers[i].getPlayerID() == thePlayer.getPlayerID())
                return i;
        }
        return -1; //return ace of carrots
    }

    //send winning player and pot
    private void SendWinner(Player[] curPlayers)
    {
        pidStrength[] handStrength = new pidStrength[curPlayers.Length];
        for (int i = 0; i < handStrength.Length; i++)
        {
            handStrength[i] = new pidStrength();
            handStrength[i].player = curPlayers[i];
            handStrength[i].handStrength = Strength.makeCombination(curPlayers[i].hand[0], curPlayers[i].hand[1], curGame.table_Cards[0], curGame.table_Cards[1], curGame.table_Cards[2], curGame.table_Cards[3], curGame.table_Cards[4]);
        }   //calculate handstrength
        for (int i = 0; i < handStrength.Length - 1; i++)
        {
            for (int j = i + 1; j < handStrength.Length; j++)
            {
                if (handStrength[i].handStrength < handStrength[j].handStrength)
                {
                    int temp = handStrength[j].handStrength;
                    handStrength[j].handStrength = handStrength[i].handStrength;
                    handStrength[i].handStrength = temp;
                }
            }
        }   //sort
        //distribute winnings
        int counter = 0;
        while (curGame.pot > 0)
        {
            if ((handStrength[counter].player.totalBet * (handStrength.Length - counter)) >= curGame.pot)
            {
                handStrength[counter].player.SetChips(handStrength[counter].player.GetChips() + curGame.pot);
                curGame.pot = 0;
            }
            else
            {
                handStrength[counter].player.SetChips(handStrength[counter].player.GetChips()
                    + (handStrength[counter].player.totalBet * (handStrength.Length - counter)));
                curGame.pot -= (handStrength[counter].player.totalBet * (handStrength.Length - counter));
            }
        }

        for (int i = 0; i < playerList.Length; i++)
        {
            string message = "CHIPS\n" + playerList[i].getPlayerID().ToString() + "\n" + playerList[i].GetChips().ToString() + "\n<EOF>";
            SendAll(message);
        }
    }
	
	//Sends information to client to start a new round 
    private void SendReset()
    {
        curGame.theDeck.Shuffle();
        string message = "RESET\n<EOF>";
        SendAll(message);
        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i].EmptyHand();
        }
    }
	
	//Sends a message to client with information on updating the game state
    private void SendUpdate(string verb, int pid, string options)
    {
        string message = "UPDATE\n" + verb + "\n" + pid.ToString() + "\n" + options + "\n<EOF>";
        SendAll(message);
    }

    private void Receive(Socket client)
    {
        try
        {
            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = client;

            // Begin receiving the data from the remote device.  
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the state object and the client socket   
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.  
            int bytesRead = client.EndReceive(ar);
 
            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
 
            if (state.sb.Length > 1)
            {
                this.clientResponse = state.sb.ToString();
            }
            // Signal that all bytes have been received.  
            Console.WriteLine("Received Data: " + this.clientResponse);
            thisReceiveDone.Set();
            state.sb.Clear();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private void Send(Socket handler, String data)
    {
        Console.WriteLine("Date being sent to client : " + data);
        // Convert the string data to byte data using ASCII encoding
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device
        handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            //Retrieve the socket from the state object
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            thisSendDone.Set();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

	//Sends a message to all players with gamestate information
    private void SendAll(string serverMessage)
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i] != null)
            {
                Console.WriteLine("Date being sent to client in sendall : " + serverMessage);
                Send(playerList[i].playerSocket, serverMessage);
                this.thisSendDone.WaitOne();
            }
        }
    }

    //splices server message and returns tokens delimited by \n
    private static string spliceString(ref string lobbyMessage)
    {
        string token = "";
        char iterator = lobbyMessage[0];   //iterator for parsing string
        int counter = 0;            //used for moving iterator
        while (counter != lobbyMessage.Length)
        {
            token += iterator;
            counter++;
            if ((token.IndexOf("<EOF>") > -1)) //end of message reached
            {
                return token;
            }
            if (counter >= lobbyMessage.Length)                    //Check if iterator is out of bound
                return token;

            iterator = lobbyMessage[counter];                      //Check if delimiter reached
            if (iterator == '\n')                           //delimiter reached, parse command
            {
                lobbyMessage = lobbyMessage.Substring(counter + 1);
                return token;
            }
        }
        return null;  
    }
	
	//Removes a player from the list of players at the given index
    private Player[] RemoveIndices(Player[] IndicesArray, int RemoveAt)
    {
        Player[] newIndicesArray = new Player[IndicesArray.Length - 1];

        int i = 0;
        int j = 0;
        while (i < IndicesArray.Length)
        {
            if (i != RemoveAt)
            {
                newIndicesArray[j] = IndicesArray[i];
                j++;
            }

            i++;
        }

        return newIndicesArray;
    }
}

