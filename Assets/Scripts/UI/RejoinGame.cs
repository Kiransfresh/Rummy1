using System;
using System.Collections;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RejoinGame : TableBaseMono
{
    [SerializeField] private Button acceptBtn;
    [SerializeField] private Button rejectBtn;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI timeCountext;
    [SerializeField] private Image timerImage;
    private bool isCounterStart;
    private float maxFillAmount = 1f;
    private int time;
    private int counter = 0;

    private void Start()
    {
        acceptBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            ServerManager.instance.SendRejoinAcceptance(true);
        });

        rejectBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            ServerManager.instance.SendRejoinAcceptance(false);
        });
    }

    private void Update()
    {
        Timer();
    }

    private Coroutine rejoinTimerCoroutine = null;
    public void SetRejoinDetails(Response<RejoinModel> response)
    {
        gameObject.SetActive(true);
        messageText.text = response.message;
        if (response.data.time > 0)
        {
            counter = 0;
            timerImage.fillAmount = 0;
            time = response.data.time;
            if (rejoinTimerCoroutine != null)
            {
                StopCoroutine(rejoinTimerCoroutine);
            }
            rejoinTimerCoroutine = StartCoroutine(RejoinTimer(response.data.time));
        }
        else {
            gameObject.SetActive(false);
        }
    }

    public IEnumerator RejoinTimer(int timer)
    {
        isCounterStart = false;
        while (!isCounterStart)
        {
            timeCountext.text = (timer - counter).ToString();
            yield return new WaitForSeconds(1f);
            if (timer <= counter)
            {
                isCounterStart = true;
                gameObject.SetActive(false);
            }
            counter++;
        }
    }

    private void Timer()
    {
        if (isCounterStart) return;
        timerImage.fillAmount += (maxFillAmount / time) * Time.deltaTime;
        if (timerImage.fillAmount != maxFillAmount) return;
    }
}
