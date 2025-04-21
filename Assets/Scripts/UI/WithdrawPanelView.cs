using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WithdrawPanelView : MonoBehaviour
{

    [Header("Animation")]
    [SerializeField] private SlidingEffect slidingEffect;

    [Header("Header buttons")]
    [SerializeField] private Button backBtn;

    [Header("Text information")]
    [SerializeField] private TextMeshProUGUI totalCashHeader;
    [SerializeField] private TextMeshProUGUI totalCashWithdrawal;


    [Header("Buttons information")]
    [SerializeField] private Button withdrawBtn;
    [SerializeField] private Button bankAccountDetailsBtn;
    [SerializeField] private Button addCashBtn;
    [SerializeField] private Button transactionHistory;
    [SerializeField] private Button informationPopup;

    [Header("Other Panels")]
    [SerializeField] private WithdrawConfirmationPanel withdrawConfirmationPanel;



    #region PRIVATE_VARS
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    private BankDetailsModel detailsModel;
    #endregion

    #region UNITY_CALLBACKS
    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }
    private void OnEnable()
    {
        ServerManager.instance.loader.ShowLoader("Fetching...");
        StartCoroutine(WithdrawPanelViewEntryEffect());
        UIManager.instance.SendProfileRequest(GetProfileCallback);
        //StartCoroutine(APIManager.instance.GetProfileDetails(GetProfileCallback));

        //string latitude = LocationManager.instance.latitude;
        //string longitude = LocationManager.instance.longitude;
        //if (latitude == null || longitude == null)
        //{
        //    LocationManager.instance.CheckForLocation();
        //}
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() => { DisableWithdrawPanelView(); });

        withdrawBtn.onClick.AddListener(() =>
        {
            withdrawConfirmationPanel.gameObject.SetActive(true);
        });

        bankAccountDetailsBtn.onClick.AddListener(() =>
        {
            UIManager.instance.accountMenuView.bankStatementRequestPanelView.gameObject.SetActive(true);
        });

        addCashBtn.onClick.AddListener(()=> {
            UIManager.instance.lobbyView.addCashPanelView.gameObject.SetActive(true);
        });

        transactionHistory.onClick.AddListener(()=> {
            UIManager.instance.lobbyView.withdrawTransactionsPanel.gameObject.SetActive(true);
        });

        informationPopup.onClick.AddListener(()=> {
            ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.WITHDRAW_AMOUNT_WARNING);
        });


    }
    #endregion


    #region API_CALLBACK
    public void GetProfileCallback(Response<UserModel> response)
    {
        ServerManager.instance.loader.HideLoader();
        if (response.status == Constants.KEYS.valid) { 
            totalCashHeader.text = Constants.Country.currency_symbol + response.data.wallet.cash.ToString();
           float withdrawalAmount = response.data.wallet.cash_withdrawal;
            totalCashWithdrawal.text = Constants.Country.currency_symbol + withdrawalAmount.ToString();
        }  else  {
            ServerManager.instance.alertPopUp.ShowView(response.message);
        }
    }
    #endregion

    #region CO-ROUTINES
    private IEnumerator WithdrawPanelViewEntryEffect()
    {
        yield return startDelay;
        StartCoroutine(slidingEffect.EntryEffect());
    }

    private IEnumerator WithdrawPanelViewExitEffect()
    {
        StartCoroutine(slidingEffect.ExitEffect());
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    #endregion

    
    private void DisableWithdrawPanelView()
    {
        StartCoroutine(WithdrawPanelViewExitEffect());
    }

}
