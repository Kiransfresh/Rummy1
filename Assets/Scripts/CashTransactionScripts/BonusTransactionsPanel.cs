using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusTransactionsPanel : MonoBehaviour
{
    public GameObject transactionsRow;
    public Transform SpawnParent;

    public SlidingEffect[] slidingEffect;
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    [SerializeField] private Button backBtn;

    [Header("Error Message")]
    [SerializeField] private TextMeshProUGUI errorMessageText;

    private void Awake()
    {
        startDelay = new WaitForSeconds(5.0f);
        disableDelay = new WaitForSeconds(10.6f);
    }

    private void OnEnable()
    {
        errorMessageText.gameObject.SetActive(true);
        errorMessageText.text = "Fetching...";
        PlayStartEffects();
        StartCoroutine(APIManager.instance.BonusTransactionsList(SetTransactionsInfoCallback));
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() =>
        {
            DeactivatePanel();

        });
    }

    private IEnumerator DepositTransactionsExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    public void DisableDepositTransactionsPanel()
    {
        StartCoroutine(DepositTransactionsExitEffect());
        ResetTransactionInfo();
    }

    private void PlayStartEffects()
    {
        for (int i = 1; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].EntryEffect());
        }
    }

    private void PlayEndEffect()
    {
        for (int i = 1; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].ExitEffect());
        }
    }

    public void DeactivatePanel()
    {
        DisableDepositTransactionsPanel();
    }

    public void SetTransactionsInfoCallback(Response<List<BonusTransactionsModel>> response)
    {
        if (response.status == Constants.KEYS.valid && response.data.Count > 0)
        {
            errorMessageText.gameObject.SetActive(false);
            foreach (var transaction in response.data)
            {
                var transactionRow = Instantiate(transactionsRow, SpawnParent);
                var bonusTransaction = transactionRow.GetComponent<TransactionRow>();
                bonusTransaction.transactionId.text = transaction.ref_id;
                bonusTransaction.amount.text = Constants.Country.currency_symbol + transaction.amount;
                bonusTransaction.dateAndTime.text = transaction.created_date_time;

                bonusTransaction.remarkText.gameObject.SetActive(true);

                bonusTransaction.remarkText.text = transaction.remark;
            }
        }
        else
        {
            errorMessageText.text = Constants.MESSAGE.TRANSACTION_ERROR;
            errorMessageText.gameObject.SetActive(true);
        }
    }

    public void ResetTransactionInfo()
    {
        for (var i = 1; i < SpawnParent.childCount; i++)
        {
            Destroy(SpawnParent.GetChild(i).gameObject);
        }
    }

}
