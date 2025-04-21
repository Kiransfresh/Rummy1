using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnackBar : MonoBehaviour
{
    [Header("Animations")]
    public SlidingEffect slidingEffect;

    [Header("Message Text")]
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Buttons")]
    [SerializeField] private Button closeBtn;


    private WaitForSeconds startDelay = new WaitForSeconds(0.15f);
    private WaitForSeconds disableDelay = new WaitForSeconds(0.6f);

    private void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            if (messageCourotine != null)
            {
                StopCoroutine(messageCourotine);
            }
            StartCoroutine(ExitEffect());
        });

    }

    private void OnEnable()
    {
        StartCoroutine(EntryEffect());
    }

    private Coroutine messageCourotine;
    public void ShowMessage(string message) {
        gameObject.SetActive(true);
        messageText.text = message;
        if (messageCourotine != null) {
            StopCoroutine(messageCourotine);
            messageCourotine = null;
        }
        messageCourotine = StartCoroutine(ShowMessage());
    }


    private IEnumerator ShowMessage() {
        yield return new WaitForSeconds(4);
        if(messageCourotine != null)
        {
            StopCoroutine(messageCourotine);
            StartCoroutine(ExitEffect());
        }
    }

    

    private IEnumerator EntryEffect()
    {
        yield return startDelay;
        StartCoroutine(slidingEffect.EntryEffect());
    }

    public IEnumerator ExitEffect() {
        StartCoroutine(slidingEffect.ExitEffect());
        yield return disableDelay;
        gameObject.SetActive(false);
    }
}
