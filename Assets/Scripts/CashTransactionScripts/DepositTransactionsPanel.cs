using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DepositTransactionsPanel : MonoBehaviour
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
        startDelay = new WaitForSeconds(5.15f);
        disableDelay = new WaitForSeconds(6.6f);
    }

    private void OnEnable()
    {
        PlayStartEffects();

        errorMessageText.gameObject.SetActive(true);
        errorMessageText.text = "Fetching...";
        StartCoroutine(APIManager.instance.DepositTransactionsList(SetTransactionsInfoCallback));
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

    private void SetTransactionsInfoCallback(Response<List<DepositTransactionsModel>> response)
    {
        if (response.status == Constants.KEYS.valid && response.data.Count > 0)
        {
            errorMessageText.gameObject.SetActive(false);
            foreach (var transaction in response.data)
            {
                var transactionRow = Instantiate(transactionsRow, SpawnParent);
                var depositTransaction = transactionRow.GetComponent<TransactionRow>();
                depositTransaction.transactionId.text = transaction.transaction_id;
                depositTransaction.amount.text = Constants.Country.currency_symbol + transaction.credits_count;
                depositTransaction.dateAndTime.text = transaction.updated_at;

                string remark = "Status : " + transaction.transaction_status;

                if (string.IsNullOrEmpty(transaction.comment) == false)
                {
                    remark = remark + ", Comment : " + transaction.comment;
                }

                depositTransaction.remarkText.text = remark;
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
