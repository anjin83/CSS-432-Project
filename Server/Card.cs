using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Card
{
   private int suite;
   private int value;
   public bool dealt;
  
   public Card(int cardSuite, int cardValue) 
   {
      suite = cardSuite;
      value = cardValue;
      dealt = false;
   }
    
    public void SetValue(int newVal)
    {
        value = newVal;
    }

   public int ReturnSuite()
   {
      return suite;
   }

   public int ReturnValue()
   {
      return value;
   }
}

