using System;
using System.Collections;
using MagneticScrollView;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : BaseMonoBehaviour
{
    public static UIManager instance;
    
    public SplashView splashView;
    public LoginView loginView;
    public ProfilePanelView profilePanelView;
    public RegisterView registerView;
    public VerificationPopUpview verificationPopUpView;
    public ForgotPasswordView forgotPasswordView;
    public LobbyView lobbyView;
    public PointRummyView pointRummyView;
    public PoolRummyView poolRummyView;
    public DealRummyView dealRummyView;
    public TournamentView tournamentView;
    public PrivateTableView privateTableView;
    public PoolGamesView poolGamesView;
    public ContactUsForm contactUsForm;


    public AccountMenuView accountMenuView;
    public GameObject gameRoom;
    public UpdateUsernamePopUp usernameupdateView;
    public GameObject UpdragePanelPrefab;

    public FortuneWheel fortuneWheelPanel;

    public SharePrivateTableCode sharePrivateTableCode;



    private int splashShowCount;
    private List<GameObject> publicVariableList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
       
    }

    private void OnEnable()
    {
        UiChangesOnKickEvent();
        SequenceTest();
    }

    private void OnApplicationPause()
    {
        PlayerPrefs.DeleteKey(Constants.KEYS.is_splash_shown);
        PlayerPrefs.Save();
    }

    private void UiChangesOnKickEvent()
    {
        splashShowCount = PlayerPrefs.GetInt(Constants.KEYS.is_splash_shown);
        if (splashShowCount < 1)
        {
            splashView.gameObject.SetActive(true);
            splashShowCount++;
            PlayerPrefs.SetInt(Constants.KEYS.is_splash_shown, splashShowCount);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Login view activated");
            loginView.gameObject.SetActive(true);
        }
    }

    public void ClearEntryFeeList(List<TextMeshProUGUI> entryFeeList)
    {
        for (var i = 0; i < entryFeeList.Count; i++)
        {
            entryFeeList[i].gameObject.SetActive(false);
            Destroy(entryFeeList[i].gameObject);
        }
        entryFeeList.Clear();
    }

    public void MakeCurrentIndexActive(List<TextMeshProUGUI> List, MagneticScrollRect Scroller)
    {
        if (List.Count > 0)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (i == Scroller.CurrentSelectedIndex)
                {
                    List[i].color = new Color32(255, 255, 255, 255);
                }
                else
                {
                    List[i].color = new Color32(255, 255, 255, 93);
                }
            }
        }
    }

    public void LogoutView()
    {
        StartCoroutine(LogoutEffect());
       
    }

    public void FetchGameList()
    {
        ServerManager.instance.GameListRequest((response) =>
        {
            if (response.status.Equals(Constants.KEYS.valid))
            {
                CacheMemory.GameList = response.data;
                if (gameRoom.gameObject.activeInHierarchy)
                {
                    JoinTable(null, CacheMemory.RunningTableId, null);
                }
            }
        });
    }

    public void GetCoinValue() 
    {
        ServerManager.instance.GoldCoinValueRequestRequest((response) =>
        {
            if (response.status.Equals(Constants.KEYS.valid)) 
            {
                CacheMemory.coinprice = response.data.COIN_PRICE;
            }
        });
    }

    public void FetchTournamentList()
    {
        ServerManager.instance.TournamentListRequest((response) =>
        {
            if (response.status.Equals(Constants.KEYS.valid))
            {
                CacheMemory.TournamentList = response.data;
            }
        });
    }

    public IEnumerator RefreshEntryFee(List<TextMeshProUGUI> List, float timeDelay)
    {
        foreach (var fee in List)
        {
            fee.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(timeDelay);
        foreach (var fee in List)
        {
            fee.gameObject.SetActive(true);
        }
    }

    private IEnumerator LogoutEffect()
    {
        accountMenuView.PlayEndEffect();
        yield return new WaitForSeconds(1f);
        accountMenuView.gameObject.SetActive(false);
        lobbyView.PlayEndEffect();
        yield return new WaitForSeconds(0.3f);
        lobbyView.gameObject.SetActive(false);
        loginView.gameObject.SetActive(true);
        ServerManager.instance.loader.HideLoader();
    }

    public IEnumerator EnableLobby()
    {
        ServerManager.instance.loader.ShowLoader(Constants.MESSAGE.CONNECTING);
        if (instance.verificationPopUpView.gameObject.activeInHierarchy)
        {
            verificationPopUpView.PlayEndEffect();
        }
        ServerManager.instance.Initialise();
        Debug.Log("Waiting time");
        yield return new WaitForSeconds(1f);
        
        if (instance.verificationPopUpView.gameObject.activeInHierarchy)
        {
            verificationPopUpView.gameObject.SetActive(false);
        }

        if (loginView.gameObject.activeInHierarchy)
        {
            loginView.PlayEndEffect();
        }

        if (registerView.gameObject.activeInHierarchy)
        {
            registerView.PlayEndEffect();
        }
        if (lobbyView.gameObject.activeInHierarchy)
        {
            lobbyView.PlayStartEffects();
        }

        Debug.Log("Waiting for time");
        yield return new WaitForSeconds(0.3f);
        if (registerView.gameObject.activeInHierarchy)
        {
            registerView.gameObject.SetActive(false);
        }

        if (loginView.gameObject.activeInHierarchy)
        {
            loginView.gameObject.SetActive(false);
        }

    }

    public void EmailVerificationProfilePage()
    {
       profilePanelView.gameObject.SetActive(true);
        profilePanelView.Emailnotverified();
    }

    public void ShowUpgradePanel() {

        var Upgradepanel = Instantiate(UpdragePanelPrefab, lobbyView.gameObject.transform);
    }

    

    public void RefreshAllGameList()
    {
        //poolRummyView.RefreshPoolGameList();
       // pointRummyView.RefreshPointGameList();
       // dealRummyView.RefreshDealGameList();
    }




    private void SequenceTest()
    {

        List<Card> deckCards = new List<Card>();
        {
            Card model = new Card();
            model.cardModel = new GameTableResponse.CardModel();
            model.cardModel.id = 1;
            model.cardModel.suit = Constants.CARD_SUIT.CLUB;
            model.cardModel.rank = 1;
            deckCards.Add(model);

        }
        {
            Card model = new Card();
            model.cardModel = new GameTableResponse.CardModel();
            model.cardModel.id = 2;
            model.cardModel.suit = Constants.CARD_SUIT.DIAMOND;
            model.cardModel.rank = 1;
            deckCards.Add(model);
        }

        {
            Card model = new Card();
            model.cardModel = new GameTableResponse.CardModel();
            model.cardModel.id = 3;
            model.cardModel.suit = Constants.CARD_SUIT.HEART;
            model.cardModel.rank = 7;
            model.cardModel.cutJoker = true;
            deckCards.Add(model);
        }


        {
            Card model = new Card();
            model.cardModel = new GameTableResponse.CardModel();
            model.cardModel.id = 4;
            model.cardModel.suit = Constants.CARD_SUIT.CLUB;
            model.cardModel.rank = 2;
            deckCards.Add(model);
        }

        {
            Card model = new Card();
            model.cardModel = new GameTableResponse.CardModel();
            model.cardModel.id = 5;
            model.cardModel.suit = Constants.CARD_SUIT.HEART;
            model.cardModel.rank = 2;
            deckCards.Add(model);
        }

        {
            Card model = new Card();
            model.cardModel = new GameTableResponse.CardModel();
            model.cardModel.id = 6;
            model.cardModel.suit = Constants.CARD_SUIT.DIAMOND;
            model.cardModel.rank = 4;
            deckCards.Add(model);
        }

        {
            Card model = new Card();
            model.cardModel = new GameTableResponse.CardModel();
            model.cardModel.id = 7;
            model.cardModel.suit = Constants.CARD_SUIT.HEART;
            model.cardModel.rank = 7;
            model.cardModel.cutJoker = true;
            deckCards.Add(model);
        }

        {
            Card model = new Card();
            model.cardModel = new GameTableResponse.CardModel();
            model.cardModel.suit = Constants.CARD_SUIT.SPADE;
            model.cardModel.id = 8;
            model.cardModel.rank = 4;
            deckCards.Add(model);
        }

        {
            Card model = new Card();
            model.cardModel = new GameTableResponse.CardModel();
            model.cardModel.suit = Constants.CARD_SUIT.CLUB;
            model.cardModel.id = 9;
            model.cardModel.rank = 7;
            deckCards.Add(model);
        }

        {
            Card model = new Card();
            model.cardModel = new GameTableResponse.CardModel();
            model.cardModel.suit = Constants.CARD_SUIT.SPADE;
            model.cardModel.id = 10;
            model.cardModel.rank = 3;
            deckCards.Add(model);
        }

        {
            Card model = new Card();
            model.cardModel = new GameTableResponse.CardModel();
            model.cardModel.suit = Constants.CARD_SUIT.CLUB;
            model.cardModel.id = 11;
            model.cardModel.rank = 8;
            deckCards.Add(model);
        }

        {
            Card model = new Card();
            model.cardModel = new GameTableResponse.CardModel();
            model.cardModel.suit = Constants.CARD_SUIT.SPADE;
            model.cardModel.id = 12;
            model.cardModel.rank = 6;
            deckCards.Add(model);
        }

        {
            Card model = new Card();
            model.cardModel = new GameTableResponse.CardModel();
            model.cardModel.suit = Constants.CARD_SUIT.SPADE;
            model.cardModel.id = 13;
            model.cardModel.rank = 5;
            deckCards.Add(model);
        }


        string cards = "";
        foreach (var item in deckCards)
        {
            cards += "{" + item.cardModel.suit + "," + item.cardModel.rank + "}"; 
        }
        Debug.Log(cards);

        //List<Card> cardList = CardValidator.getCardsForSystem(deckCards);

        //int groupid = 0;
        //string val = "";
        //foreach (Card temp in cardList)
        //{
        //    if(temp.cardModel.groupId < groupid)
        //    {
        //        val += "GROUP = [" + temp.cardModel.groupId + "] " + "(" + temp.cardModel.suit + ")" + "(" + temp.cardModel.rank + ")" + "(" + temp.cardModel.IsJoker() + ")"  + ",";
        //    }
        //    else
        //    {
        //        Debug.Log(val);
        //        groupid++;
        //        val = "";
        //    }
        //   // Debug.Log("GROUP = ["  + temp.cardModel.groupId + "] " + temp.cardModel.suit + temp.cardModel.rank + temp.cardModel.IsJoker());
        //}
    }
}
