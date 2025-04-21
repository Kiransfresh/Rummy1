using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlerPopUp : MonoBehaviour
{
    public ScalingEffect scalingEffect;
    public FadingColorEffect fadingColorEffect;
    public TextMeshProUGUI descriptionText;
    public Button acceptBtn;
    public Button rejectBtn;
    
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    private bool isAccepted;

    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        StartCoroutine(EnablePopUp());
    }

    private void Start()
    {
        acceptBtn.onClick.AddListener(() =>
        {
            isAccepted = true;
            DisableVerificationPanel();
        });

        rejectBtn.onClick.AddListener(() =>
        {
            isAccepted = false;
            DisableVerificationPanel();
        });
    }
    
    private void PlayStartEffects()
    {
        AudioController.instance.PlayPopUpAlert();
        StartCoroutine(fadingColorEffect.ChangeColor());
        StartCoroutine(scalingEffect.EntryEffect());
       
    }

    public void PlayEndEffect()
    {
        StartCoroutine(fadingColorEffect.SetInitialColor());
        StartCoroutine(scalingEffect.ExitEffect());
    }

    private IEnumerator EnablePopUp()
    {
        //SAI
        isAccepted = false;
        yield return startDelay;

        PlayStartEffects();
    }

    private IEnumerator DisablePopUp()
    {

        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);

        if(isAccepted)
            acceptCallback?.Invoke();
        else
            rejectCallback?.Invoke();
    }
    
    public void DisableVerificationPanel()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DisablePopUp());
        }
    }

    

    public void ShowView(string message)
    {
        
        acceptCallback = null;
        rejectCallback = null;
        descriptionText.text = message;
        rejectBtn.gameObject.SetActive(false);
        acceptBtn.transform.GetComponentInChildren<Text>().text = "Ok";
        gameObject.SetActive(true);
    }

    private Action acceptCallback;
    private Action rejectCallback;
    public void ShowView(string message, Action acceptCallback, string acceptBtnTitle, 
        Action rejectCallback, string rejectBtnTitle)
    {   
        gameObject.SetActive(true);
        descriptionText.text = message;
        acceptBtn.transform.GetComponentInChildren<Text>().text = acceptBtnTitle ?? "Ok";
        if (rejectBtnTitle == null) {
            rejectBtn.gameObject.SetActive(false);
        } else {
            rejectBtn.transform.GetComponentInChildren<Text>().text = rejectBtnTitle ?? "Cancel";
        }

        if (acceptCallback != null)
        {
            this.acceptCallback = acceptCallback;
        }
        else
        {
            acceptBtn.gameObject.SetActive(false);
        }
        if (rejectCallback != null)
        {
            this.rejectCallback = rejectCallback;
            rejectBtn.gameObject.SetActive(true);
        }
        else
        {
            rejectBtn.gameObject.SetActive(false);
        }

    }
   
}
