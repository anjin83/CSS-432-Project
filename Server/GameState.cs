using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameState
{

   private Player[] currentPlayers;
   private Player dealer;
   public Deck theDeck;
   public Card[] table_Cards;
   public int pot;
    public int currentBet;
   public int blind;
    private int maxCurrentBet;
   private int[] playerHandStrength;
   private double timer;
    public int tableCounter;

   public GameState(Player[] players) 
   {
        theDeck = new Deck();
      currentPlayers = players;
        tableCounter = 0;
        table_Cards = new Card[5];
   }

   public void updatePlayers(Player[] players)
   {
      currentPlayers = players;
   }
}





