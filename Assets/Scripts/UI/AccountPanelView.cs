using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AccountPanelView : MonoBehaviour,IActivePanel
{
    [Header("Animations")]
    [SerializeField] private SlidingEffect[] slidingEffect;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI totalCashText;
    [SerializeField] private TextMeshProUGUI inPlayCashText;
    [SerializeField] private TextMeshProUGUI withdrawCashText;
    [SerializeField] private TextMeshProUGUI bonus;

    [Header("Header buttons")]
    [SerializeField] private Button backBtn;

    [Header("Game Object")]
    [SerializeField] private GameObject bonusGameObject;

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

    private void OnEnable()
    {
     
        bonusGameObject.SetActive(Constants.GAME_CONFIG.bonus);
        SetWalletDetails();
        StartCoroutine(AccountPanelViewEntryEffect());
    }

    #endregion

    private void Start()
    {
        backBtn.onClick.AddListener(() =>
        {
            DisableAccoutPanelView();
        });
    }

    #region CO-ROUTINES
    private IEnumerator AccountPanelViewEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator AccountPanelViewExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    #endregion

    #region UI_CALLBACKS

    public void DisableAccoutPanelView()
    {
        StartCoroutine(AccountPanelViewExitEffect());
    }

    public void DeactivatePanel()
    {
        DisableAccoutPanelView();
    }

    #endregion

    #region PRIVATE_FUNCTION

    private void PlayStartEffects()
    {
        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].EntryEffect());
        }
    }

    private void PlayEndEffect()
    {
        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].ExitEffect());
        }
    }

    private void SetWalletDetails() {
        UserModel.Wallet wallet = CacheMemory.userModel.wallet;
        totalCashText.text = Constants.Country.currency_symbol + wallet.cash;
        inPlayCashText.text = Constants.Country.currency_symbol + wallet.in_play_cash;
        withdrawCashText.text = Constants.Country.currency_symbol + wallet.cash_withdrawal;
        bonus.text = Constants.Country.currency_symbol + wallet.total_bonus;
    }

    #endregion
}
