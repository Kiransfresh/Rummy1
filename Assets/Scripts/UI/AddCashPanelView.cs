using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Kakera;

public class AddCashPanelView : MonoBehaviour, IActivePanel
{
    #region PUBLIC_VARS
    [Header("Animation")]
    [SerializeField] private SlidingEffect slidingEffect;

    /* [Header("Gallery Picker")]
     [SerializeField] private Unimgpicker unimgpicker;
     [SerializeField] private PickerController pickerController;*/

    [Header("Buttons")]
    [SerializeField] private TextMeshProUGUI existingAmountText;
    [SerializeField] private TextMeshProUGUI upiIDText;

    [Header("Input fields")]
    [SerializeField] public TMP_InputField amountField;

    [Header("Buttons")]
    [SerializeField] private Button addCashBtn;
    [SerializeField] private Button backBtn;
    [SerializeField] public TextMeshProUGUI documentDisplayText;

    private Texture2D deposit_attachment;
    public TextMeshProUGUI paymenttxt;
    public LobbyView lobbyView;

    /*[Header("Game Objects")]
    [SerializeField] private GameObject bonusGameObject;*/
    #endregion

    #region PRIVATE_VARS
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    #endregion

    #region UNITY_CALLBACKS
    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void Start()
    {
        //bonusGameObject.SetActive(Constants.GAME_CONFIG.bonus);

        backBtn.onClick.AddListener(() =>
        {
            lobbyView.UpdateHeader();
            StartCoroutine(AddCashPanelViewExitEffect());
            Updateamount();
        });

        addCashBtn.onClick.AddListener(DespositMoneyToWallet);
    }

    private void OnEnable()
    {
        Updateamount();

        /*if(upiIDText != null)
        {
            upiIDText.text = CacheMemory.userModel.wallet.upi_id;
        }*/

        StartCoroutine(AddCashPanelViewEntryEffect());
        string latitude = LocationManager.instance.latitude;
        string longitude = LocationManager.instance.longitude;
        if (latitude == null || longitude == null)
        {
            LocationManager.instance.CheckForLocation();
        }
    }

    #endregion

    #region PRIVATE_FUNCTION

    private void PlayStartEffects()
    {
        StartCoroutine(slidingEffect.EntryEffect());
    }

    private void PlayEndEffect()
    {
        StartCoroutine(slidingEffect.ExitEffect());
    }
    #endregion

    #region CO-ROUTINES
    private IEnumerator AddCashPanelViewEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator AddCashPanelViewExitEffect()
    {
        StartCoroutine(slidingEffect.ExitEffect());
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    #endregion

    #region UI_CALLBACKS

    public void DisableAddCashPanelPanelView()
    {
        StartCoroutine(AddCashPanelViewExitEffect());
    }

    public void DeactivatePanel()
    {
        DisableAddCashPanelPanelView();
    }

    public void DespositMoneyToWallet()
    {
        StartCoroutine(APIManager.instance.Deposit());
    }

    public void Updateamount()
    {
        if (existingAmountText != null)
        {
            existingAmountText.text = Constants.Country.currency_symbol + CacheMemory.userModel.wallet.cash_deposit;
        }
    }
    #endregion
}