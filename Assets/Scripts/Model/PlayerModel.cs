using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerModel 
{
    public UserModel userModel;
    public string playerEnum;
    public string turnEnum;

    public bool turn;
    public bool disconnected;
    public bool dealer;
    public bool gameFinish;
    public bool leave;
    public bool cutForSeatWinner;
    public GameTableResponse.CardModel cutForSeatCard;
    public int turnCounter;
    public bool autoPlay;
    public float totalPoints;
    public float points;
    public bool rejoin;
    public bool gameWinner;
    public bool prizeWinner;
    public List<GameTableResponse.CardModel> discardedCardList;
    public List<GameTableResponse.CardModel> inHandCardList;
    public float displayChips;
    public bool meldCardReceived;
}

