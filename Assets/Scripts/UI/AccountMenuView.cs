using Sfs2X.Requests;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.Android;

public class AccountMenuView : MonoBehaviour
{
    public SlidingEffect[] slidingEffect;
    
    public GameObject aboutUsSubMenuList;
    public GameObject transactionsSubMenuList;
    public GameObject kycSubMenuList;
    //public GameObject accountMenuList;

    [Header("Buttons")]
    public Button depositTransactionsBtn;
    public Button withdrawTransactionsBtn;
    public Button accountPanelBtn;
    public Button withDrawalHistoryPanelBtn;
    public Button KYCPanelBtn;
    public Button bankAccountDetailsBtn;
    public Button bonusTransactionsBtn;
    public Button gameHistoryBtn;
    public Button hostedGameDetailsBtn;
    public Button inquiryBtn;
    public Button accountMenuBtn;
    public Button bankAccountBtn;
    public Button transactionBtn;
    public Button withDrawBtn;
    public Button promotionWebPageBtn;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI versionCode; 

    public DepositTransactionsPanel depositTransactionsPanel;
    public WithdrawTransactionsPanel withdrawTransactionsPanel;
    public BonusTransactionsPanel bonusTransactionsPanel;
    public GameHistoryPanel gameHistoryPanel;
    public HostedPrivateGamesPanel hostedPrivateGamesPanel;
    public InquiryFormView inquiryFormView;
    public AccountPanelView accountPanelView;
    public BankStatementRequestPanelView bankStatementRequestPanelView;
    public ProfilePanelView profilePanelView;
    public KYCPanelView kycPanelView;
    public WithdrawPanelView withdrawPanelView;
    public ChangePasswordPanelView changePasswordPanelView;
    public ReadOnlyPanel readOnlyPanel;
    public ContactUsForm contactUsForm;

    public ReferFriendPanel referFriendPanel;


    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    public GameObject profileViewerBtn;
    //  [SerializeField]private Button onGoingGameListBtn;
    [SerializeField] private Button howToPlayBtn;
    [SerializeField] private Button contactUsBtn;
    public OnGoingGameList onGoingGameList;
    public LobbyView lobbyview;
    private GameListModel selectedGameModel = null;
    [HideInInspector]
    GameObject ActivePanel;
    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        //accountMenuBtn.gameObject.SetActive(Constants.CONFIG.is_paid);
        //bankAccountBtn.gameObject.SetActive(Constants.CONFIG.is_paid);
        //transactionBtn.gameObject.SetActive(Constants.CONFIG.is_paid);
        //kycBtn.gameObject.SetActive(Constants.CONFIG.is_paid);
        ///withDrawBtn.gameObject.SetActive(Constants.CONFIG.is_paid);
        StartCoroutine(AccountMenuListEntryEffect());
    }

    private void Start()
    {
        howToPlayBtn.onClick.AddListener(HowToPlayPanel);
        contactUsBtn.onClick.AddListener(EnableContactUs);
        depositTransactionsBtn.onClick.AddListener(EnableDepositTransactions);
        withdrawTransactionsBtn.onClick.AddListener(EnableWithDrawTransactions);
        accountPanelBtn.onClick.AddListener(EnableAccountDetails);
        withDrawalHistoryPanelBtn.onClick.AddListener(EnableWithdrawalHistoryDetails);
        //KYCPanelBtn.onClick.AddListener(EnableKYCDetails);
        bankAccountDetailsBtn.onClick.AddListener(EnableBankDetails);
        bonusTransactionsBtn.onClick.AddListener(EnableBonusTransactions);
        gameHistoryBtn.onClick.AddListener(EnableGameHistory);
        hostedGameDetailsBtn.onClick.AddListener(EnableHostedGamesPanel);
        promotionWebPageBtn.onClick.AddListener(OnClickPromotion);
        string latitude = LocationManager.instance.latitude;
        string longitude = LocationManager.instance.longitude;
        KYCPanelBtn.onClick.AddListener(() => {
            EnableKYCDetails();
        });
        withdrawTransactionsBtn.onClick.AddListener(() => {
            EnableWithdrawPanelView();
        });
        versionCode.text = "Current Version: " + Application.version;
    }

    public void OnClickPromotion()
    {
        var url = Constants.SERVER_DETAILS.Mobile_Url + "promotions";
        Application.OpenURL(url);
    }

    public void PlayStartEffects()
    {
        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].EntryEffect());
        }
    }

    public void PlayEndEffect()
    {
        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].ExitEffect());
        }
    }

    private IEnumerator AccountMenuListEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator AccountMenuListExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);

    }


    public void DisableAccountMenuList()
    {
        StartCoroutine(AccountMenuListExitEffect());
    }

    public void EnableAboutUsList()
    {
        aboutUsSubMenuList.SetActive(!aboutUsSubMenuList.activeInHierarchy);
    }

    public void EnableAccountPanelView()
    {
        lobbyview.CheckforActivepanels();
        StartCoroutine(AccountMenuListExitEffect());
        accountPanelView.gameObject.SetActive(true);

    }

    public void EnableInquiryForPanelView()
    {
        lobbyview.CheckforActivepanels();
        StartCoroutine(AccountMenuListExitEffect());
        inquiryFormView.gameObject.SetActive(true);
    }

    public void EnableBankStatementPanelView()
    {
        lobbyview.CheckforActivepanels();
        StartCoroutine(AccountMenuListExitEffect());
        bankStatementRequestPanelView.gameObject.SetActive(true);
    }

    public void EnableProfilePanelView()
    {
        lobbyview.CheckforActivepanels();
        StartCoroutine(AccountMenuListExitEffect());
        profilePanelView.gameObject.SetActive(true);
    }

    public void EnableWithdrawPanelView()
    {
        if (Application.platform == RuntimePlatform.Android && (!Input.location.isEnabledByUser || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)))
        {
            LocationManager.instance.CheckForLocation();
        }
        else 
        {
            lobbyview.CheckforActivepanels();
            StartCoroutine(AccountMenuListExitEffect());
            withdrawPanelView.gameObject.SetActive(true);
        }
    }

    public void EnableChangePasswordPanelView()
    {
        lobbyview.CheckforActivepanels();
        StartCoroutine(AccountMenuListExitEffect());
        changePasswordPanelView.gameObject.SetActive(true);
    }

    public void EnableTermsConditionsPanelView()
    {
        StartCoroutine(ServerManager.instance.commonWebView.WebViewPanelViewEntryEffect
            ("Terms and Conditions", Constants.URL.TERMS_AND_CONDITIONS));
        /*StartCoroutine(AccountMenuListExitEffect());
        readOnlyPanel.gameObject.SetActive(true);
        readOnlyPanel.SetTermsConditionsText();*/

    }

    public void EnableLegalitiesPanelView()
    {
        StartCoroutine(ServerManager.instance.commonWebView.WebViewPanelViewEntryEffect
            ("Legalities", Constants.URL.LEGALITY));
        /*StartCoroutine(AccountMenuListExitEffect());
        readOnlyPanel.gameObject.SetActive(true);
        readOnlyPanel.SetLegalitiesText();*/
    }
    public void EnableResponsibleGamingPanelView()
    {
        StartCoroutine(ServerManager.instance.commonWebView.WebViewPanelViewEntryEffect
            ("Responsible Gaming", Constants.URL.RESPONSIBLE_GAMING));
        /*StartCoroutine(AccountMenuListExitEffect());
        readOnlyPanel.gameObject.SetActive(true);
        readOnlyPanel.SetResponsibleGamingText();*/
    }

    public void EnablePrivatePolicyPanelView()
    {
        StartCoroutine(ServerManager.instance.commonWebView.WebViewPanelViewEntryEffect
            ("Privacy Policy", Constants.URL.PRIVACY_POLICY));
        /*StartCoroutine(AccountMenuListExitEffect());
        readOnlyPanel.gameObject.SetActive(true);
        readOnlyPanel.SetPrivatePolicyText();*/
    }
    public void EnableAboutUsPanelView()
    {
        StartCoroutine(ServerManager.instance.commonWebView.WebViewPanelViewEntryEffect
            ("About Us", Constants.URL.ABOUT_US));
        /*StartCoroutine(AccountMenuListExitEffect());
        readOnlyPanel.gameObject.SetActive(true);
        readOnlyPanel.SetAboutUsText();*/
    }

    
    public void Logout()
    {
        ServerManager.instance.alertPopUp.ShowView("Are you Sure, Do you want to Logout?", () =>
        {
            lobbyview.CheckforActivepanels();
            ServerManager.instance.loader.ShowLoader("Logging Out...");
            var Request = new LogoutRequest();
            ServerManager.instance.sfs.Send(Request);
            PlayerPrefs.DeleteAll();
            Caching.ClearCache();
            lobbyview.CheckforActivepanels();
        }, "Yes", () => { }, "No");
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

    private void HowToPlayPanel()
    {
        AudioController.instance.OnClick();
        StartCoroutine(AccountMenuListExitEffect());
        lobbyview.CheckforActivepanels();
        StartCoroutine(ServerManager.instance.commonWebView.WebViewPanelViewEntryEffect("How to Play", Constants.URL.HOW_TO_PLAY));
    }

    private void EnableContactUs()
    {
        AudioController.instance.OnClick();
        StartCoroutine(AccountMenuListExitEffect());
        lobbyview.CheckforActivepanels();
        contactUsForm.gameObject.SetActive(true);
    }


    public void EnableTransactionSubList()
    {
        transactionsSubMenuList.SetActive(!transactionsSubMenuList.activeInHierarchy);
    }

    public void EnableKYCSubList()
    {
        kycSubMenuList.SetActive(!kycSubMenuList.activeInHierarchy);
    }

    public void EnableDepositTransactions()
    {
        StartCoroutine(AccountMenuListExitEffect());
        lobbyview.CheckforActivepanels();
        depositTransactionsPanel.gameObject.SetActive(true);
    }

    public void EnableWithDrawTransactions()
    {
        StartCoroutine(AccountMenuListExitEffect());
        lobbyview.CheckforActivepanels();
        withdrawPanelView.gameObject.SetActive(true);

    }

    public void EnableAccountDetails()
    {
        StartCoroutine(AccountMenuListExitEffect());
        lobbyview.CheckforActivepanels();
        accountPanelView.gameObject.SetActive(true);
    }

    public void EnableWithdrawalHistoryDetails()
    {
        StartCoroutine(AccountMenuListExitEffect());
        lobbyview.CheckforActivepanels();
        withdrawTransactionsPanel.gameObject.SetActive(true);
    }

    public void EnableKYCDetails()
    {

        if (Application.platform == RuntimePlatform.Android && (!Input.location.isEnabledByUser || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)))
        {
            LocationManager.instance.CheckForLocation();
        }
        else
        {
            StartCoroutine(AccountMenuListExitEffect());
            lobbyview.CheckforActivepanels();
            kycPanelView.gameObject.SetActive(true);
        }
    }

    public void EnableBankDetails()
    {
        StartCoroutine(AccountMenuListExitEffect());
        lobbyview.CheckforActivepanels();
        bankStatementRequestPanelView.gameObject.SetActive(true);
    }

    public void EnableBonusTransactions()
    {
        StartCoroutine(AccountMenuListExitEffect());
        lobbyview.CheckforActivepanels();
        bonusTransactionsPanel.gameObject.SetActive(true);
    }

    public void EnablePromotionTab()
    {
        StartCoroutine(AccountMenuListExitEffect());
        lobbyview.CheckforActivepanels();
        referFriendPanel.gameObject.SetActive(true);
    }

    public void EnableGameHistory()
    {
        AudioController.instance.OnClick();
        StartCoroutine(AccountMenuListExitEffect());
        lobbyview.CheckforActivepanels();
        gameHistoryPanel.gameObject.SetActive(true);
        StartCoroutine(APIManager.instance.GameHistory());
    }

    public void EnableHostedGamesPanel()
    {
        AudioController.instance.OnClick();
        StartCoroutine(AccountMenuListExitEffect());
        lobbyview.CheckforActivepanels();
        StartCoroutine(APIManager.instance.HostedGameDetails(() => 
        {
            hostedPrivateGamesPanel.gameObject.SetActive(true);
        }));
    }

    private void PlayGame()
    {
        KYCPanelBtn.interactable = false;
        withDrawalHistoryPanelBtn.interactable = false;
        if (!UIManager.instance.lobbyView.isPrivateTable)
        {
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
        KYCPanelBtn.interactable = true;
        withDrawalHistoryPanelBtn.interactable = true;
    }
}
