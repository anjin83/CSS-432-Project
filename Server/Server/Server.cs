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
}

public class Server
{
    //private List<Socket> clients;
    private List<Socket> clientSockets;
    private List<Lobby> lobbyList;
    private List<Player> tempPlayerList;    //store players who are not in a lobby yet
    int nextPlayerID = 0;           //needs to be syncronized to prevent 2 players with same ID

    public ManualResetEvent readDone = new ManualResetEvent(false);
    public ManualResetEvent allDone = new ManualResetEvent(false);

    //Change to 8 element array for lobbies
    public Server() {
        lobbyList = new List<Lobby>();
        tempPlayerList = new List<Player>();
        clientSockets = new List<Socket>();
        for(int i = 1; i < 9; i++)
        {
            lobbyList.Add(new Lobby(i.ToString()));
        }         
    }

    public void StartListening()
    {
        //Thread signal
       
        //buffer for sending message to clients
        byte[] sendBuffer = new Byte[1500];

        //buffer for receiving messages from clients
        byte[] receiveBuffer = new Byte[1500];

        //Establish the endpoint for the socket
        //retrieve the ip address of the server
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 12000);
      
        //Create TCP/IP socket
        Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        //bind the socket to the local endpoint and listen for incoming connections
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);
            while (true)
            {
            // Set the event to a non-signaled state 
            allDone.Reset();

            Console.WriteLine(ipAddress.ToString());
            //Start an asychronous socket to listen to connections
            Console.WriteLine("Waiting for connection...");
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

        // Create the state object
        StateObject state = new StateObject();
        state.workSocket = handler;
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
        return counter;
    }

    private void Send(Socket handler, String data)
    {
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
            Socket handler = (Socket) ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to client.", bytesSent);

        //handler.Shutdown(SocketShutdown.Both);
        //handler.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    //CLEAN THIS SHIT UP
    public int JoinLobby(int counter, string clientMess, Socket handler)
    {
        //"JoinLobby\n1\npid\n<EOF>"
        string lobbyNum = "";
        counter++;
        char iterator = clientMess[counter];   //currently pointing to first delimiter
        while (iterator != '\n')
        {
            lobbyNum += iterator;
            counter++;
            iterator = clientMess[counter];
            if (counter > clientMess.Length) //did not read a \n and out of bounds, big problems here
            {
                break;
            }
        }
        counter++;
        iterator = clientMess[counter];
        string playerID = "";
        while (iterator != '\n')
        {
            playerID += iterator;
            counter++;
            iterator = clientMess[counter];
            if (counter > clientMess.Length) //did not read a \n and out of bounds, big problems here
            {
                break;
            }
        }
        
        /*
         * lock(x)
         * {
         *      do stuff
         * }
         * 
         */

        Lobby tempLobby = null;

        foreach (Lobby curLobby in lobbyList)
        {
            if (lobbyNum == curLobby.GetName())
            {
                tempLobby = curLobby;
                break;
            }
        }

        if(tempLobby.numPlayers() == 8) //lobby if full, send refreshed lobby information
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
            if (counter > clientMess.Length) //did not read a \n and out of bounds, big problems here
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
    


    public static int Main(String[] args)
    {
        Server gameServer = new Server();
        gameServer.StartListening();
        return 0;
    }
}
