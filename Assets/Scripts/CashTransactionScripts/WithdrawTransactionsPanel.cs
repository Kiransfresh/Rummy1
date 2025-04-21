using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WithdrawTransactionsPanel : MonoBehaviour
{
    public GameObject transactionsRow;
    public Transform SpawnParent;

    public SlidingEffect[] slidingEffect;
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    [Header("Buttons")]
    [SerializeField] private Button backBtn;

    [Header("Error Message")]
    [SerializeField] private TextMeshProUGUI errorMessageText;

    private void Awake()
    {
        startDelay = new WaitForSeconds(2.15f);
        disableDelay = new WaitForSeconds(2.6f);
    }

    private void OnEnable()
    {
        StartCoroutine(EntryEffect());
        errorMessageText.gameObject.SetActive(true);
        errorMessageText.text = "Fetching...";
        StartCoroutine(APIManager.instance.WithdrawTransactionsList(SetTransactionsInfoCallback));
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() =>
        {
            DeactivatePanel();
        });
    }

    private IEnumerator EntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
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

    public void DeactivatePanel()
    {
        DisableDepositTransactionsPanel();
    }

    private void SetTransactionsInfoCallback(Response<List<WithdrawTransactionsModel>> response)
    {
        if (response.status == Constants.KEYS.valid && response.data.Count > 0)
        {
            errorMessageText.gameObject.SetActive(false);
            foreach (var transaction in response.data)
            {
                var transactionRow = Instantiate(transactionsRow, SpawnParent);
                var withdrawTransaction = transactionRow.GetComponent<TransactionRow>();
                withdrawTransaction.transactionId.text = transaction.withdrawal_id;
                withdrawTransaction.amount.text = Constants.Country.currency_symbol + transaction.request_amount;
                withdrawTransaction.dateAndTime.text = transaction.request_date_time;

                string remark = "Request Status : " + transaction.request_status;

                if (transaction.request_status == "Completed") {
                    remark = remark + ", Transfer UTR : " + transaction.transfer_utr_ref_id + ", Transfer by : " + transaction.transfer_type;
                }

                withdrawTransaction.remarkText.text = remark;
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
        for (var i = 0; i < SpawnParent.childCount; i++)
        {
            Destroy(SpawnParent.GetChild(i).gameObject);
        }
    }

}
