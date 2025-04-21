using System;
using MagneticScrollView;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.Networking;

public class LobbyView : BaseMonoBehaviour
{
    [Header("Panels")]
    [SerializeField] public AddCashPanelView addCashPanelView;
    [SerializeField] public AddCashPanelView addCashPanelViewV2;
    [SerializeField] public WithdrawTransactionsPanel withdrawTransactionsPanel;
    [SerializeField] public WithdrawPanelView withdrawPanelView;

    public SlidingEffect[] slidingEffect;
    public ScalingEffect[] scalingEffect;
    public GameObject[] mainGameTypes;

    public NotificationPanel notificationPanel;

    //public TextMeshProUGUI onlinePlayerText;

    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    public Image PlayerAvatar;

    public Text labelTxt;
    [SerializeField] private TextMeshProUGUI EntryAmount;

    private GameListModel selectedGameModel = null;

    public GameObject Menus, SubMenus;



    public bool isPrivateTable = false;
    public bool isPoolsGame = false;



    //SAI
    [Header("Header Buttons")]
    [SerializeField] private Button profileButton;
    [SerializeField] private Button menuBtn;
    [SerializeField] private Button notificationButton;
    [SerializeField] private Button bgMusic;
    [SerializeField] private Button refreshChipsBtn;
    [SerializeField] private Button CashDetails;
    [SerializeField] private Button WithdrawDetails;

    [Header("Game Type Buttons")]
    [SerializeField] private Button cashGameBtn;
    [SerializeField] private Button practiceGameBtn;

    [Header("Play Game Buttons")]
    [SerializeField] private Button pointRummyBtn, points;
    [SerializeField] private Button poolRummyBtn101, pool101;
    [SerializeField] private Button poolRummyBtn201, pool201;
    [SerializeField] private Button privateTableBtn, Private, poolsGameBtn, poolsGame;
    [SerializeField] private Button dealRummyBtn, Deal;

    [Header("Footer Buttons")]
    [SerializeField] private Button supportBtn;
    [SerializeField] private Button howToPlayBtn;
    [SerializeField] private Button onGoingGameBtn;
    [SerializeField] private Button exitBtn;

    [Header("Text Information")]
    [SerializeField] public TextMeshProUGUI usernameText;
    [SerializeField] public TextMeshProUGUI myText;
    [SerializeField] public Text funchipsText;
    [SerializeField] public Text CashText;
    [SerializeField] public Text WithdrawText;

    [Header("Toggles")]
    [SerializeField] public Toggle cashToggle;
    [SerializeField] public Toggle practiceToggle;

    public GameListModel gameListModel;

    private void OnEnable()
    {
        DisableAutoRotation();
        UpdateHeader();
        string latitude = LocationManager.instance.latitude;
        string longitude = LocationManager.instance.longitude;
        CashDetails.onClick.AddListener(() =>
        {
            if (Application.platform == RuntimePlatform.Android && (!Input.location.isEnabledByUser || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)))
            {
                LocationManager.instance.CheckForLocation();
            }
            else
            {
                addCashPanelView.gameObject.SetActive(true);
            }
        });

        WithdrawDetails.onClick.AddListener(() =>
        {
            UIManager.instance.accountMenuView.withdrawPanelView.gameObject.SetActive(true);
        });
    }


    public void Toggle_Changed(bool newValue)
    {
        Constants.CONFIG.is_paid = newValue;
        //cashGameBtn.gameObject.SetActive(Constants.CONFIG.is_paid);
        CashDetails.gameObject.SetActive(Constants.CONFIG.is_paid);
        if (Constants.CONFIG.is_paid)
        {
            //FunAndCashToggle(Constants.GAME_TYPE.CASH);
        }
        else
        {
            FunAndCashToggle(Constants.GAME_TYPE.PRACTICE);
        }
    }

    private void Start()
    {
        APIManager.instance.CheckVersion();
        ServerManager.instance.GetConfig();
        StartCoroutine(LobbyEntryEffect());
        profileButton.onClick.AddListener(() =>
        {
            UIManager.instance.accountMenuView.profilePanelView.gameObject.SetActive(true);
        });

        /*if (CacheMemory.GameType.Equals(Constants.GAME_TYPE.PRACTICE)
           || CacheMemory.GameType.Equals(Constants.GAME_TYPE.CASH))
        {
            var gamelist = GameListFilter.GetGameText(CacheMemory.GameType, Constants.GAME_SUB_TYPE.POINT, Constants.GAME_TYPE.CASH);
            foreach (var model in gamelist)
            {
                Debug.Log("Model point: " + gamelist);
            }
        }*/

        menuBtn.onClick.AddListener(() =>
        {
            UIManager.instance.accountMenuView.gameObject.SetActive(true);
            UIManager.instance.accountMenuView.profileViewerBtn.SetActive(CacheMemory.IsGuestUser == "0");
        });

        notificationButton.onClick.AddListener(() =>
        {
            notificationPanel.gameObject.SetActive(true);
        });
        bgMusic.onClick.AddListener(() =>
        {
            AudioController.instance.audioManager.ToggleBgm();
        });


        poolRummyBtn101.onClick.AddListener(() =>
        {
            UIManager.instance.lobbyView.isPrivateTable = false;
            UIManager.instance.poolRummyView.selectedPoolType = Constants.POOL_TYPE.POOL_101.ToString();
            PoolRummyEnable();
        });

        poolRummyBtn201.onClick.AddListener(() =>
        {


           /* UIManager.instance.lobbyView.isPrivateTable = false;
            UIManager.instance.poolRummyView.selectedPoolType = Constants.POOL_TYPE.POOL_201.ToString();
            PoolRummyEnable(); */
            ServerManager.instance.alertPopUp.ShowView("COMING SOON");
        });

        pointRummyBtn.onClick.AddListener(() =>
        {
            UIManager.instance.lobbyView.isPrivateTable = false;
            PointRummyEnable();
        });

        dealRummyBtn.onClick.AddListener(() =>
        {
            UIManager.instance.lobbyView.isPrivateTable = false;
            DealRummyEnable();
        });

        privateTableBtn.onClick.AddListener(() =>
        {
            PrivateTableEnable();
        });



        pool101.onClick.AddListener(() =>
        {
            UIManager.instance.lobbyView.isPrivateTable = false;
            UIManager.instance.poolRummyView.selectedPoolType = Constants.POOL_TYPE.POOL_101.ToString();
            PoolRummyEnable();
        });

        pool201.onClick.AddListener(() =>
        {
            UIManager.instance.lobbyView.isPrivateTable = false;
            UIManager.instance.poolRummyView.selectedPoolType = Constants.POOL_TYPE.POOL_201.ToString();
            PoolRummyEnable();
        });

        points.onClick.AddListener(() =>
        {
            UIManager.instance.lobbyView.isPrivateTable = false;
            PointRummyEnable();
        });

        Deal.onClick.AddListener(() =>
        {
            UIManager.instance.lobbyView.isPrivateTable = false;
            DealRummyEnable();
        });

        Private.onClick.AddListener(() =>
        {
            PrivateTableEnable();
        });

        poolsGame.onClick.AddListener(() =>
        {
            PoolsGameView();
        });


        refreshChipsBtn.onClick.AddListener(() =>
        {
            RefreshChips();
        });

        onGoingGameBtn.onClick.AddListener(() =>
        {
            FetchOnGoingGameList();
        });

        howToPlayBtn.onClick.AddListener(() =>
        {
            StartCoroutine(ServerManager.instance.commonWebView.WebViewPanelViewEntryEffect("How to Play", Constants.URL.HOW_TO_PLAY));
        });

        exitBtn.onClick.AddListener(() =>
        {
            AudioController.instance.Click();
            ExitGame();
        });

        supportBtn.onClick.AddListener(() =>
        {
            EnableContactUs();
        });

        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
        //UIManager.instance.toggleController.isOn = false;
        FunAndCashToggle(Constants.GAME_TYPE.CASH);

        cashToggle.onValueChanged.AddListener((IsOn) =>
        {
            if (IsOn)
            {
                cashToggle.gameObject.SetActive(Constants.CONFIG.is_paid);
                CacheMemory.GameType = Constants.GAME_TYPE.CASH;
                RefreshGameList();
                Debug.Log("CASH");
            }
        });

        practiceToggle.onValueChanged.AddListener((IsOn) =>
        {
            if (IsOn)
            {
                CacheMemory.GameType = Constants.GAME_TYPE.PRACTICE;
                RefreshGameList();
                Debug.Log("Practice");
            }
        });
    }
    public void ExitGame()
    {
        ServerManager.instance.alertPopUp.ShowView("Are you Sure, Do you want to Exit?", () =>
        {
            ServerManager.instance.loader.ShowLoader("Bye Bye see you later....");
            Application.Quit();
            Debug.Log("Exit");
        }, "Yes", () => { }, "No");
    }

    public void DisableAutoRotation()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    public void PlayStartEffects()
    {
        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].EntryEffect());
        }

        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].EntryEffect());
        }
    }

    public void PlayEndEffect()
    {
        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].ExitEffect());
        }

        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].ExitEffect());
        }
    }

    private IEnumerator LobbyEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
        AudioController.instance.StopSounds();
    }

    public IEnumerator LobbyPanelExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        UIManager.instance.loginView.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void PointRummyEnable()
    {
        UIManager.instance.pointRummyView.gameObject.SetActive(true);
    }

    public void PoolRummyEnable()
    {
        UIManager.instance.poolRummyView.gameObject.SetActive(true);
    }

    public void DealRummyEnable()
    {
        UIManager.instance.dealRummyView.gameObject.SetActive(true);
    }

    public void PrivateTableEnable()
    {
        UIManager.instance.privateTableView.gameObject.SetActive(true);
        isPrivateTable = true;
        /*if (CacheMemory.GameType == Constants.GAME_TYPE.CASH)
        {
            ServerManager.instance.alertPopUp.ShowView("Private table is not avilable for the Cash mode! Please play click on practice mode and then create a private table");
        }
        else {
            UIManager.instance.privateTableView.gameObject.SetActive(true);
            isPrivateTable = true;
        }*/
    }

    public void PoolsGameView()
    {
        UIManager.instance.poolGamesView.gameObject.SetActive(true);
        isPoolsGame = true;
    }

    public void TorunamentView()
    {
        UIManager.instance.tournamentView.gameObject.SetActive(true);
    }

    public void EnableVerificationPanel()
    {
        CheckforActivepanels();
        UIManager.instance.verificationPopUpView.gameObject.SetActive(true);
    }

    private void EnableContactUs()
    {
        AudioController.instance.OnClick();
        UIManager.instance.contactUsForm.gameObject.SetActive(true);
    }



    private void FetchOnGoingGameList()
    {
        AudioController.instance.OnClick();
        ServerManager.instance.loader.ShowLoader("Fetching...");
        ServerManager.instance.OnGoingGameListRequest((response) =>
        {
            ServerManager.instance.loader.HideLoader();
            if (response.status.Equals(Constants.KEYS.valid))
            {
                UIManager.instance.accountMenuView.onGoingGameList.ShowOnGoingGames(response);
            }
            else
            {
                ServerManager.instance.alertPopUp.ShowView(response.message);
            }
        });
    }


    public void FunAndCashToggle(string gameType)
    {
        Debug.Log("Selected Game type == " + gameType);
        if (gameType == Constants.GAME_TYPE.CASH)
        {
            CacheMemory.GameType = Constants.GAME_TYPE.CASH;
            RefreshGameList();
            practiceGameBtn.GetComponent<Image>().color = Color.black;
            cashGameBtn.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            CacheMemory.GameType = Constants.GAME_TYPE.PRACTICE;
            RefreshGameList();
            practiceGameBtn.GetComponent<Image>().color = Color.grey;
            cashGameBtn.GetComponent<Image>().color = Color.black;
        }

    }

    private void RefreshGameList()
    {
        // UIManager.instance.pointRummyView.RefreshPointGameList();
        // UIManager.instance.poolRummyView.RefreshPoolGameList();
        // UIManager.instance.dealRummyView.RefreshDealGameList();
    }



    private void UpdateHeaderData(Response<UserModel> response)
    {
        if (response.status.Equals(Constants.KEYS.valid))
        {
            CacheMemory.userModel = response.data;
            usernameText.text = response.data.unique_name;
            //myText.text = "ssssssssssssssssssssssssssssssssss";

            funchipsText.text = response.data.wallet.chips.ToString();
            var totalCash = response.data.wallet.cash_deposit;
            CashText.text = "₹" + totalCash.ToString();
            WithdrawText.text = response.data.wallet.cash_withdrawal.ToString();

            if (gameObject.activeInHierarchy)
            {
                //StartCoroutine(SetUserAvatar(PlayerAvatar, response.data.avatar_path));
                SetUserAvatarLocally(PlayerAvatar);
            }

            withdrawPanelView?.GetProfileCallback(response);

        }
        else
        {
            ServerManager.instance.alertPopUp.ShowView(response.message);
        }
    }



    public void RefreshChips()
    {
        ServerManager.instance.RefreshFunChips((response) =>
        {
            if (response.status.Equals(Constants.KEYS.valid))
            {
                ServerManager.instance.alertPopUp.ShowView(response.message);
            }
            else
            {
                ServerManager.instance.alertPopUp.ShowView(response.message);
            }
        });
    }

    public void RefreshCoins()
    {
        ServerManager.instance.RefreshGoldCoins((response) =>
        {
            if (response.status.Equals(Constants.KEYS.valid))
            {
                UpdateHeader();
                ServerManager.instance.alertPopUp.ShowView(response.message);
            }
            else
            {
                ServerManager.instance.alertPopUp.ShowView(response.message);
            }
        });
    }

    public void UpdateHeader()
    {
        SendProfileRequest(UpdateHeaderData);
    }


    public IEnumerator SetUserAvatar(Image avatarImg, string Avatarurl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(Avatarurl);
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.Log("URL = " + Avatarurl + ", Error = " + request.error);
        }
        else
        {
            DownloadHandlerTexture texture = ((DownloadHandlerTexture)request.downloadHandler);
            Sprite sprite = Sprite.Create(texture.texture, new Rect(0, 0, texture.texture.width, texture.texture.height), new Vector2(0, 0));
            avatarImg.sprite = sprite;
        }

    }


    public void SetUserAvatarLocally(Image avatarImg)
    {
        if(PlayerPrefs.HasKey(Constants.PLAYER_PREFS_CONSTANTS.AVATARIMAGE_ID))
        {
            int id = PlayerPrefs.GetInt(Constants.PLAYER_PREFS_CONSTANTS.AVATARIMAGE_ID,0);
            avatarImg.sprite = UIManager.instance.usernameupdateView.avatarSprites.Find(x => x.id == id).sprite;
        }
        else
        {
            avatarImg.sprite = UIManager.instance.usernameupdateView.avatarSprites.Find(x => x.id == 0).sprite;
        }
    }

    public void RestrictBlockPlayer(Action playGame)
    {
        if (CacheMemory.PlayerStatus.Equals(Constants.KEYS.Block) && CacheMemory.GameType.Equals(Constants.GAME_TYPE.CASH))
        {
            ServerManager.instance.alertPopUp.ShowView("Your Account Is Restricted Kindly Contact Us");
        }
        else
        {
            playGame.Invoke();
        }
    }

    //SAI
    public void CheckforActivepanels()
    {

        foreach (Transform child in Menus.GetComponentInChildren<Transform>())
        {
            if (child.gameObject.activeInHierarchy)
            {

                child.GetComponent<IActivePanel>().DeactivatePanel();
            }

        }

        foreach (Transform child in SubMenus.GetComponentInChildren<Transform>())
        {
            if (child.gameObject.activeInHierarchy)
            {
                child.GetComponent<BaseMonoBehaviour>().DeactivateSubGameView();
            }

        }
    }

    private void PlayGame()
    {
        CashDetails.interactable = false;
        if (!UIManager.instance.lobbyView.isPrivateTable)
        {
            JoinTable(selectedGameModel, null, null);
            GamePlayManager.instance.gameTableEventHandler.ResetTable();
            GamePlayManager.instance.gameTableEventHandler.ResetAllBeforeFocus();
            GamePlayManager.instance.gameTableEventHandler.gameResult.gameObject.SetActive(false);
            //  GamePlayManager.instance.gameTableEventHandler.meldCards.gameObject.SetActive(false);
            UIManager.instance.lobbyView.gameObject.SetActive(true);
        }

        StartCoroutine(EnabledrDisabledPlayButton());
    }

    private IEnumerator EnabledrDisabledPlayButton()
    {
        yield return new WaitForSeconds(0.2f);
        CashDetails.interactable = true;
    }

}
