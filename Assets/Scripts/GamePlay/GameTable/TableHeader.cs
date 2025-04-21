using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TableHeader : TableBaseMono
{
    public TextMeshProUGUI tableId;
    public TextMeshProUGUI gameType;
    [SerializeField]
    public TextMeshProUGUI walletAmount;
    [SerializeField]
    private TextMeshProUGUI walletAmountV2;
    public TextMeshProUGUI prize;

    public Button lastDealBtn;
    public Button settingBtn;
    public Button leaveTableBtn;

    public GameSettingMenu gameSettingMenu;
    [SerializeField] private GameScoreboard gameScoreboard;

    public GameObject chipsImage;
    public GameObject rupeesImage;
    public GameObject prizeMoney;

    [SerializeField]
    private TextMeshProUGUI entryFeesText;

    [SerializeField]
    private Button addCashButton;

    private void OnEnable()
    {
        if (UIManager.instance.gameRoom.activeInHierarchy)
            UIManager.instance.lobbyView.gameObject.SetActive(false);
    }

    private void Start()
    {
        lastDealBtn.onClick.AddListener(() =>
        {
            LastDealDetails();
        });

        settingBtn.onClick.AddListener(() =>
        {
            gameSettingMenu.gameObject.SetActive(!gameSettingMenu.gameObject.activeInHierarchy);
        });

        leaveTableBtn.onClick.AddListener(() =>
        {
            gameSettingMenu.gameObject.SetActive(false);

            string message = Constants.MESSAGE.LEAVE_TABLE_CONFIRMATION_PRACTICE;
            if (gameTableEventHandler.gameTableResponse.gameModel.game_type == Constants.GAME_TYPE.CASH)
            {
                message = Constants.MESSAGE.LEAVE_TABLE_CONFIRMATION;
            }

            ServerManager.instance.alertPopUp.ShowView(message,
                () =>
                {
#if UNITY_WEBGL
                    Application.ExternalEval("Quit()");
#else
                    gameTableEventHandler.LeaveTable();
                    AudioController.instance.StopSounds();
#endif
                }, "Yes", () => { }, "No");
        });
    }

    public void LastDealDetails()
    {
        gameSettingMenu.gameObject.SetActive(false);
        LastRoundResult();
        gameTableEventHandler.gameResult.lastDealandScorecardToggleGroup.transform.GetChild(0).GetComponent<Toggle>()
            .onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    LastRoundResult();
                }
            });

        gameTableEventHandler.gameResult.lastDealandScorecardToggleGroup.transform.GetChild(1).GetComponent<Toggle>()
            .onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    ShowScoreboard();
                }
            });
    }

    private void ShowScoreboard()
    {
        ServerManager.instance.SCORE_BOARD(gameTableEventHandler.gameTableResponse.id, (response) =>
        {
            if (response.status.Equals(Constants.KEYS.valid))
            {
                gameScoreboard.gameObject.SetActive(true);
                gameScoreboard.SetScoreboard(response.data);
            }
            else
            {
                ServerManager.instance.alertPopUp.ShowView("Scoreboard data not available");
            }
        });
    }

    private void LastRoundResult()
    {
        ServerManager.instance.LAST_ROUND_RESULT(gameTableEventHandler.gameTableResponse.id, (response) =>
        {
            if (response.status.Equals(Constants.KEYS.valid))
            {
                gameTableEventHandler.gameResult.lastDealandScorecardToggleGroup.gameObject.SetActive(true);
                gameTableEventHandler.gameResult.lastDealandScorecardToggleGroup.transform.GetChild(0)
                    .GetComponent<Toggle>().isOn = true;
                gameTableEventHandler.gameResult.ShowResult(response.data, true);
            }
            else
            {
                ServerManager.instance.alertPopUp.ShowView("Last deal data not available");
            }
        });
    }

    public void SetTableHeader()
    {
        var tableModel = gameTableEventHandler.gameTableResponse;
        tableId.text = tableModel.id + "-" + tableModel.round;

        if (tableModel.gameModel.game_type.Equals(Constants.GAME_TYPE.PRACTICE))
        {
            rupeesImage.SetActive(false);
            chipsImage.SetActive(true);
        }
        else
        {
            chipsImage.SetActive(false);
            rupeesImage.SetActive(true);
        }

        prizeMoney.gameObject.SetActive(!tableModel.gameModel.game_sub_type.Equals(Constants.GAME_SUB_TYPE.POINT));

        if (tableId.text.Length > 20)
        {
            tableId.text = "#" + tableId.text.Substring(0, 2) + ".."
                           + tableId.text.Substring(tableId.text.Length - 6);
        }
        else
        {
            tableId.text = "#" + tableId.text;
        }

        if (tableModel.gameModel.game_sub_type.Equals(Constants.GAME_SUB_TYPE.POOL))
        {

            gameType.text = tableModel.gameModel.game_sub_type + "-" +
                            tableModel.gameModel.pool_game_type + "(" + CacheMemory.NumberOfPlayers + ")";
        }
        else
        {
            gameType.text = tableModel.gameModel.game_sub_type + "(" + CacheMemory.NumberOfPlayers + ")";
        }

        var playerModel = tableModel.GetPlayer(PlayerPrefsManager.GetAuthToken());
        if (playerModel != null)
        {
            walletAmount.text = tableModel.gameModel.game_type
                .Equals(Constants.GAME_TYPE.CASH) ?
                playerModel.userModel.wallet.cash.ToString("0.00")
                : playerModel.userModel.wallet.chips.ToString("0");

            walletAmountV2.text = tableModel.gameModel.game_type
                .Equals(Constants.GAME_TYPE.CASH) ?
                playerModel.userModel.wallet.cash.ToString("0.00")
                : playerModel.userModel.wallet.chips.ToString("0");

        }
        else
        {
            walletAmount.text = "Waiting...";
        }

        if (tableModel.liveGameModel?.gameDetail != null)
        {
            prize.text = tableModel.liveGameModel.gameDetail.prize_after_deduction
                .ToString(tableModel.gameModel.game_type
                    .Equals(Constants.GAME_TYPE.CASH) ? "0.00" : "0");
        }
        else
        {
            prize.text = "Waiting...";
        }

        if (tableModel.liveGameModel?.gameDetail != null )
        {
            entryFeesText.text = float.Parse(tableModel.liveGameModel.gameDetail.entry_fee).ToString("0.00");
        }
        else {
            entryFeesText.text = "Waiting...";
        }
        if(CacheMemory.userModel != null)
        {
            walletAmountV2.text = CacheMemory.userModel.wallet.cash.ToString();
        }
        
    }
}
