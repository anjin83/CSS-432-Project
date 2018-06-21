using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

//Server side information about a player and relevant methods.
public class Player
{
	public Socket playerSocket;
	String playerName;
	int chips;
	public Card[] hand;
	public int currentBet;
	public int totalBet;
	private int playerID; 
	int currentLobby;
	public int maxBet;

	public Player(String name, int ID, Socket pSocket) 
	{
		playerSocket = pSocket;
		playerName = name;
		playerID = ID;
		chips = 5000;
		hand = new Card[2];
	}

	public void SetMinBet(int bet)
	{
		currentBet = bet;
	}

	public int GetChips()
	{
		return chips;
	}

	public void SetChips(int numChips)
	{
		chips = numChips;
	}
	
	public int getPlayerID()
	{
	  return playerID;
	}

	public string getName()
	{
		return playerName;
	}

	public void EmptyHand()
	{
		hand = new Card[2];
	}
}

