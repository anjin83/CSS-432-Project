using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

// State object for reading client data asynchronously
public class StateObject
{
   public Socket workSocket = null;

   public const int BufferSize = 1500;

   public byte[] buffer = new byte[BufferSize];

   public StringBuilder sb = new StringBuilder();

    public bool stopServer = false;
}

public class Server
{
    //private List<Socket> clients;
    private List<Socket> clientSockets;		//List of client socket information
    private List<Lobby> lobbyList;			//List of lobbies hosting games
    private List<Player> tempPlayerList;    //store players who are not in a lobby yet
	
    int nextPlayerID = 0;           //Player ID given to connecting clients. Updated once assigned to a client.

	//Thread signaling event
    public ManualResetEvent readDone = new ManualResetEvent(false);
    public ManualResetEvent allDone = new ManualResetEvent(false);
    private ManualResetEvent receiveDone = new ManualResetEvent(false);
    public ManualResetEvent gameStarted = new ManualResetEvent(false);
    
    public Server() {
        lobbyList = new List<Lobby>();
        tempPlayerList = new List<Player>();
        clientSockets = new List<Socket>();
		
		//Create eight lobbies with IDs 1-9
        for(int i = 1; i < 9; i++)
        {
            lobbyList.Add(new Lobby(i.ToString()));
        }         
    }
	
	//Handle incoming client connections.
    public void StartListening()
    {               
        //Establish the endpoint for the socket
        //retrieve the ip address of the server
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 12000);
      
        //Create TCP/IP socket
        Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

		//Write server IP address to screen for connecting clients to use
            Console.WriteLine(ipAddress.ToString());
		
        //Listen for incoming connections
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);
            while (true)
            {            
            allDone.Reset();				
			
            //Start an asychronous socket to listen to connections
            Console.WriteLine("Waiting for connection...");
			
			//Create a thread that handles client connection.
            listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

            // Wait until a connection is made before continuing
            allDone.WaitOne();         
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

    }

    public void AcceptCallback(IAsyncResult ar)
    {
        //Signal the main thread to continue
        allDone.Set();

        // Get the socket that handles to client request
        Socket listener = (Socket) ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        //
        clientSockets.Add(handler);
       
        //Create the state object to store communication information
        StateObject state = new StateObject();
        state.workSocket = handler;
		
		//Keep connection to client open
        while (true)
        {
            readDone.Reset();
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            readDone.WaitOne();            
        }
    }

    //Perform reads from clients
    public void ReadCallback(IAsyncResult ar)
    {
        String content = String.Empty;

        // Retrieve the state object and the handler socket
        // from the asynchronous state object
        StateObject state = (StateObject) ar.AsyncState;
        Socket handler = state.workSocket;

        // Read data from the client socket
        int bytesRead = handler.EndReceive(ar);

        if (bytesRead > 0)
        {
            // There might be more data, so store the data received so far.
            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

            // Check for end-of-file tag. If its not there, read more data
            content = state.sb.ToString();
            if (content.IndexOf("<EOF>") > -1)
            {
               // All the data has been read from the client. Display it on the console.
               Console.WriteLine("Read {0} bytes from the socket. \n Data: {1}",
                   content.Length,
                   content);

               //Parse message
               ParseMessage(content, handler); //data sent back to client here
               readDone.Set();
               state.sb.Clear();
            }
        }
        else
        {
            //Not all data received, get more
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }
    }

    private void ParseMessage(String clientMessage, Socket handler)
    {
        String command = "";
        char iterator = clientMessage[0];   //iterator for parsing string
        int counter = 0;
        while (counter != clientMessage.Length)
        {
            command += iterator;
            counter++;

            if (counter >= clientMessage.Length) //Check if iterator is out of bound
            break;

            iterator = clientMessage[counter];    //Check if delimiter reached
            if (iterator == '\n')   //delimiter reached, parse command
            {
            counter = ParseCommand(command, counter, clientMessage, handler);
            command = "";  //reset command
            if(counter+1 < clientMessage.Length) counter++;  //start reading next command
            iterator = clientMessage[counter];
            }
            if (command.IndexOf("<EOF>") > -1)
            break;
        }  
    }

	
	//Parses command read from client message and processes that command
    private int ParseCommand(String command, int counter, string clientMess, Socket handler)
    {
        if (command == "JoinLobby")
        {
            counter = JoinLobby(counter, clientMess, handler);
        }

        else if (command == "CreatePlayer")
        {
            counter = CreatePlayer(counter, clientMess, handler);
        }
        else if (command == "GetLobbyInfo")
        {
            sendLobbyInfo(handler);
        }
        else if (command == "PopulateLobby")
        {
            counter = PopulateLobby(counter, clientMess, handler);
        }
        else if (command == "INITGAME")
        {
            counter = InitGame(counter, clientMess, handler);
        }
        else if(command == "START")
        {
            counter = StartGame(counter, clientMess, handler);
        }
        else if (command == "WAIT")
        {
            Wait();
        }

        return counter;
    }
	
	//Used for synchronization purposes.
    private void Wait()
    {
        gameStarted.WaitOne();
    }

	//Send a message to a client. Actual sending is handled inside SendCallback thread.
    private void Send(Socket handler, String data)
    {
        Console.WriteLine("Data being sent: " + data);
        // Convert the string data to byte data using ASCII encoding
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device
        handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        if (data.IndexOf("WAIT") > -1)
            gameStarted.WaitOne();
    }
	
	//Thread that handles sending data to client
    private void SendCallback(IAsyncResult ar)
    {
        try 
        {
            //Retrieve the socket from the state object
            Socket handler = (Socket) ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to client.", bytesSent);
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
	
	//Initialize a game with the lobbies list of players
    private int InitGame(int counter, string clientMess, Socket handler)
    {        
        counter++; 				//Correct the pointer for the message parser
        string lobbyNum = "";	//Extract the lobby number to initialize from the client message
        lobbyNum = spliceString(ref clientMess, ref counter);

        Lobby tempLobby = null;
		
		//Get each player in the lobbies information
        foreach (Lobby curLobby in lobbyList)
        {
            if (lobbyNum == curLobby.GetName())
            {
                tempLobby = curLobby;
                break;
            }
        }

        //Close the lobby in the lobby list
        tempLobby.lobbyFull = true;
        int numPlayers = tempLobby.numPlayers();
		
		//Send a message to each client to initialize their game screen.
        for(int i = 0; i < numPlayers; i++)
        {
            string message = "INITGAME\n";
            int j = i + 1;
            for (int z = 0; z < numPlayers - 1; z++, j++)		//Get information from each player in lobby and send that information to all players to construct their game screen
            {
                message += tempLobby.playerList[j % numPlayers].getPlayerID().ToString();
                message += '\n';
                message += tempLobby.playerList[j % numPlayers].getName();
                message += '\n';
            }
            message += "<EOF>";
            Send(tempLobby.playerList[i].playerSocket, message);
        }
        
        Console.WriteLine("Starting Game");
        tempLobby.startGame();
        gameStarted.WaitOne();
        return counter;
    }
		
	//Get the information for the players currently in the lobby and send 
	//that information to each client in the lobby to populate their lobby list.
    public int PopulateLobby(int counter, string clientMess, Socket handler)
    {
       counter++; 
       string lobbyNum = "";
       lobbyNum = spliceString(ref clientMess, ref counter);

       Lobby tempLobby = null;

       foreach (Lobby curLobby in lobbyList)
       {
          if (lobbyNum == curLobby.GetName())
          {
             tempLobby = curLobby;
             break;
          }
       }

       Send(handler, "SUCCESS\n" + tempLobby.listPlayers());
       return counter;
    }
	
	//Move a player into a lobby, update that lobby, and send the updated
	//lobby information to all players in the lobby.
    public int JoinLobby(int counter, string clientMess, Socket handler)
    {       
		//Extract lobby number from client message
        counter++; 
        string lobbyNum = "";
        lobbyNum = spliceString(ref clientMess, ref counter);      
		
		//Extract player ID from client message
        counter++;
        string playerID = "";
        playerID = spliceString(ref clientMess, ref counter);                      

        Lobby tempLobby = null;
		
		//Find the lobby that the player wants to enter
        foreach (Lobby curLobby in lobbyList)
        {
            if (lobbyNum == curLobby.GetName())
            {
                tempLobby = curLobby;
                break;
            }
        }
		
		//lobby is full, send refreshed lobby information
        if(tempLobby.numPlayers() == 8) 
        {
            string lobbyInfo = "";
            foreach (Lobby curLobby in lobbyList)
            {
                lobbyInfo += curLobby.ToString();
            }
            lobbyInfo += "<EOF>";
            Send(handler, "FULL\n" + lobbyInfo);
            return counter;
        }

        Player tempPlayer = null;
		
		//Remove the player from the temporary player list and into a lobby.
        foreach (Player curPlayer in tempPlayerList)
        {
            if (curPlayer.getPlayerID().ToString() == playerID)
            {
                tempPlayer = curPlayer;
                tempPlayerList.Remove(curPlayer);
                break;
            }
        }

        tempLobby.AddPlayer(tempPlayer);

        Send(handler, "SUCCESS\n" + tempLobby.listPlayers());
        return counter;
    }
	
	//Create a player object with the information in the client message and 
	//add that player to the temporary player list.
    public int CreatePlayer(int counter, string clientMess, Socket handler)
    {
        //extract player name from message        
        string name = "";
        counter++;
        char iterator = clientMess[counter];   //currently pointing to first delimiter
        while (iterator != '\n')
        {
            name += iterator;
            counter++;
            iterator = clientMess[counter];
            if (counter > clientMess.Length) //did not read a \n and out of bounds
            {
                break;
            }
        }
        Player tempPlayer = new Player(name, nextPlayerID, handler);
        nextPlayerID++;
        tempPlayerList.Add(tempPlayer);
        String pid = tempPlayer.getPlayerID().ToString();
        pid += "\n<EOF>";
        Send(handler, pid); // Send player ID back to client
        return counter;
    }
	
	//Send client information on the status of each lobby
    public void sendLobbyInfo(Socket handler)
    {
       string lobbyInfo = "";
       foreach (Lobby curLobby in lobbyList)
       {
          lobbyInfo += curLobby.ToString();
       }
       lobbyInfo += "<EOF>";
       Send(handler, lobbyInfo);
    }
	
	//Extracts relevant information from client messages
    private static string spliceString(ref string lobbyMessage, ref int counter)
    {
       
       String token = "";
       char iterator = lobbyMessage[counter];   //iterator for parsing string
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
             return token;
          }
       }
       return null;  //Null is returned on error
    }

    public static int Main(String[] args)
    {
        Server gameServer = new Server();
        gameServer.StartListening();
        return 0;
    }
}
