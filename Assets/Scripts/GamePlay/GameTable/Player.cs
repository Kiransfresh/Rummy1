using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : TableBaseMono
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerPoint;
    [SerializeField] private TextMeshProUGUI playerAmount;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI practiceOrCashGameText;

    [SerializeField] private Image playerAvatar;
    [SerializeField] private Image playerInitialTurnTime;
    [SerializeField] private Image playerExtraTurnTime;
    [SerializeField] private TextMeshProUGUI timerText;

    public string authToken;
    public Button DiscardHistoryBtn;
    public GameObject DiscardHistroryGrid, DicardImagePrefab;
    [HideInInspector]
    public bool DiscardHistoryStatus;
    private float time;
    private Image timerImage;
    private bool isTimerStart = false;
    private float maxFillAmount = 1f;
    private PlayerModel CurrentPlayerModel;
    private PlayerHolder playerholder;
    public TextMeshProUGUI statusText;
    private bool IsPointerBtn;


    public GameObject chipsIcon;
    public GameObject moneyIcon;

    public GameObject crownIcon;

    private bool PlayerCounterNeedToRun = true;

    private void OnEnable()
    {
        if (CacheMemory.GameType.Equals(Constants.GAME_TYPE.CASH))
        {
            chipsIcon.SetActive(false);
            moneyIcon.SetActive(true);
        }
        else
        {
            moneyIcon.SetActive(false);
            chipsIcon.SetActive(true);
        }
    }

    private void OnDisable()
    {
        statusText.text = "";
        statusText.transform.parent.parent.gameObject.SetActive(false);
    }


    private void Start()
    {
        ResetTime();
        DiscardHistoryBtn.onClick.AddListener(ShowPlayerDicardsHistory);
        DiscardHistroryGrid.transform.parent.gameObject.SetActive(false);
        playerholder = GetComponentInParent<PlayerHolder>();
        //   CurrentPlayerModel =gameTableEventHandler.gameTableResponse.liveGameModel.playerModels;

        //playerInitialTurnTime.gameObject.SetActive(!PlayerCounterNeedToRun);
       // playerExtraTurnTime.gameObject.SetActive(!PlayerCounterNeedToRun);
        timeText.gameObject.SetActive(PlayerCounterNeedToRun);
    }

    private int nextUpdate = 1;
    private void Update()
    {
        PlayTimer();
    }

    public void SetPlayerAvatar()
    {
        var player = gameTableEventHandler.gameTableResponse.GetPlayerFromChair(authToken);
        //StartCoroutine(APIManager.instance.SetImages(playerAvatar, player.userModel.avatar_path));
    }


    public void SetPlayerInfo(PlayerModel playerModel)
    {
        playerInitialTurnTime.enabled = false;
        playerExtraTurnTime.enabled = false;
        CurrentPlayerModel = playerModel;
        playerName.text = playerModel.userModel.unique_name;

        if (playerName.text.Length > 7)
        {
            playerName.text = playerName.text.Substring(0, 6) + "..";
        }
        if (gameTableEventHandler.gameTableResponse.gameModel.game_sub_type == Constants.GAME_SUB_TYPE.POINT)
        {
            if (playerAmount != null)
            {
                if (CacheMemory.GameType.Equals(Constants.GAME_TYPE.CASH))
                {
                    playerAmount.text = "CASH";
                }
                else
                {
                    playerAmount.text = "PRACTICE";
                }
            }
            if (authToken.Equals(PlayerPrefsManager.GetAuthToken()))
            {
                playerPoint.text = "" + (GamePlayManager.instance.totalInvalidCount >= 80 ? 80 : GamePlayManager.instance.totalInvalidCount);
            }
            else
            {
                //playerPoint.text = playerModel.displayChips.ToString();
            }
        }
        else
        {
            if (authToken.Equals(PlayerPrefsManager.GetAuthToken()))
            {
                playerPoint.text = "" + (GamePlayManager.instance.totalInvalidCount >= 80 ? 80 : GamePlayManager.instance.totalInvalidCount);
                if (playerAmount != null)
                {
                    playerAmount.text = "PTS : " + playerModel.displayChips.ToString();
                }
            }
            else
            {
                playerPoint.text = playerModel.displayChips.ToString();
            }
        }
        authToken = playerModel.userModel.auth_token;
        switch (playerModel.turnEnum)
        {
            case Constants.TURN_ENUM.PLAYER_TURN_INITIAL_TIMER:
                if (authToken.Equals(PlayerPrefsManager.GetAuthToken()))
                {
                    AudioController.instance.PlayerTurn();
                }
                if (playerModel.autoPlay)
                {
                    time = gameTableEventHandler.gameTableResponse.liveGameModel.playerAutoPlayTime;
                }
                else
                {
                    time = gameTableEventHandler.gameTableResponse.liveGameModel.playerInitialTurnTime;
                }

                timerImage = playerInitialTurnTime;
                playerInitialTurnTime.enabled = true;
                playerExtraTurnTime.enabled = false;
                ValidateElapsedTimer();
                gameTableEventHandler.isDropOnTimeOut = false;
                isTimerStart = true;
                break;
            case Constants.TURN_ENUM.PLAYER_TURN_EXTRA_TIME:
                time = gameTableEventHandler.gameTableResponse.liveGameModel.playerExtraTurnTime;
                timerImage = playerExtraTurnTime;
                playerExtraTurnTime.enabled = true;
                playerInitialTurnTime.enabled = false;
                if (authToken.Equals(PlayerPrefsManager.GetAuthToken()) && gameTableEventHandler.CurrentEvent != Constants.GAME_TABLE_EVENT.GAME_END && !gameTableEventHandler.gameResult.gameObject.activeInHierarchy)
                {
                    AudioController.instance.StartTimer();
                }
                ValidateElapsedTimer();
                isTimerStart = true;
                break;
            case Constants.TURN_ENUM.PLAYER_DECLARE_TIMER:
                time = gameTableEventHandler.gameTableResponse.liveGameModel.gameDeclareTime;
                timerImage = playerInitialTurnTime;
                playerInitialTurnTime.enabled = true;
                playerExtraTurnTime.enabled = false;
                ValidateElapsedTimer();
                isTimerStart = true;
                break;
            default:
                isTimerStart = false;
                break;
        }

        if (authToken.Equals(PlayerPrefsManager.GetAuthToken()))
        {
            UIManager.instance.lobbyView.SetUserAvatarLocally(playerAvatar);
        }
        else
        {
            playerAvatar.sprite = UIManager.instance.usernameupdateView.avatarSprites[0].sprite;
        }
        crownIcon.SetActive(false);
        if (playerModel.dealer) crownIcon.SetActive(true);
        Debug.Log("Dealer -------- "+ playerModel.dealer);
    }

    void ShowPlayerDicardsHistory()
    {
        PlayerModel playerModel = gameTableEventHandler.gameTableResponse.GetPlayer(authToken: authToken);
        if (playerModel != null && playerModel.discardedCardList.Count > 0)
        {
            DiscardHistroryGrid.transform.parent.gameObject.SetActive(!DiscardHistroryGrid.gameObject.activeInHierarchy);
            DiscardHistoryStatus = DiscardHistroryGrid.gameObject.activeInHierarchy;
            if (DiscardHistoryStatus)
                playerholder.SinglePlayerDiscardHistory(this);

            int count = DiscardHistroryGrid.transform.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                var Discard = DiscardHistroryGrid.transform.GetChild(i).gameObject;
                Discard.transform.SetParent(null);
                Destroy(Discard);
            }
            for (int i = 0; i < playerModel.discardedCardList.Count; i++)
            {
                var card = Instantiate(DicardImagePrefab, DiscardHistroryGrid.transform);
                string cardname = playerModel.discardedCardList[i].suit + "_" + playerModel.discardedCardList[i].rank;
                card.GetComponent<Image>().sprite = GamePlayManager.instance.GetSprite(cardname);
            }
        }
    }

    public void SetPlayerStatus()
    {
        var player = gameTableEventHandler.gameTableResponse.GetPlayerFromChair(authToken);
        if (player == null) return;
        Debug.Log("Player Status - " + player.playerEnum.ToString());
        //StartCoroutine(APIManager.instance.SetImages(playerAvatar, player.userModel.avatar_path));
        if (player.disconnected)
        {
            statusText.text = "Disconnected";
            statusText.transform.parent.parent.gameObject.SetActive(true);
        }
        else if (player.leave)
        {
            statusText.text = "Leave";
            statusText.transform.parent.parent.gameObject.SetActive(true);
        }
        else if (player.playerEnum.Equals(Constants.PLAYER_ENUM.DROP))
        {
            statusText.text = "Dropped";
            statusText.transform.parent.parent.gameObject.SetActive(true);
        }
        else if (player.playerEnum.Equals(Constants.PLAYER_ENUM.MIDDLE_DROP))
        {
            statusText.text = "Middle Dropped";
            statusText.transform.parent.parent.gameObject.SetActive(true);
        }
        else if (player.playerEnum.Equals(Constants.PLAYER_ENUM.WRONG_SHOW))
        {
            statusText.text = "Wrong Show";
            statusText.transform.parent.parent.gameObject.SetActive(true);
        }
        else if (gameTableEventHandler.gameTableResponse.tableEnum.Equals(Constants.TABLE_ENUM.WAITING_FOR_PLAYER) &&
                gameObject.activeInHierarchy)
        {
            if (UIManager.instance.lobbyView.isPrivateTable == true)
            {
                UIManager.instance.sharePrivateTableCode.gameObject.SetActive(PlayerPrefsManager.GetAuthToken().Equals(CacheMemory.privateTableHostAuthToken));
                GamePlayManager.instance.gameTableEventHandler.buttonStats.SharePrivateTableButtonStats(PlayerPrefsManager.GetAuthToken().Equals(CacheMemory.privateTableHostAuthToken));
            }
            gameTableEventHandler.messageInfo.ShowWithMessage(Constants.MESSAGE.WAITING_FOR_PLAYER);
            statusText.text = "Waiting";
            statusText.transform.parent.parent.gameObject.SetActive(true);
        }
        else if (player.autoPlay)
        {
            statusText.text = "Auto Play";
            statusText.transform.parent.parent.gameObject.SetActive(true);
        }
        else
        {
            statusText.text = "";
            statusText.transform.parent.parent.gameObject.SetActive(false);
        }
    }


    public void ResetDiscardedCards()
    {
        DiscardHistroryGrid.transform.parent.gameObject.SetActive(false);
        DiscardHistoryStatus = false;
    }

    private void ValidateElapsedTimer()
    {
        float elapsedTime = gameTableEventHandler.gameTableResponse.liveGameModel.baseStateModel.timer
            .elapsedCounter;
        timerImage.fillAmount = maxFillAmount - (elapsedTime / time);
    }

    public void PlayTimer()
    {
        if (gameTableEventHandler.CurrentEvent.Equals(Constants.GAME_TABLE_EVENT.GAME_PLAY_RESULT))
        {
            isTimerStart = false;
            if (timerImage != null)
                timerImage.fillAmount = maxFillAmount;
        }
        if (!isTimerStart) return;
        timerImage.fillAmount -= (maxFillAmount / time) * Time.deltaTime;
        UpdateTimerText();
        if (timerImage.fillAmount >= 0) return;
        isTimerStart = false;
        AudioController.instance.StopTimer();
    }
  
    public void UpdateTimerText()
    {
        // Calculate remaining time in seconds
        float remainingTime = timerImage.fillAmount * time;

        // Update the timer text with the remaining time
        timerText.text = Mathf.CeilToInt(remainingTime).ToString();
    }

    public void ResetTime()
    {
        playerInitialTurnTime.enabled = false;
        playerExtraTurnTime.enabled = false;
        timeText.text = "";
        isTimerStart = false;
        if (timerImage != null)
            timerImage.fillAmount = maxFillAmount;
        playerInitialTurnTime.fillAmount = maxFillAmount;
        playerExtraTurnTime.fillAmount = maxFillAmount;
    }

    public void SetPlayerScoreForPointGame()
    {
        if (authToken.Equals(PlayerPrefsManager.GetAuthToken()))
        {
            playerPoint.text = "" + (GamePlayManager.instance.totalInvalidCount >= 80 ? 80 : GamePlayManager.instance.totalInvalidCount);
            if (gameTableEventHandler.gameTableResponse.gameModel.game_sub_type == Constants.GAME_SUB_TYPE.POINT)
            {
                if (practiceOrCashGameText != null)
                {
                    if (CacheMemory.GameType.Equals(Constants.GAME_TYPE.CASH))
                    {
                        practiceOrCashGameText.text = "CASH";
                    }
                    else
                    {
                        practiceOrCashGameText.text = "PRACTICE";
                    }
                }
            }
        }
    }
}