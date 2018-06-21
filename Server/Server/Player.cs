using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

public class Player
{
   public Socket playerSocket;
   String playerName;
   int chips;
   Card[] hand;
   int currentBet;
   private int playerID; 
   int currentLobby;
   
   public Player(String name, int ID, Socket pSocket) 
   {
      playerSocket = pSocket;
      playerName = name;
      playerID = ID;
      chips = 5000;
   }

   public int getPlayerID()
   {
      return playerID;
   }

    public string getName()
    {
        return playerName;
    }
}

