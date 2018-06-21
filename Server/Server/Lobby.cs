using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Lobby
{
   private String lobbyName;
   private Player[] playerList;
   private Int16[] optionList;
   private Player host;
   private GameState curGame;
   private bool lobbyFull;


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
      host = hostPlayer;
      playerList[0] = hostPlayer;
      lobbyName = name;
      lobbyFull = false;
      curGame = new GameState(playerList);
    }

   public void startGame()
   {
     // curGame();
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
        while(playerList[iter] != thePlayer && iter < playerList.Length)
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

    public int numPlayers()
    {
        int numPlayers = 0;
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i] != null) numPlayers++;
        }
        return numPlayers;
    }

    public string listPlayers()
    {
        string players = "";
        for(int i = 0; i < playerList.Length; i++)
        {
            if(playerList[i] != null)
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
      if(lobbyFull) lobbyInfo += "CLOSED\n";
      else lobbyInfo += "OPEN\n";
      lobbyInfo += numPlayers().ToString();
      lobbyInfo += "\n";
      return lobbyInfo;
   }
}

