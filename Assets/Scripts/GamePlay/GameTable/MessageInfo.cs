using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageInfo : MonoBehaviour
{
    [Header("Message Text")]
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Buttons")]
    [SerializeField] private Button declareBtn;


    #region Public_Variables
    public Action declareAction;
    #endregion

    #region Private_Variables
    private bool isCounterStart;
    #endregion

    private void Start()
    {
        declareBtn.onClick.AddListener(() =>
        {
            declareAction.Invoke();
            declareBtn.gameObject.SetActive(false);
        });

    }

    public void ShowWithMessage(string message)
    {
        StopAllCoroutines();
        isCounterStart = true;
        messageText.text = message;
        gameObject.SetActive(true);
    }

    public IEnumerator ShowWithMessage(string message, int timer)
    {
        var counter = 0;
        gameObject.SetActive(true);
        isCounterStart = false;
        while (!isCounterStart)
        {
            messageText.text = message + (timer - counter).ToString();
            yield return new WaitForSeconds(1f);
            if (timer <= counter)
            {
                isCounterStart = true;
                gameObject.SetActive(false);
                DeclareButton(false);
                StopAllCoroutines();
            }
            counter++;
        }
    }

    public IEnumerator ShowMessage(string message, float timer)
    {
        gameObject.SetActive(true);
        messageText.text = message;
        yield return new WaitForSeconds(timer);
        gameObject.SetActive(false);
        DeclareButton(false);
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        declareBtn.gameObject.SetActive(false);
    }

    public void DeclareButton(bool status)
    {
        declareBtn.gameObject.SetActive(status);
    }
}
