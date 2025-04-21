using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class PlayerHolder : TableBaseMono
{
    public List<Player> players = new List<Player>();

    private List<PlayerModel> prePlayer = new List<PlayerModel>();
    private List<PlayerModel> postPlayer = new List<PlayerModel>();
    private List<PlayerModel> selfPlayer = new List<PlayerModel>();
    private List<PlayerModel> mainList = new List<PlayerModel>();
    public Button CloseDiscardHistorypanel;
    private void Start()
    {
        
        ResetPlayers();
        CloseDiscardHistorypanel.onClick.AddListener(CloseDiscardpanel);
    }
   
    public void SeatTable()
    {
        ResetPlayers();
        
        var chairModel = gameTableEventHandler.gameTableResponse.chairListModel.chairModels;
        foreach (var model in chairModel.Where(model => model.playerModel?.userModel != null)) 
            mainList.Add(model.playerModel);
        GetPlayerSequence();
        

    }
    private void GetPlayerSequence()
    {
        var selfPlayerIndex = -1;
        for (var i = 0; i < mainList.Count; i++)
        {
            var playerModel = mainList[i];
            players[i].gameObject.SetActive(false);
            if (playerModel?.userModel == null) continue;
            if (playerModel.userModel.auth_token.Equals(PlayerPrefsManager.GetAuthToken()))
            {
                selfPlayerIndex = i;
                selfPlayer.Add(playerModel);
            }
            else if (selfPlayerIndex == -1)
            {
                prePlayer.Add(playerModel);
            }
            else
            {
                postPlayer.Add(playerModel);
            }
        }

        foreach (var model in prePlayer)
        {
            postPlayer.Add(model);
        }

        foreach (var model in postPlayer)
        {
            selfPlayer.Add(model);
        }
        ArrangeSeat();
    }
    public void ArrangeSeat()
    {
        switch (selfPlayer.Count)
        {
            case 1:
            case 5:
            case 6:
                {
                    for (var i = 0; i < selfPlayer.Count; i++)
                    {
                        players[i].SetPlayerInfo(selfPlayer[i]);
                        players[i].gameObject.SetActive(true);
                    }
                    break;
                }
            case 2:
                {
                    for (var i = 0; i < selfPlayer.Count; i++)
                    {
                        if (i > 0)
                        {
                            players[3].SetPlayerInfo(selfPlayer[i]);
                            players[3].gameObject.SetActive(true);
                        }
                        else
                        {
                            players[i].SetPlayerInfo(selfPlayer[i]);
                            players[i].gameObject.SetActive(true);
                        }
                    }

                    break;
                }
            case 3:
                {
                    for (var i = 0; i < selfPlayer.Count; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                players[i].SetPlayerInfo(selfPlayer[i]);
                                players[i].gameObject.SetActive(true);
                                break;
                            case 1:
                                players[2].SetPlayerInfo(selfPlayer[i]);
                                players[2].gameObject.SetActive(true);
                                break;
                            default:
                                players[3].SetPlayerInfo(selfPlayer[i]);
                                players[3].gameObject.SetActive(true);
                                break;
                        }
                    }

                    break;
                }
            case 4:
                {
                    for (var i = 0; i < selfPlayer.Count; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                players[i].SetPlayerInfo(selfPlayer[i]);
                                players[i].gameObject.SetActive(true);
                                break;
                            case 1:
                                players[2].SetPlayerInfo(selfPlayer[i]);
                                players[2].gameObject.SetActive(true);
                                break;
                            case 2:
                                players[3].SetPlayerInfo(selfPlayer[i]);
                                players[3].gameObject.SetActive(true);
                                break;
                            default:
                                players[4].SetPlayerInfo(selfPlayer[i]);
                                players[4].gameObject.SetActive(true);
                                break;
                        }
                    }

                    break;
                }
        }
    }

    public void SetPlayerTurn()
    {
        ResetPlayersTurn();
        var playerModels = gameTableEventHandler.gameTableResponse.liveGameModel.playerModels;
        foreach (var playerModel in playerModels.Where(playerModel => playerModel.turn))
        {
            foreach (var player in players.Where(player => player.gameObject.activeInHierarchy)
                .Where(player => player.authToken.Equals(playerModel.userModel.auth_token)))
            {
                player.SetPlayerInfo(playerModel);
                if (gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken()) == playerModel)
                {
                    GamePlayManager.instance.isCurrentPlayerTurn = playerModel.turn;
                    GamePlayManager.instance.isDeckInteracted = false;
                }
                break;
            }
            break;
        }


    }

    public void ResetPlayersTurn()
    {
        foreach (var player in players)
        {
            player.ResetTime();
            player.ResetDiscardedCards();//SAI
        }
    }

    public bool IsSelfPlayer()
    {
        return gameTableEventHandler.gameTableResponse.chairListModel.chairModels.
            Where(chair => chair.playerModel?.userModel != null).Any(chair => chair
                .playerModel.userModel.auth_token.Equals(PlayerPrefsManager.GetAuthToken()));
    }

    public void ResetPlayers()
    {
        selfPlayer.Clear();
        prePlayer.Clear();
        postPlayer.Clear();
        mainList.Clear();
        foreach (var player in players)
        {
           
            player.gameObject.SetActive(false);
            player.ResetTime();
        }

    }
    public void SinglePlayerDiscardHistory(Player Singleplayer) {

        CloseDiscardHistorypanel.gameObject.SetActive(true);

        foreach (var player  in players)
        {
            if (player.gameObject.activeInHierarchy)
            {
                if (player.DiscardHistoryStatus && player != Singleplayer)
                {

                    player.DiscardHistroryGrid.transform.parent.gameObject.SetActive(false);
                    player.DiscardHistoryStatus = false;
                }
            }
        }
        
    }

    void CloseDiscardpanel() {

        foreach (var player in players)
        {
            if (player.gameObject.activeInHierarchy)
            {
                player.DiscardHistroryGrid.transform.parent.gameObject.SetActive(false);
                player.DiscardHistoryStatus = false;
            }

        }

        CloseDiscardHistorypanel.gameObject.SetActive(false);
    }

    public void UpdatePlayerStatus()
    {
        foreach (var player in players)
        {
            if (player.gameObject.activeInHierarchy)
            {
                player.SetPlayerStatus();
            }
        }
    }

    public void ResetPlayerHolder()
    {
        foreach (var player in players)
        {
            player.ResetDiscardedCards();
        }
    }

}
