using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Calculates the hand strength from all possible combinations of a players hand and returns as an integer value
//their best hand strength. The higher the value returned the better the hand.
public class Strength
{
    public static void sortCards(Card[] com)
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = i + 1; j < 5; j++)
            {
                if (com[i].ReturnValue() < com[j].ReturnValue())
                {
                    Card temp;
                    temp = com[i];
                    com[i] = com[j];
                    com[j] = temp;
                }
            }
        }
    }

    //return the highest point from 10 possible combination of 7 cards
    public static int finalPoints(int p1,int p2, int p3, int p4, int p5, int p6, int p7, int p8, int p9, int p10)
    {
        int max = 0;
        int[] temp = new int[10];
        temp[0] = p1; temp[1] = p2; temp[2] = p3; temp[3] = p4; temp[4] = p5;
        temp[5] = p6; temp[6] = p7; temp[7] = p8; temp[8] = p9; temp[9] = p10;
        for(int i=0; i<10; i++)
        {
            if(temp[i]> max)
            {
                max = temp[i];
            }
        }

        return max;

    }

    //create 10 of 5-cards combination out of 7 cards
    public static int makeCombination(Card card1, Card card2, Card card3, Card card4, Card card5, Card card6, Card card7)
    {
        checkAce(card1, card2, card3, card4, card5, card6, card7);
        Card[] com1 = new Card[5];
        com1[0] = card1; com1[1] = card2; com1[2] = card3; com1[3] = card4; com1[4] = card5;
        sortCards(com1);

        Card[] com2 = new Card[5];
        com2[0] = card1; com2[1] = card2; com2[2] = card3; com2[3] = card4; com2[4] = card6;
        sortCards(com2);

        Card[] com3 = new Card[5];
        com3[0] = card1; com3[1] = card2; com3[2] = card3; com3[3] = card4; com3[4] = card7;
        sortCards(com3);

        Card[] com4 = new Card[5];
        com4[0] = card1; com4[1] = card2; com4[2] = card3; com4[3] = card5; com4[4] = card6;
        sortCards(com4);

        Card[] com5 = new Card[5];
        com5[0] = card1; com5[1] = card2; com5[2] = card3; com5[3] = card5; com5[4] = card7;
        sortCards(com5);

        Card[] com6 = new Card[5];
        com6[0] = card1; com6[1] = card2; com6[2] = card4; com6[3] = card5; com6[4] = card6;
        sortCards(com6);

        Card[] com7 = new Card[5];
        com7[0] = card1; com7[1] = card2; com7[2] = card4; com7[3] = card5; com7[4] = card7;
        sortCards(com7);

        Card[] com8 = new Card[5];
        com8[0] = card1; com8[1] = card2; com8[2] = card4; com8[3] = card6; com8[4] = card7;
        sortCards(com8);

        Card[] com9 = new Card[5];
        com9[0] = card1; com9[1] = card2; com9[2] = card5; com9[3] = card6; com9[4] = card7;
        sortCards(com9);

        Card[] com10 = new Card[5];
        com10[0] = card1; com10[1] = card2; com10[2] = card3; com10[3] = card6; com10[4] = card7;
        sortCards(com10);

        return checkPoints(com1, com2, com3, com4, com5, com6, com7, com8, com9, com10);
    }

    //checking all 10 possible combination and return the highest points
    public static int checkPoints(Card[] com1, Card[] com2, Card[] com3, Card[] com4, Card[] com5,
        Card[] com6, Card[] com7, Card[] com8, Card[] com9, Card[] com10)
    {

        if (cardCategury(com1) >= 900 || cardCategury(com2) >= 900 || cardCategury(com3) >= 900 || cardCategury(com4) >= 900 ||
                cardCategury(com5) >= 900 || cardCategury(com6) >= 900 || cardCategury(com7) >= 900 || cardCategury(com8) >= 900 ||
                cardCategury(com9) >= 900 || cardCategury(com10) >= 900)
        {
            return finalPoints(
            cardCategury(com1), cardCategury(com2), cardCategury(com3), cardCategury(com4), cardCategury(com5),
            cardCategury(com6), cardCategury(com7), cardCategury(com8), cardCategury(com9), cardCategury(com10)
            );
        }
        else if (cardCategury(com1) >= 800 || cardCategury(com2) >= 800 || cardCategury(com3) >= 800 || cardCategury(com4) >= 800 ||
               cardCategury(com5) >= 800 || cardCategury(com6) >= 800 || cardCategury(com7) >= 800 || cardCategury(com8) >= 800 ||
               cardCategury(com9) >= 800 || cardCategury(com10) >= 800)
        {
            return finalPoints(
            cardCategury(com1), cardCategury(com2), cardCategury(com3), cardCategury(com4), cardCategury(com5),
            cardCategury(com6), cardCategury(com7), cardCategury(com8), cardCategury(com9), cardCategury(com10)
            );

        }
        else if (cardCategury(com1) >= 700 || cardCategury(com2) >= 700 || cardCategury(com3) >= 700 || cardCategury(com4) >= 700 ||
               cardCategury(com5) >= 700 || cardCategury(com6) >= 700 || cardCategury(com7) >= 700 || cardCategury(com8) >= 700 ||
               cardCategury(com9) >= 700 || cardCategury(com10) >= 700)
        {
            return finalPoints(
            cardCategury(com1), cardCategury(com2), cardCategury(com3), cardCategury(com4), cardCategury(com5),
            cardCategury(com6), cardCategury(com7), cardCategury(com8), cardCategury(com9), cardCategury(com10)
            );

        }
        else if (cardCategury(com1) >= 600 || cardCategury(com2) >= 600 || cardCategury(com3) >= 600 || cardCategury(com4) >= 600 ||
               cardCategury(com5) >= 600 || cardCategury(com6) >= 600 || cardCategury(com7) >= 600 || cardCategury(com8) >= 600 ||
               cardCategury(com9) >= 600 || cardCategury(com10) >= 600)
        {
            return finalPoints(
            cardCategury(com1), cardCategury(com2), cardCategury(com3), cardCategury(com4), cardCategury(com5),
            cardCategury(com6), cardCategury(com7), cardCategury(com8), cardCategury(com9), cardCategury(com10)
            );

        }
        else if (cardCategury(com1) >= 500 || cardCategury(com2) >= 500 || cardCategury(com3) >= 500 || cardCategury(com4) >= 500 ||
               cardCategury(com5) >= 500 || cardCategury(com6) >= 500 || cardCategury(com7) >= 500 || cardCategury(com8) >= 500 ||
               cardCategury(com9) >= 500 || cardCategury(com10) >= 500)
        {
            return finalPoints(
            cardCategury(com1), cardCategury(com2), cardCategury(com3), cardCategury(com4), cardCategury(com5),
            cardCategury(com6), cardCategury(com7), cardCategury(com8), cardCategury(com9), cardCategury(com10)
            );
        }
        else if (cardCategury(com1) >= 400 || cardCategury(com2) >= 400 || cardCategury(com3) >= 400 || cardCategury(com4) >= 400 ||
               cardCategury(com5) >= 400 || cardCategury(com6) >= 400 || cardCategury(com7) >= 400 || cardCategury(com8) >= 400 ||
               cardCategury(com9) >= 400 || cardCategury(com10) >= 400)
        {
            return finalPoints(
            cardCategury(com1), cardCategury(com2), cardCategury(com3), cardCategury(com4), cardCategury(com5),
            cardCategury(com6), cardCategury(com7), cardCategury(com8), cardCategury(com9), cardCategury(com10)
            );
        }
        else if (cardCategury(com1) >= 300 || cardCategury(com2) >= 300 || cardCategury(com3) >= 300 || cardCategury(com4) >= 300 ||
               cardCategury(com5) >= 300 || cardCategury(com6) >= 300 || cardCategury(com7) >= 300 || cardCategury(com8) >= 300 ||
               cardCategury(com9) >= 300 || cardCategury(com10) >= 300)
        {
            return finalPoints(
            cardCategury(com1), cardCategury(com2), cardCategury(com3), cardCategury(com4), cardCategury(com5),
            cardCategury(com6), cardCategury(com7), cardCategury(com8), cardCategury(com9), cardCategury(com10)
            );

        }
        else if (cardCategury(com1) >= 200 || cardCategury(com2) >= 200 || cardCategury(com3) >= 200 || cardCategury(com4) >= 200 ||
               cardCategury(com5) >= 200 || cardCategury(com6) >= 200 || cardCategury(com7) >= 200 || cardCategury(com8) >= 200 ||
               cardCategury(com9) >= 200 || cardCategury(com10) >= 200)
        {
            return finalPoints(
            cardCategury(com1), cardCategury(com2), cardCategury(com3), cardCategury(com4), cardCategury(com5),
            cardCategury(com6), cardCategury(com7), cardCategury(com8), cardCategury(com9), cardCategury(com10)
            );

        }
        else if (cardCategury(com1) >= 100 || cardCategury(com2) >= 100 || cardCategury(com3) >= 100 || cardCategury(com4) >= 100 ||
               cardCategury(com5) >= 100 || cardCategury(com6) >= 100 || cardCategury(com7) >= 100 || cardCategury(com8) >= 100 ||
               cardCategury(com9) >= 100 || cardCategury(com10) >= 100)
        {
            return finalPoints(
            cardCategury(com1), cardCategury(com2), cardCategury(com3), cardCategury(com4), cardCategury(com5),
            cardCategury(com6), cardCategury(com7), cardCategury(com8), cardCategury(com9), cardCategury(com10)
            );
        }
        else
        {
            return finalPoints(
            cardCategury(com1), cardCategury(com2), cardCategury(com3), cardCategury(com4), cardCategury(com5),
            cardCategury(com6), cardCategury(com7), cardCategury(com8), cardCategury(com9), cardCategury(com10)
            );
        }
    }

    //return the cards and situation values
    public static int cardCategury(Card[] com)
    {
        int sum = com[0].ReturnValue() + com[1].ReturnValue() + com[2].ReturnValue() + com[3].ReturnValue() + com[4].ReturnValue();
        if (checkSuite(com[0], com[1], com[2], com[3], com[4]))
        {//RF,SF,F
            sum += sameSuite(com[0], com[1], com[2], com[3], com[4]);
        }
        else
        {//4K,FH,3K,2P,1P
            sum += sameValue(com[0], com[1], com[2], com[3], com[4]);
        }
        return sum;
    }

    public static int sameSuite(Card card1, Card card2, Card card3, Card card4, Card card5)
    {
        int sum = 500;
        if (discrepancy(card1, card2, card3, card4, card5) == 1)
        {// check for royal Flush
            if (card1.ReturnValue() == 14 || card2.ReturnValue() == 14 || card3.ReturnValue() == 14 || card4.ReturnValue() == 14 || card5.ReturnValue() == 14)
            {
                return (sum = 900);
            }
            else
            {// check for straight Flush
                return (sum = 800);
            }
        }
        return sum;
    }

    public static int sameValue(Card card1, Card card2, Card card3, Card card4, Card card5)
    {
        int sum = 0;
        if (checkValue(card1, card2, card3, card4, card5))
        {//check for 4K
            return (sum = 700);
        }
        else if (discrepancy(card1, card2, card3, card4, card5) == 4)
        {
            return (sum = 600);
        }
        else if (discrepancy(card1, card2, card3, card4, card5) == 1)
        {
            return (sum = 400);
        }
        else if (discrepancy(card1, card2, card3, card4, card5) == 0)
        {
            return (sum = 300);
        }
        else if (discrepancy(card1, card2, card3, card4, card5) == 2)
        {//check for 2P
            return (sum = 200);
        }
        else if (discrepancy(card1, card2, card3, card4, card5) == 3)
        {// check for 1P
            return (sum = 100);
        }
        else
        {
            return sum;
        }
    }

    public static bool checkValue(Card card1, Card card2, Card card3, Card card4, Card card5)
    {
        if (
        ((card1.ReturnValue() == card2.ReturnValue()) && (card1.ReturnValue() == card3.ReturnValue()) && (card1.ReturnValue() == card4.ReturnValue())) ||
        ((card1.ReturnValue() == card2.ReturnValue()) && (card1.ReturnValue() == card3.ReturnValue()) && (card1.ReturnValue() == card5.ReturnValue())) ||
        ((card1.ReturnValue() == card2.ReturnValue()) && (card1.ReturnValue() == card4.ReturnValue()) && (card1.ReturnValue() == card5.ReturnValue())) ||
        ((card1.ReturnValue() == card3.ReturnValue()) && (card1.ReturnValue() == card4.ReturnValue()) && (card1.ReturnValue() == card5.ReturnValue())) ||
        ((card2.ReturnValue() == card3.ReturnValue()) && (card2.ReturnValue() == card4.ReturnValue()) && (card2.ReturnValue() == card5.ReturnValue()))
        )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //checking the values or 3K or FH
    public static int checkAllValue(Card card1, Card card2, Card card3, Card card4, Card card5)
    {
        if (discrepancy(card1, card2, card3, card4, card5) == 0)
        {
            return 0;
        }else
        {
            return -1;

        }
     }

    public static bool checkSuite(Card card1, Card card2, Card card3, Card card4, Card card5)
    {
        if (checkAllSuite(card1, card2, card3, card4, card5) == 1 ||
           checkAllSuite(card1, card2, card3, card4, card5) == 2 ||
           checkAllSuite(card1, card2, card3, card4, card5) == 3 ||
           checkAllSuite(card1, card2, card3, card4, card5) == 4)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //checking the all cards suite, if all the same return 1~4 - helper method for checkSuite
    public static int checkAllSuite(Card c1, Card c2, Card c3, Card c4, Card c5)
    {
        if (c1.ReturnSuite() == 1 && c2.ReturnSuite() == 1 && c3.ReturnSuite() == 1 && c4.ReturnSuite() == 1 && c5.ReturnSuite() == 1)
        {
            return 1;
        }
        else if (c1.ReturnSuite() == 2 && c2.ReturnSuite() == 2 && c3.ReturnSuite() == 2 && c4.ReturnSuite() == 2 && c5.ReturnSuite() == 2)
        {
            return 2;
        }
        else if (c1.ReturnSuite() == 3 && c2.ReturnSuite() == 3 && c3.ReturnSuite() == 3 && c4.ReturnSuite() == 3 && c5.ReturnSuite() == 3)
        {
            return 3;
        }
        else if (c1.ReturnSuite() == 4 && c2.ReturnSuite() == 4 && c3.ReturnSuite() == 4 && c4.ReturnSuite() == 4 && c5.ReturnSuite() == 4)
        {
            return 3;
        }
        else
        {
            return 0;
        }
    }

    //vheck the card with value of 1 and decide does it ACE or 1
    public static void checkAce(Card c1, Card c2, Card c3, Card c4, Card c5, Card c6, Card c7)
    {

        if (
        (c1.ReturnValue() == 13 || c2.ReturnValue() == 13 || c3.ReturnValue() == 13 || c4.ReturnValue() == 13 || c5.ReturnValue() == 13 || c6.ReturnValue() == 13 || c7.ReturnValue() == 13) &&
         (c1.ReturnValue() == 12 || c2.ReturnValue() == 12 || c3.ReturnValue() == 12 || c4.ReturnValue() == 12 || c5.ReturnValue() == 12 || c6.ReturnValue() == 12 || c7.ReturnValue() == 12)
        )
        {
            if (c1.ReturnValue() == 1) { c1.SetValue(14); }
            else if (c2.ReturnValue() == 1) { c2.SetValue(14); }
            else if (c3.ReturnValue() == 1) { c3.SetValue(14); }
            else if (c4.ReturnValue() == 1) { c4.SetValue(14); }
            else if (c5.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c6.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c7.ReturnValue() == 1) { c5.SetValue(14); }
        }

        if (c1.ReturnValue() == 1)
        {
            if (c2.ReturnValue() == 1) { c2.SetValue(14); }
            else if (c3.ReturnValue() == 1) { c3.SetValue(14); }
            else if (c4.ReturnValue() == 1) { c4.SetValue(14); }
            else if (c5.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c6.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c7.ReturnValue() == 1) { c5.SetValue(14); }
        }
        if (c2.ReturnValue() == 1)
        {
            if (c1.ReturnValue() == 1) { c2.SetValue(14); }
            else if (c3.ReturnValue() == 1) { c3.SetValue(14); }
            else if (c4.ReturnValue() == 1) { c4.SetValue(14); }
            else if (c5.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c6.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c7.ReturnValue() == 1) { c5.SetValue(14); }
        }
        if (c3.ReturnValue() == 1)
        {
            if (c1.ReturnValue() == 1) { c2.SetValue(14); }
            else if (c2.ReturnValue() == 1) { c3.SetValue(14); }
            else if (c4.ReturnValue() == 1) { c4.SetValue(14); }
            else if (c5.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c6.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c7.ReturnValue() == 1) { c5.SetValue(14); }
        }
        if (c4.ReturnValue() == 1)
        {
            if (c1.ReturnValue() == 1) { c2.SetValue(14); }
            else if (c2.ReturnValue() == 1) { c3.SetValue(14); }
            else if (c3.ReturnValue() == 1) { c4.SetValue(14); }
            else if (c5.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c6.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c7.ReturnValue() == 1) { c5.SetValue(14); }
        }
        if (c5.ReturnValue() == 1)
        {
            if (c1.ReturnValue() == 1) { c2.SetValue(14); }
            else if (c2.ReturnValue() == 1) { c3.SetValue(14); }
            else if (c3.ReturnValue() == 1) { c4.SetValue(14); }
            else if (c4.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c6.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c7.ReturnValue() == 1) { c5.SetValue(14); }
        }
        if (c6.ReturnValue() == 1)
        {
            if (c1.ReturnValue() == 1) { c2.SetValue(14); }
            else if (c2.ReturnValue() == 1) { c3.SetValue(14); }
            else if (c3.ReturnValue() == 1) { c4.SetValue(14); }
            else if (c4.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c5.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c7.ReturnValue() == 1) { c5.SetValue(14); }
        }
        if (c7.ReturnValue() == 1)
        {
            if (c1.ReturnValue() == 1) { c2.SetValue(14); }
            else if (c2.ReturnValue() == 1) { c3.SetValue(14); }
            else if (c3.ReturnValue() == 1) { c4.SetValue(14); }
            else if (c4.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c5.ReturnValue() == 1) { c5.SetValue(14); }
            else if (c6.ReturnValue() == 1) { c5.SetValue(14); }
        }
    }

    //calculating the difference between cards which could be eigher in order or not
    public static int discrepancy(Card c1, Card c2, Card c3, Card c4, Card c5)
    {
        int d12 = (c1.ReturnValue() - c2.ReturnValue());
        int d13 = (c1.ReturnValue() - c3.ReturnValue());
        int d14 = (c1.ReturnValue() - c4.ReturnValue());
        int d15 = (c1.ReturnValue() - c5.ReturnValue());
        int d23 = (c2.ReturnValue() - c3.ReturnValue());
        int d24 = (c2.ReturnValue() - c4.ReturnValue());
        int d25 = (c2.ReturnValue() - c5.ReturnValue());
        int d34 = (c3.ReturnValue() - c4.ReturnValue());
        int d35 = (c3.ReturnValue() - c4.ReturnValue());
        int d45 = (c4.ReturnValue() - c5.ReturnValue());
        if (//check for 1 pair
            ((d12 == 0) && (d13 != 0) && (d14 != 0) && (d15 != 0) && (d23 != 0) && (d24 != 0) && (d25 != 0) && (d34 != 0) && (d35 != 0) && (d45 != 0)) ||
            ((d13 == 0) && (d12 != 0) && (d14 != 0) && (d15 != 0) && (d23 != 0) && (d24 != 0) && (d25 != 0) && (d34 != 0) && (d35 != 0) && (d45 != 0)) ||
            ((d14 == 0) && (d12 != 0) && (d13 != 0) && (d15 != 0) && (d23 != 0) && (d24 != 0) && (d25 != 0) && (d34 != 0) && (d35 != 0) && (d45 != 0)) ||
            ((d15 == 0) && (d12 != 0) && (d13 != 0) && (d14 != 0) && (d23 != 0) && (d24 != 0) && (d25 != 0) && (d34 != 0) && (d35 != 0) && (d45 != 0)) ||
            ((d23 == 0) && (d13 != 0) && (d14 != 0) && (d15 != 0) && (d12 != 0) && (d24 != 0) && (d25 != 0) && (d34 != 0) && (d35 != 0) && (d45 != 0)) ||
            ((d24 == 0) && (d13 != 0) && (d14 != 0) && (d15 != 0) && (d23 != 0) && (d12 != 0) && (d25 != 0) && (d34 != 0) && (d35 != 0) && (d45 != 0)) ||
            ((d25 == 0) && (d13 != 0) && (d14 != 0) && (d15 != 0) && (d23 != 0) && (d24 != 0) && (d12 != 0) && (d34 != 0) && (d35 != 0) && (d45 != 0)) ||
            ((d34 == 0) && (d13 != 0) && (d14 != 0) && (d15 != 0) && (d23 != 0) && (d24 != 0) && (d25 != 0) && (d12 != 0) && (d35 != 0) && (d45 != 0)) ||
            ((d35 == 0) && (d13 != 0) && (d14 != 0) && (d15 != 0) && (d23 != 0) && (d24 != 0) && (d25 != 0) && (d34 != 0) && (d12 != 0) && (d45 != 0)) ||
            ((d45 == 0) && (d13 != 0) && (d14 != 0) && (d15 != 0) && (d23 != 0) && (d24 != 0) && (d25 != 0) && (d34 != 0) && (d35 != 0) && (d12 != 0))
          )
        {
            return 3;

        }
        else if ((d12 == 0 && d23 == 0 && d45 == 0) || (d12 == 0 && d24 == 0 && d35 == 0) || (d12 == 0 && d25 == 0 && d34 == 0) ||
                    (d13 == 0 && d34 == 0 && d25 == 0) || (d13 == 0 && d35 == 0 && d24 == 0) || (d14 == 0 && d45 == 0 && d23 == 0) ||
                    (d23 == 0 && d34 == 0 && d15 == 0) || (d24 == 0 && d45 == 0 && d13 == 0) || (d23 == 0 && d35 == 0 && d14 == 0) ||
                    (d34 == 0 && d45 == 0 && d12 == 0)
                    )
        {// check for FH
            return 4;

        }
        else if ((d12 == 0 && d23 == 0) || (d12 == 0 && d24 == 0) || (d12 == 0 && d25 == 0) || (d13 == 0 && d34 == 0) || (d13 == 0 && d35 == 0) ||
                 (d14 == 0 && d45 == 0) || (d23 == 0 && d34 == 0) || (d24 == 0 && d45 == 0) || (d23 == 0 && d35 == 0) || (d34 == 0 && d45 == 0))
        {
            //check for 3kind
            return 0;
        }
        else if (//check for 2 pairs
           (d12 == 0 && d34 == 0) || (d12 == 0 && d35 == 0) || (d12 == 0 && d45 == 0) || (d13 == 0 && d24 == 0) || (d13 == 0 && d25 == 0) ||
           (d13 == 0 && d45 == 0) || (d14 == 0 && d23 == 0) || (d14 == 0 && d25 == 0) || (d14 == 0 && d35 == 0) || (d15 == 0 && d23 == 0) ||
           (d15 == 0 && d34 == 0) || (d23 == 0 && d45 == 0) || (d24 == 0 && d35 == 0) || (d24 == 0 && d15 == 0) || (d34 == 0 && d25 == 0)
         )
        {
            return 2;
        }
        else if (d12 == 1 && d23 == 1 && d34 == 1 && d45 == 1)
        {
            //check for flush
            return 1;
        }
        else
        {
            return -1;
        }
    }
}

