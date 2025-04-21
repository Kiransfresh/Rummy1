using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResult : TableBaseMono
{
    public GameObject pointGameUserCardParent;
    public GameObject pointGameScorecardHolder;
    public GameObject pointGameScoreCard;

    public GameObject poolGameUserCardParent;
    public GameObject poolGameScorecardHolder;
    public GameObject poolGameScoreCard;

    public Image gameCutJoker;
    public TextMeshProUGUI nextGameInfo;
    public TextMeshProUGUI tableId;
    public Button splitBtn;
    public Button leaveBtn;
    public Coroutine resultTimerCoroutine;

    public ToggleGroup lastDealandScorecardToggleGroup;
    public GameObject totalScoreboard;
    public bool isShowResult;
    public int timeAfterCloseResultPanel = 0;

    [SerializeField] private Button closeBtn;
    
    
    private bool isDrop = false;
    private bool isWinOrLoosPlay = false;

   


    private void OnEnable()
    {
        gameTableEventHandler.splitGame.gameObject.SetActive(false);
        gameTableEventHandler.rejoinGame.gameObject.SetActive(false);
        gameTableEventHandler.messageInfo.DeclareButton(false);
        isWinOrLoosPlay = false;
        splitBtn.gameObject.SetActive(false);
        authTokens.Clear();
      //  gameTableEventHandler.meldCards.gameObject.SetActive(false);
    }

    private void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            if (resultTimerCoroutine != null) 
            {
                StopCoroutine(resultTimerCoroutine);
            }
            isShowResult = true;
            gameTableEventHandler.messageInfo.DeclareButton(false);
            gameObject.SetActive(false);
        });

        leaveBtn.onClick.AddListener(() =>
        {
            string message = Constants.MESSAGE.LEAVE_TABLE_CONFIRMATION_PRACTICE;
            if (gameTableEventHandler.gameTableResponse.gameModel.game_type == Constants.GAME_TYPE.CASH) {
                message = Constants.MESSAGE.LEAVE_TABLE_CONFIRMATION;
            }
            ServerManager.instance.alertPopUp.ShowView(message,
                () =>
                {
                    gameTableEventHandler.LeaveTable();
                    AudioController.instance.StopSounds();
                }, "Yes", () => { }, "No");
        });

        splitBtn.onClick.AddListener(() =>
        {
            ServerManager.instance.SendSplitAcceptance(false);
        });
    }

    private void OnDisable()
    {
       
        gameTableEventHandler.splitGame.ResetSplitRows();
        gameTableEventHandler.rejoinGame.gameObject.SetActive(false);
        GamePlayManager.instance.FinishCardAnimation.gameObject.SetActive(false);
        splitBtn.gameObject.SetActive(false);
        AudioController.instance.StopSounds();
        ResetScorecardHolder();
        authTokens.Clear();
        isShowResult = false;
        isDrop = false;
    }

    public List<string> authTokens = new List<string>();
    public void ShowResult(GameTableResponse.LiveGameModel gameModel, bool isLastDeal)
    {
        gameObject.SetActive(true);
        if (gameModel == null) return;
        var tableModel = gameTableEventHandler.gameTableResponse;
        tableId.text = "#" + tableModel.id + "-" + tableModel.round;
        DropCounting(gameModel);
        for (var i = 0; i < gameModel.playerModels.Count; i++)
        {
            var playerModel = gameModel.playerModels[i];
            
            if (gameTableEventHandler.gameTableResponse.gameModel.game_sub_type.Equals(Constants.GAME_SUB_TYPE.POOL)
                || gameTableEventHandler.gameTableResponse.gameModel.game_sub_type.Equals(Constants.GAME_SUB_TYPE.DEALS))
            {
                poolGameScoreCard.SetActive(false);
                pointGameScoreCard.SetActive(true);
                GenerateResultCell(poolGameUserCardParent, poolGameScorecardHolder, gameModel, playerModel, i);
            }
            else
            {
                pointGameScoreCard.SetActive(false);
                poolGameScoreCard.SetActive(true);
                GenerateResultCell(pointGameUserCardParent, pointGameScorecardHolder, gameModel, playerModel, i);
            }
        }

        GameTableResponse.CardModel cardModel = gameModel.gamePlayCardModel.cutJoker;
        var name = cardModel.suit + "_" + cardModel.rank;

        GamePlayManager.instance.SetCutJoker(gameCutJoker, cardName: name);

        if (!isLastDeal)
        {
            nextGameInfo.gameObject.SetActive(true);
            PlayWinAndLoseSound();
            lastDealandScorecardToggleGroup.gameObject.SetActive(false);
            totalScoreboard.SetActive(false);

            if (gameTableEventHandler.gameTableResponse.gameModel.game_sub_type.Equals(Constants.GAME_SUB_TYPE.POOL)
                || gameTableEventHandler.gameTableResponse.gameModel.game_sub_type.Equals(Constants.GAME_SUB_TYPE.DEALS))
            {
                pointGameScoreCard.SetActive(false);
                poolGameScoreCard.SetActive(true);
            }
            else
            {
                poolGameScoreCard.SetActive(false);
                pointGameScoreCard.SetActive(true);

            }

            gameTableEventHandler.buttonStats.isGameSarted = false;
            UpdateTimer();
        }
        else
        {
            nextGameInfo.gameObject.SetActive(false);
            totalScoreboard.SetActive(false);
            timeAfterCloseResultPanel = 0;
            lastDealandScorecardToggleGroup.gameObject.SetActive(true);
            
            if (gameTableEventHandler.gameTableResponse.gameModel.game_sub_type.Equals(Constants.GAME_SUB_TYPE.POOL)
                || gameTableEventHandler.gameTableResponse.gameModel.game_sub_type.Equals(Constants.GAME_SUB_TYPE.DEALS))
            {
                pointGameScoreCard.SetActive(false);
                poolGameScoreCard.SetActive(true);
            }
            else
            {
                poolGameScoreCard.SetActive(false);
                pointGameScoreCard.SetActive(true);
            }
        }
    }

    private void GenerateResultCell(GameObject cardParent, GameObject scoreHolder, GameTableResponse.LiveGameModel gameModel, PlayerModel playerModel, int i)
    {
        if (gameModel.playerModels.Count == cardParent.transform.childCount - 1)
        {
            for (var j = 1; j < cardParent.transform.childCount; j++)
            {
                var scoreCardHolder = cardParent.transform.GetChild(j);
                var authToken = scoreCardHolder.transform.GetChild(scoreCardHolder.transform.childCount - 1)
                    .GetComponent<TextMeshProUGUI>().text;

                if (authToken.Equals(playerModel.userModel.auth_token) && !isDrop)
                {
                    SetPlayerResultInfo(scoreCardHolder, playerModel, i, isDrop);
                }
            }
        }
        else
        {
            var scoreCardHolder = Instantiate(scoreHolder, cardParent.transform);
            SetPlayerResultInfo(scoreCardHolder.transform, playerModel, i, isDrop);
        }
    }

    private void DropCounting(GameTableResponse.LiveGameModel gameModel)
    {
        var dropCount = 0;
        foreach (var playerModel in gameModel.playerModels
            .Where(playerModel => playerModel.playerEnum.Equals(Constants.PLAYER_ENUM.DROP)
                                  || playerModel.playerEnum.Equals(Constants.PLAYER_ENUM.MIDDLE_DROP)))
        {
            dropCount++;
            if (dropCount == gameModel.playerModels.Count - 1)
            {
                isDrop = true;
            }
        }
    }

    private void SetPlayerResultInfo(Transform scoreCardHolder, PlayerModel playerModel, int i, bool isDrop)
    {
       
        scoreCardHolder.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text
            = playerModel.userModel.unique_name;

        scoreCardHolder.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text
            = playerModel.points.ToString();

        var totalChips = playerModel.points * gameTableEventHandler.gameTableResponse.gameModel.point_value;

        if (gameTableEventHandler.gameTableResponse.gameModel.game_sub_type.Equals(Constants.GAME_SUB_TYPE.POOL)
            || gameTableEventHandler.gameTableResponse.gameModel.game_sub_type.Equals(Constants.GAME_SUB_TYPE.DEALS))
        {
            scoreCardHolder.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text
                = playerModel.totalPoints.ToString();
        }
        else
        {
            scoreCardHolder.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text
                = totalChips.ToString();
        }

        scoreCardHolder.transform.GetChild(scoreCardHolder.transform.childCount - 1).GetComponent<TextMeshProUGUI>()
                .text
            = playerModel.userModel.auth_token;

        ResultValidator(playerModel, i, scoreCardHolder.gameObject);

        scoreCardHolder.gameObject.SetActive(true);
        var groupParent = scoreCardHolder.transform.GetChild(2).GetChild(0);

        scoreCardHolder.transform.GetChild(2).GetChild(1).gameObject.SetActive(!playerModel.meldCardReceived);

        if (!playerModel.meldCardReceived) return;
        if (!playerModel.playerEnum.Equals(Constants.PLAYER_ENUM.DROP)
            && !playerModel.playerEnum.Equals(Constants.PLAYER_ENUM.MIDDLE_DROP) && !isDrop)
        {
            if (!authTokens.Contains(playerModel.userModel.auth_token))
            {
                if (playerModel.meldCardReceived)
                    authTokens.Add(playerModel.userModel.auth_token);
                SetCardGroup(playerModel.inHandCardList, groupParent, isDrop);
            }
        }
    }

    private void PlayWinAndLoseSound()
    {
        if (isWinOrLoosPlay) return;
        isWinOrLoosPlay = true;
        var gameModel = gameTableEventHandler.gameTableResponse.liveGameModel;
        if (gameModel == null) return;
        foreach (var playerModel in gameModel.playerModels)
        {
            if (playerModel.gameWinner &&
                playerModel.userModel.auth_token.Equals(PlayerPrefsManager.GetAuthToken()))
            {
                AudioController.instance.OnGameWin();
            }

            if (!playerModel.gameWinner &&
                playerModel.userModel.auth_token.Equals(PlayerPrefsManager.GetAuthToken()))
            {
                AudioController.instance.OnGameLose();
            }
        }
    }

    private void ResultValidator(PlayerModel playerModel, int index, GameObject scoreHolder)
    {
        if (playerModel.playerEnum.Equals(Constants.PLAYER_ENUM.DROP))
        {
            scoreHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Dropped";
        }
        else if (playerModel.playerEnum.Equals(Constants.PLAYER_ENUM.MIDDLE_DROP))
        {
            scoreHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Middle Drop";
        }
        else if (playerModel.playerEnum.Equals(Constants.PLAYER_ENUM.WINNER))
        {
            scoreHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Won";
        }
        else if (playerModel.playerEnum.Equals(Constants.PLAYER_ENUM.WRONG_SHOW))
        {
            scoreHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Wrong Show";
        }
        else
        {
            scoreHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Lost";
        }
    }

    private void SetCardGroup(List<GameTableResponse.CardModel> inHandCardList, Transform groupParent, bool isDrop)
    {
        if (isDrop) return;
        foreach (var cardModel in inHandCardList)
        {
            SetCardByGroupId(groupParent, cardModel, cardModel.groupId);
        }
    }

    private void SetCardByGroupId(Transform groupParent, GameTableResponse.CardModel cardsModel, int index)
    {
        var name = cardsModel.suit + "_" + cardsModel.rank;
        var sprite = Instantiate(groupParent.GetChild(index).GetChild(0), groupParent.GetChild(index));
        sprite.GetComponent<Image>().sprite = GamePlayManager.instance.GetSprite(name);
        sprite.gameObject.SetActive(true);
        return;
    }

    private Coroutine updateTimerCoroutine = null;

    public void UpdateTimer()
    {
        var gameModel = gameTableEventHandler.gameTableResponse.liveGameModel;
        switch (gameModel?.gameEnum)
        {
            case null:
                return;
            case Constants.GAME_ENUM.GAME_RESULT:
            {
                var time = gameModel.gameResultTime - gameModel.baseStateModel.timer.elapsedCounter;
                if (updateTimerCoroutine != null)
                {
                    StopCoroutine(updateTimerCoroutine);
                } 
                updateTimerCoroutine = StartCoroutine(ResultCounter(time));
                break;
            }
            case Constants.GAME_ENUM.GAME_DECLARE:
                nextGameInfo.text = "Please wait for other player result";
                break;
        }
    }

    public IEnumerator ResultCounter(int timer)
    {
        var counter = 0;
        isShowResult = false;
        gameObject.SetActive(true);
        while (!isShowResult)
        {
            timeAfterCloseResultPanel = timer - counter;
            nextGameInfo.text = "Next game starts in " + (timer - counter).ToString() + " Seconds";
            yield return new WaitForSeconds(1f);
            if (timer <= counter)
            {
                isShowResult = true;
                gameObject.SetActive(false);
                ResetScorecardHolder();
                StopAllCoroutines();
            }
            counter++;
        }
    }


    public void ResetCoroutine()
    {
        isShowResult = true;
        UpdateTimer();
    }

    public void ResetScorecardHolder()
    {
        for (var i = 1; i < pointGameUserCardParent.transform.childCount; i++)
        {
            Destroy(pointGameUserCardParent.transform.GetChild(i).gameObject); 
        }
        
        for (var i = 1; i < poolGameUserCardParent.transform.childCount; i++)
        {
            Destroy(poolGameUserCardParent.transform.GetChild(i).gameObject); 
        }
    }
}
