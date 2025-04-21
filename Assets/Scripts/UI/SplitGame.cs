using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SplitGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI timeCountext;
    [SerializeField] private Image timerImage;
    [SerializeField] private Button confirmButton;
    private bool isCounterStart;
    private float maxFillAmount = 1f;
    private int time;
    private int counter = 0;

    public GameObject splitRow;
    public Transform spawnParent;

    private Coroutine timerCoroutine;
    private bool timerCoroutineStatus = false;

    private void Start()
    {
        confirmButton.onClick.AddListener(() =>
        {
            confirmButton.gameObject.SetActive(false);
            ServerManager.instance.SendSplitAcceptance(true);
        });
    }


    private void Update()
    {
        Timer();
    }

    public void SetSplitDetails(Response<SplitModel> response)
    {
        gameObject.SetActive(true);
        messageText.text = response.message;
        SetRowInfo(response);
        if (timerCoroutineStatus == false)
        {
            timerCoroutineStatus = true;
            time = response.data.time;
            timerCoroutine = StartCoroutine(SplitAcceptTimer(response.data.time));
        }
    }

    private List<SplitRow> rowsData = new List<SplitRow>();

    public void SetRowInfo(Response<SplitModel> response)
    {
       
        if (response.data.splits.Count <= 0)
        {
            return;
        }

        foreach (var splitData in response.data.splits)
        {
            if (spawnParent.childCount >= response.data.splits.Count) break;
            var row = Instantiate(splitRow, spawnParent);
            var splitRowData = row.GetComponent<SplitRow>();
            rowsData.Add(splitRowData);
            splitRowData.playerName.text = splitData.unique_name;
            splitRowData.playerTotalScore.text = splitData.score;
            if (CacheMemory.GameType == Constants.GAME_TYPE.CASH)
            {
                splitRowData.winningAmountValueAfterSplit.text = Constants.Country.currency_symbol + splitData.amount.ToString();
            }
            else
            {
                splitRowData.winningAmountValueAfterSplit.text = splitData.amount.ToString() + " chips";
            }
            splitRowData.splitChoice.text = !splitData.accepted ? "Waiting" : "Accepted";
        }

        foreach (var splitData in response.data.splits)
        {
            foreach (var splitRow in rowsData)
            {
                if (splitRow.playerName.text.Equals(splitData.unique_name))
                {
                    splitRow.splitChoice.text = !splitData.accepted ? "Waiting" : "Accepted";
                }
            }
        }
    }

    public IEnumerator SplitAcceptTimer(int timer)
    {
        isCounterStart = false;
        while (!isCounterStart)
        {
            int pendingTime = timer - counter;
            timeCountext.text = pendingTime.ToString();
            yield return new WaitForSeconds(1f);
            if (timer <= counter || pendingTime <= 0)
            {
                ResetSplitRows();
            }
            counter++;
        }
    }

    private void Timer()
    {
        if (isCounterStart) return;
        timerImage.fillAmount += (maxFillAmount / time) * Time.deltaTime;
        if (timerImage.fillAmount != maxFillAmount) return;
        isCounterStart = true;
    }

    public void ResetSplitRows()
    {
        isCounterStart = true;
        for (var i = 0; i < spawnParent.childCount; i++)
        {
            Destroy(spawnParent.GetChild(i).gameObject);
        }
        confirmButton.gameObject.SetActive(true);
        timeCountext.text = "0";
        if (timerCoroutineStatus == true)
        {
            timerCoroutineStatus = false;
            timerImage.fillAmount = 0f;
            counter = 0;
            StopCoroutine(timerCoroutine);
        }
        rowsData.Clear();
        gameObject.SetActive(false);
    }
}
