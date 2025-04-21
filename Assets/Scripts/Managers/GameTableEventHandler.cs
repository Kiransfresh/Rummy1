using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameTableEventHandler : TableBaseMono
{
    [HideInInspector] public GameTableResponse gameTableResponse = null;

    public SnackBar snackBar;
    public MessageInfo messageInfo;
    public PlayerHolder playerHolder;
    public CutForSeat cutForSeat;
    public TableHeader tableHeader;
    public GameResult gameResult;
    public ButtonStats buttonStats;
    //  public MeldCards meldCards;
    public FortuneWheel fortuneWheel;
    public GameRoomAnimationController AnimationController;
    public RejoinGame rejoinGame;
    public SplitGame splitGame;
    public GameObject blockpanel;
    private Coroutine messageInfoCoroutine;

    private bool isPause;
    [HideInInspector]
    public string CurrentEvent;
    private void Start()
    {
        blockpanel.SetActive(true);
        ServerManager.instance.GameTableCallback = GameTableResponseHandler;
        ServerManager.instance.splitCallback = SPLIT;
        ServerManager.instance.rejoinCallback = REJOIN_GAME;
       // blockpaneloff(response);
    }

    public static GameTableEventHandler Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void blockpaneloff(Response<GameTableResponse> response)
    {
     if(response.data.gameWaitingTime == 0)
        {
         blockpanel.SetActive(false);
        } 
    }

    private void GameTableResponseHandler(Response<GameTableResponse> response)
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ServerManager.instance.loader.HideLoader();
        if (response.status.Equals(Constants.KEYS.valid))
        {
            if (CacheMemory.RunningTableId != null && CacheMemory.RunningTableId == response.data.id)
            {
                gameTableResponse = response.data;
                var eventType = response.eventType;
                CurrentEvent = eventType;
                playerHolder.UpdatePlayerStatus();
                Debug.Log("Game Table Events: " + eventType);
                if (eventType.Equals(Constants.GAME_TABLE_EVENT.PLAYER_TAKE_SEAT))
                {
                    TAKE_SEAT();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.PLAYER_LEAVE_TABLE))
                {
                    LEAVE_TABLE();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.PLAYER_SWITCH_TABLE))
                {
                    SWITCH_TABLE();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.PLAYER_DISCONNECTED))
                {
                    playerHolder.SeatTable();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.PLAYER_REMOVED))
                {
                    PLAYER_REMOVED(response.message);
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.WAITING_FOR_OTHER_PLAYER))
                {
                    WAITING_FOR_PLAYER();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.WAITING_FOR_GAME_START))
                {
                    WAITING_FOR_GAME(response);
                    UIManager.instance.sharePrivateTableCode.gameObject.SetActive(false);
                    buttonStats.SharePrivateTableButtonStats(false);
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.TOSS))
                {
                    blockpanel.SetActive(false);
                    CUT_FOR_SEAT();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.RE_ARRANGE))
                {
                    RE_ARRANGE();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.CARD_DEALING))
                {
                    DEALING(true);
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.PLAYER_DROP))
                {
                    DROP();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.PLAYER_EXTRA_TIME))
                {
                    PLAYER_EXTRA_TIME();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.PLAYER_AUTO_PLAY))
                {

                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.CARD_PICKED))
                {
                    CARD_PICKED();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.PLAYER_TURN_FINISH))
                {
                    FINISH_PLAYER_TURN();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.INFO_MESSAGE))
                {
                    messageInfoCoroutine = StartCoroutine(messageInfo.ShowMessage(response.message, 3f));
                    Debug.Log("INFO_MESSAGE : " + response.message);
                    //snackBar.ShowMessage(response.message);
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.GAME_PLAY_RESULT))
                {
                    GAME_RESULT();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.GAME_END))
                {
                    GAME_END(response.message);
                    blockpanel.SetActive(false);

                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.EXISTING_PLAYER_JOIN_TABLE))
                {
                    EXISTING_PLAYER_JOIN_TABLE(response);
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.REQUEST_MELD_CARD))
                {
                    REQUEST_MELD_CARD();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.RECEIVED_MELD_CARD))
                {
                    MELD_CARD_RECEIVED();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.UPDATE_RESULT_TIMER))
                {
                    UPDATE_RESULT_TIMER(response);
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.UPDATE_DECK_CARDS))
                {
                    StartCoroutine(GamePlayManager.instance.SetOpenDeckCards());
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.REFRESH_TABLE))
                {
                    REFRESH_TABLE();
                }
            }
        }
        else
        {
            ServerManager.instance.alertPopUp.ShowView(response.message, () => { LeaveTable(); }, "Ok", null, null);
        }
    }

    private void CARD_PICKED()
    {
        StartCoroutine(GamePlayManager.instance.SetOpenDeckCards());
        GamePlayManager.instance.OtherPlayersCardPick();
    }

    private void RE_ARRANGE()
    {
        playerHolder.SeatTable();
    }

    private void WAITING_FOR_GAME(Response<GameTableResponse> response)
    {
        messageInfoCoroutine = StartCoroutine(messageInfo.ShowWithMessage(Constants.MESSAGE.WAITING_FOR_GAME,
            response.data.gameWaitingTime));
    }

    private void WAITING_FOR_PLAYER()
    {
        messageInfo.ShowWithMessage(Constants.MESSAGE.WAITING_FOR_PLAYER);
        if (messageInfoCoroutine != null)
            StopCoroutine(messageInfoCoroutine);
        playerHolder.SeatTable();
    }

    private void SWITCH_TABLE()
    {
        tableHeader.SetTableHeader();
        playerHolder.SeatTable();
    }

    private void LEAVE_TABLE()
    {
        tableHeader.SetTableHeader();
        playerHolder.SeatTable();
    }

    private void TAKE_SEAT()
    {
        blockpanel.SetActive(true);
        tableHeader.SetTableHeader();
        playerHolder.SeatTable();
        if (gameTableResponse.tableEnum.Equals(Constants.TABLE_ENUM.WAITING_FOR_GAME))
        {
            if (!messageInfo.gameObject.activeInHierarchy)
            {
                var elapsedTime = gameTableResponse.timer.elapsedCounter;
                var time = gameTableResponse.gameWaitingTime - elapsedTime;
                messageInfoCoroutine = StartCoroutine(messageInfo.ShowWithMessage(Constants.MESSAGE.WAITING_FOR_GAME,
                    time));
            }
        }
    }

    private void PLAYER_EXTRA_TIME()
    {
        Debug.Log(" Player extra time Gametable ");
        playerHolder.SetPlayerTurn();
    }

    private void FINISH_PLAYER_TURN()
    {
        AudioController.instance.StopTimer();
        GamePlayManager.instance.AutoDrop();
        buttonStats.isGameSarted = true;
   
        playerHolder.SetPlayerTurn();
        if (isAnimRun)
        {
            GamePlayManager.instance.OtherPlayersDiscardCard();
        }
        isAnimRun = true;
        StartCoroutine(GamePlayManager.instance.SetOpenDeckCards());
        GamePlayManager.instance.isCardPicked = false;

    }

    private void DEALING(bool isAnim)
    {
        AudioController.instance.StopTimer();
        messageInfo.gameObject.SetActive(false);
        cutForSeat.ResetCards();
        playerHolder.SeatTable();
        tableHeader.SetTableHeader();
        if (isAnim)
        {
            gameResult.authTokens.Clear();
            //meldCards.ResetMeldCards();
            AnimationController.DistributionEnd.RemoveAllListeners();
            AnimationController.DistributionEnd.AddListener(() =>
            {

                var player = gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken());
                if (player != null)
                {
                    GamePlayManager.instance.StartDealing(player.inHandCardList);
                }
            }
            );
            AnimationController.StartDistribution();
        }
        else if (gameTableResponse != null)
        {

            var player = gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken());
            if (player != null && gameTableResponse.liveGameModel != null)
            {
                var cardList = player.inHandCardList;
                if (gameTableResponse.liveGameModel.gamePlayCardModel.picked != null
                    && !string.IsNullOrEmpty(gameTableResponse.liveGameModel.gamePlayCardModel.picked.auth_token)
                    && !string.IsNullOrWhiteSpace(gameTableResponse.liveGameModel.gamePlayCardModel.picked.auth_token)
                    && gameTableResponse.liveGameModel.gamePlayCardModel.picked.auth_token.Equals(player.userModel
                        .auth_token))
                { //&& !meldCards.gameObject.activeInHierarchy
                    var groupIds = new List<int>();
                    foreach (var cardModel in cardList.Where(cardModel => !groupIds.Contains(cardModel.groupId)))
                    {
                        groupIds.Add(cardModel.groupId);
                    }

                    gameTableResponse.liveGameModel.gamePlayCardModel.picked.cardModel.groupId =
                        groupIds[groupIds.Count - 1];
                    cardList.Add(gameTableResponse.liveGameModel.gamePlayCardModel.picked.cardModel);
                }

                GamePlayManager.instance.StartDealing(cardList: cardList);
                if (player.playerEnum.Equals(Constants.PLAYER_ENUM.DROP)
                    || player.playerEnum.Equals(Constants.PLAYER_ENUM.WRONG_SHOW)
                    || player.playerEnum.Equals(Constants.PLAYER_ENUM.MIDDLE_DROP))
                {
                    DROP();
                }
                else
                {
                    GamePlayManager.instance.AnimationController.IsInShrinkState = true;
                    GamePlayManager.instance.AnimationController.ShrinkOrStrechCards();
                }
            }


        }
    }

    private void OnApplicationQuit()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        LeaveTable();
    }




    private void OnApplicationFocus(bool isFocus)
    {

#if !UNITY_EDITOR && !UNITY_WEBGL

        if (isFocus)
        {
            if (gameTableResponse.liveGameModel != null
                && gameTableResponse.liveGameModel.gameEnum != null
                && UIManager.instance.gameRoom.activeInHierarchy)
            {
                RefreshTable();
            }
        }
#endif
    }

    private void RefreshTable()
    {
        var request = new GameTableRequest();
        request.table_id = gameTableEventHandler.gameTableResponse.id;
        request.table_event_type = Constants.GAME_TABLE_EVENT.REFRESH_TABLE;
        var sfsObject = request.ToISfsObject();
        SendGameTableRequest(sfsObject);
    }

    public void ResetAllBeforeFocus()
    {
        GamePlayManager.instance.DeselectAllCards();
        playerHolder.ResetPlayerHolder();
        playerHolder.UpdatePlayerStatus();
        AudioController.instance.StopSounds();
        GamePlayManager.instance.ResetGameplay();
        splitGame.ResetSplitRows();
    }


    public void LeaveTable()
    {
        SendGameTableRequest(Constants.GAME_TABLE_EVENT.PLAYER_LEAVE_TABLE, gameTableResponse.id);
        ResetTable();

    }

    public void ResetTable()
    {
        //RESET SHARE BUTTONS 

        gameResult.gameObject.SetActive(false);
        splitGame.ResetSplitRows();
        tableHeader.gameSettingMenu.ReportProblemPanel.SetActive(false);

        CacheMemory.RunningTableId = null;

#if UNITY_WEBGL
        Application.ExternalEval("Quit()");
#else
        UIManager.instance.lobbyView.gameObject.SetActive(true);
        if (UIManager.instance.gameRoom.GetComponent<GameRoomCanvas>().gameObject.activeSelf == true)
        {
            UIManager.instance.gameRoom.GetComponent<GameRoomCanvas>().DisableRoom();
        }

#endif

        //RESET SHARE BUTTONS 
        UIManager.instance.sharePrivateTableCode.gameObject.SetActive(false);
        buttonStats.SharePrivateTableButtonStats(false);

        playerHolder.ResetPlayers();
        cutForSeat.ResetCards();
        GamePlayManager.instance.ResetGameplay();
    }

    private void CUT_FOR_SEAT()
    {
        blockpanel.SetActive(false);
        playerHolder.SeatTable();
        tableHeader.SetTableHeader();
        if (messageInfoCoroutine != null)
            StopCoroutine(messageInfoCoroutine);
        cutForSeat.StartCutForSeat();
    }

    public bool isAnimRun;
    public bool isDropOnTimeOut = false;

    private void DROP()
    {
        AudioController.instance.StopSounds();
        var selfPlayer = gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken());

        if ((selfPlayer.playerEnum.Equals(Constants.PLAYER_ENUM.DROP)
             || selfPlayer.playerEnum.Equals(Constants.PLAYER_ENUM.MIDDLE_DROP)
             || selfPlayer.playerEnum.Equals(Constants.PLAYER_ENUM.WRONG_SHOW)) && !isDropOnTimeOut)
        {

            GamePlayManager.instance.AnimationController.IsInShrinkState = false;

            GamePlayManager.instance.ShrinkOrStrechForDrop(
                selfPlayer.playerEnum.Equals(Constants.PLAYER_ENUM.WRONG_SHOW));
            PlayerPrefs.DeleteKey(Constants.KEYS.Selection_Counter);
            isDropOnTimeOut = true;
        }

        var playerModel = gameTableResponse.GetTurnPlayer();
        if (playerModel.playerEnum.Equals(Constants.PLAYER_ENUM.WRONG_SHOW))
        {
            isAnimRun = true;
            var cardModel = gameTableResponse.liveGameModel.gamePlayCardModel.discarded.cardModel;
            var spriteName = cardModel.suit + "_" + cardModel.rank;
            GamePlayManager.instance.FinishCardAnimation.gameObject.SetActive(false);
            GamePlayManager.instance.cardForAnimation.SetOpenCardSprite(GamePlayManager.instance.GetSprite(spriteName));
            GamePlayManager.instance.cardForAnimation.DiscardAnimation(GamePlayManager.instance.FinishCardAnimation.transform.position);
            GamePlayManager.instance.cardForAnimation.gameObject.SetActive(true);
        }
        else
        {
            isAnimRun = false;
        }
    }

    private void GAME_RESULT()
    {
        playerHolder.ResetPlayersTurn();
        cutForSeat.ResetCards();
        GamePlayManager.instance.ResetGameplay();
        gameResult.lastDealandScorecardToggleGroup.gameObject.SetActive(false);
        gameResult.ShowResult(gameTableResponse.liveGameModel, false);
    }

    private void MELD_CARD_RECEIVED()
    {
        if (gameTableResponse.liveGameModel != null)
        {
            if (gameTableResponse.liveGameModel.gamePlayCardModel.discarded != null)
            {
                var cardModel = gameTableResponse.liveGameModel.gamePlayCardModel.discarded.cardModel;
                var spriteName = cardModel.suit + "_" + cardModel.rank;
                GamePlayManager.instance.FinishCardAnimation.GetComponent<Image>().sprite =
                    GamePlayManager.instance.GetSprite(spriteName);
            }


            var player = gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken());
            if (gameTableResponse.liveGameModel.gameEnum.Equals(Constants.GAME_ENUM.GAME_DECLARE))
            {
                if (player != null && player.meldCardReceived)
                {
                    GAME_RESULT();
                }
                else
                {
                    messageInfo.ShowWithMessage("Please meld your cards");
                    AudioController.instance.OnShow();
                }
            }

            if (player.playerEnum.Equals(Constants.PLAYER_ENUM.DROP) ||
                player.playerEnum.Equals(Constants.PLAYER_ENUM.MIDDLE_DROP) &&
                player.playerEnum.Equals(Constants.PLAYER_ENUM.WRONG_SHOW))
            {
                GamePlayManager.instance.inHandCards.SetActive(false);
            }
            else
            {
                GamePlayManager.instance.inHandCards.SetActive(true);
            }

            if (!gameTableResponse.liveGameModel.gameEnum.Equals(Constants.GAME_ENUM.PLAYER_TURN))
            {
                if (player.meldCardReceived)
                {
                    GamePlayManager.instance.SetAllDeck(false);
                }
                else
                {
                    messageInfo.ShowWithMessage("Please meld your cards");
                    AudioController.instance.OnShow();
                    // meldCards.gameObject.SetActive(true);

                }
            }

            playerHolder.SeatTable();
        }
    }

    private void REQUEST_MELD_CARD()
    {
        if (gameTableResponse.liveGameModel.gameEnum.Equals(Constants.GAME_ENUM.PLAYER_TURN))
        {
            if (!gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken()).turn)
            {
                var message = gameTableResponse.GetTurnPlayer().userModel.unique_name +
                              " Declared, please wait for validation";
                messageInfo.ShowWithMessage(message);
                GamePlayManager.instance.OtherPlayersDiscardCard();
            }
            else
            {

                if (gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken()).meldCardReceived)
                {
                    GAME_RESULT();
                }
                else
                {
                    GamePlayManager.instance.UpdateTimer();
                }
            }
        }
        else if (gameTableResponse.liveGameModel.gameEnum.Equals(Constants.GAME_ENUM.GAME_DECLARE))
        {
            var playerModel = gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken());
            if (playerModel != null)
            {
                if (playerModel.meldCardReceived)
                {
                    GAME_RESULT();
                }
                else
                {
                    GamePlayManager.instance.UpdateTimer();
                }
            }
        }

        playerHolder.SeatTable();
    }

    private void GAME_END(string message)
    {
        gameResult.gameObject.SetActive(false);
        AudioController.instance.StopTimer();
        PlayerPrefs.DeleteKey(Constants.KEYS.Selection_Counter);
        ServerManager.instance.alertPopUp.DisableVerificationPanel();
        playerHolder.SeatTable();
        GamePlayManager.instance.ResetGameplay();
    }

    private void PLAYER_REMOVED(string message)
    {
        tableHeader.SetTableHeader();
        playerHolder.SeatTable();

        if ((gameTableResponse.gameModel.game_sub_type == Constants.GAME_SUB_TYPE.POOL || gameTableResponse.gameModel.game_sub_type == Constants.GAME_SUB_TYPE.DEALS) && gameTableResponse.selfWinner())
        {
            string gameType = "In " + gameTableResponse.gameModel.game_sub_type;
            if (gameTableResponse.gameModel.game_sub_type == Constants.GAME_SUB_TYPE.POOL)
            {
                gameType = gameType + " " + gameTableResponse.gameModel.pool_game_type;
            }
            //gameType = gameType + " ● " + gameTableResponse.gameModel.number_of_players_per_table ;
            string entry = gameTableResponse.gameModel.entry_fee;
            ServerManager.instance.winAlertPopUp.ShowView(message, gameType, entry, () => {
                ResetTable();
            });
        }
        else
        {
            ServerManager.instance.alertPopUp.ShowView(message, () =>
            {
                ResetTable();
            }, "Ok", null, null);
        }

    }

    private Coroutine dealingCoroutine = null;

    private void EXISTING_PLAYER_JOIN_TABLE(Response<GameTableResponse> response)
    {
        if (response.data.liveGameModel != null && response.data.liveGameModel.gameEnum != null
                                                && !response.data.liveGameModel.gameEnum.Equals(
                                                    Constants.GAME_ENUM.NONE)
                                                && response.data.rejoinedUserAuthToken.Equals(PlayerPrefsManager
                                                    .GetAuthToken()))
        {
            var eventType = response.data.liveGameModel.eventType;
            buttonStats.isGameSarted = true;
            GamePlayManager.instance.isCardPicked = false;
            GamePlayManager.instance.isGameFinish = false;
            AudioController.instance.StopSounds();
            GamePlayManager.instance.ClearList();
            tableHeader.SetTableHeader();
            if (response.data.liveGameModel.gameEnum.Equals(Constants.GAME_ENUM.TOSS))
            {
                CUT_FOR_SEAT();
            }
            else
            {
                GamePlayManager.instance.ResetSlots();
                DEALING(false);

                if (eventType.Equals(Constants.GAME_TABLE_EVENT.PLAYER_DROP))
                {
                    DROP();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.PLAYER_TURN_FINISH))
                {
                    FINISH_PLAYER_TURN();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.PLAYER_EXTRA_TIME))
                {
                    PLAYER_EXTRA_TIME();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.GAME_PLAY_RESULT))
                {
                    GAME_RESULT();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.GAME_END))
                {
                    GAME_END(response.message);
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.REQUEST_MELD_CARD))
                {
                    REQUEST_MELD_CARD();
                }
                else if (eventType.Equals(Constants.GAME_TABLE_EVENT.RECEIVED_MELD_CARD))
                {
                    MELD_CARD_RECEIVED();
                }

            }
        }
    }

    private void SPLIT(Response<SplitModel> response)
    {
        gameResult.splitBtn.gameObject.SetActive(gameResult.gameObject.activeInHierarchy);
        splitGame.SetSplitDetails(response);
    }

    private void REJOIN_GAME(Response<RejoinModel> response)
    {
        if (response.status.Equals(Constants.KEYS.valid))
        {
            rejoinGame.SetRejoinDetails(response);
        }
    }

    private void UPDATE_RESULT_TIMER(Response<GameTableResponse> response)
    {
        messageInfo.ShowWithMessage(response.message);
        if (gameResult.gameObject.activeInHierarchy)
        {
            gameResult.ResetCoroutine();
        }
    }

    private void REFRESH_TABLE()
    {

        playerHolder.ArrangeSeat();

        /* if (meldCards.gameObject.activeInHierarchy)
             meldCards.UpdateTimer(); */

        if (gameResult.gameObject.activeInHierarchy)
            gameResult.UpdateTimer();



        AudioController.instance.StopSounds();
        FINISH_PLAYER_TURN();
    }

}

