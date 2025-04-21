using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sfs2X.Entities.Data;
using UnityEngine;

[Serializable]
public class GameTableRequest
{
    public string table_id = "";
    public string table_event_type = "";

    [HideInInspector] public List<GameObject> inHandGroups = new List<GameObject>();

    public ISFSObject ToISfsObject()
    {
        ISFSObject obj = new SFSObject();
        obj.PutUtfString(Constants.KEYS.table_id, table_id);
        obj.PutUtfString(Constants.KEYS.table_event_type, table_event_type);
        return obj;
    }
}

[Serializable]
public class GameTableResponse
{
    public string id;
    public ChairListModel chairListModel;
    public GameListModel gameModel;
    public LiveGameModel liveGameModel;
    public string tableEnum;
    public RTimer timer;
    public int gameWaitingTime;
    public int round = 0;
    public string rejoinedUserAuthToken;

    [Serializable]
    public class RTimer
    {
        public int elapsedCounter;
    }

    [Serializable]
    public class ChairListModel
    {
        public List<ChairModel> chairModels;
    }

    [Serializable]
    public class LiveGameModel
    {
        public RTimer timer;
        public GameListModel gameDetail;
        public GamePlayCardModel gamePlayCardModel;
        public List<PlayerModel> playerModels;
        public string gameEnum;
        public int playerInitialTurnTime;
        public int playerExtraTurnTime;
        public int playerAutoPlayTime;
        public int gameDeclareTime;
        public int gameResultTime;
        public string eventType;
        public BaseStateModel baseStateModel;
        public bool firstTurn;
    }

    [Serializable]
    public class BaseStateModel
    {
        public RTimer timer;
    }

    [Serializable]
    public class ChairModel
    {
        public int id;
        public string chairState;
        public PlayerModel playerModel;
    }

    [Serializable]
    public class GamePlayCardModel
    {
        public CardModel cutJoker;
        public List<CardModel> openDeckCardList;
        public CardModel closeDeck;
        public UserCard declared;
        public UserCard picked;
        public UserCard discarded;
    }

    [Serializable]
    public class CardModel
    {
        public int id;
        public string suit;
        public int rank;
        public int groupId;
        public bool cutJoker;
        public bool openDeck;
        public bool closeDeck;
        public string suitReArrange;

        public void SuitValueForSorting() 
        {
            if (suit.Equals(Constants.CARD_SUIT.SPADE))
            {
                suitReArrange = "A";

            }
            else if (suit.Equals(Constants.CARD_SUIT.HEART))
            {
                suitReArrange = "B";
            }
            else if (suit.Equals(Constants.CARD_SUIT.DIAMOND))
            {
                suitReArrange = "C";
            }
            else if (suit.Equals(Constants.CARD_SUIT.CLUB))
            {
                suitReArrange = "D";
            }
            else 
            {
                suitReArrange = "E";
            }
        }

        private bool IsPrintedJoker()
        {
            return suit.Equals(Constants.CARD_SUIT.JOKER);
        }

        public bool IsJoker()
        {
            return IsPrintedJoker() || cutJoker;
        }

        public CardModel Clone()
        {
            CardModel NewCardModel = new CardModel();
            NewCardModel.id = id;
            NewCardModel.suit = suit;
            NewCardModel.rank = rank;
            NewCardModel.groupId = groupId;
            NewCardModel.cutJoker = cutJoker;
            return NewCardModel;
        }
    }

    [Serializable]
    public class UserCard
    {
        public string auth_token;
        public CardModel cardModel;
        public bool closeDeck;
        public bool declare;
    }
    
    public PlayerModel GetPlayer(string authToken)
    {
        return liveGameModel?.playerModels?.FirstOrDefault(model => model
            .userModel.auth_token.Equals(authToken));
    }

    public PlayerModel GetPlayerFromChair(string authToken)
    {
        return (from chairModel in chairListModel.chairModels where chairModel.playerModel?
            .userModel != null && chairModel.playerModel.userModel.auth_token
            .Equals(authToken) select chairModel.playerModel).FirstOrDefault();
    }

    public PlayerModel GetCutForSeatWinner()
    {
        return liveGameModel?.playerModels?.FirstOrDefault(model => model.cutForSeatWinner);
    }

    public PlayerModel GetTurnPlayer()
    {
        return liveGameModel.playerModels.FirstOrDefault(model => model.turn);
    }

    public CardModel GetCutForSeatCard(string authToken)
    {
        foreach (var chairModel in chairListModel.chairModels)
        {
            if (chairModel.playerModel?.userModel != null 
                && chairModel.playerModel.userModel.auth_token == authToken)
            {
                return chairModel.playerModel.cutForSeatCard;
            }
        }
        return null;
    }


    public bool selfWinner()
    {
        Debug.Log("LOGIN - " + PlayerPrefsManager.GetAuthToken());
        
        for (int i = 0; i < liveGameModel?.playerModels?.Count; i++) {
            if (liveGameModel?.playerModels[i].prizeWinner == true && liveGameModel?.playerModels[i].userModel.auth_token == PlayerPrefsManager.GetAuthToken()) {
                Debug.Log("WINNER - " + liveGameModel?.playerModels[i].userModel.auth_token);

                return true;
            }
        }
        return false;
    }
}

