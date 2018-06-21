using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameState
{

   private Player[] currentPlayers;
   private Player dealer;
   private Player bigBlind;
   private Player smallBlind;
   private Deck theDeck;
   private Card[] table_Cards;
   private int pot;
   private int minBet;
   private int[] playerHandStrength;
   private double timer;

   public GameState(Player[] players) 
   {
      currentPlayers = players;
   }

   public void updatePlayers(Player[] players)
   {
      currentPlayers = players;
   }
}





