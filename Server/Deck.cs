using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Deck
{
   private Card[] deck;
   Random dealer = new Random();

   //initialize the deck with 52 cards
   public Deck() 
   {
      deck = new Card[52];
      for (int i = 0; i < 4; i++)
      {
         for (int j = 0; j < 13; j++)
         {
            deck[(i * 13) + j] = new Card(i, j);
         }
      }
   }

   //randomly selects a card to deal, returns the ace of carrots if theres a problem
   public Card Deal()
   {
      bool dealt = false;
      int cardDealt = dealer.Next(52);
      while (!dealt)
      {
         if (deck[cardDealt].dealt != true)
         {
            deck[cardDealt].dealt = true;
            return deck[cardDealt];
         }
         cardDealt = dealer.Next(52);
      }

      return new Card(-1, -1); ;  //problem with dealing
   }

   //sets all cards in the deck to undealt
   public void Shuffle()
   {
      for (int i = 0; i < 52; i++)
      {
         if (deck[i].dealt == true)
            deck[i].dealt = false;
      }
        deck = new Card[5];
   }
}

