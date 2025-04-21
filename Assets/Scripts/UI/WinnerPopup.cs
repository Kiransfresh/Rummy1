using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinnerPopup : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private ScalingEffect scalingEffect;

    [Header("Buttons")]
    [SerializeField] private Button closeBtn;

    [Header("Information Texts")]
    [SerializeField] private TextMeshProUGUI winnerMessageText;
    [SerializeField] private TextMeshProUGUI gameDetailsText;
    [SerializeField] private TextMeshProUGUI gameEntryFeeText;


    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    private Action btnCallback;

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
        closeBtn.onClick.AddListener(() =>
        {
            StartCoroutine(DisablePopUp());
        });
    }


    private IEnumerator EnablePopUp()
    {
        yield return startDelay;
        AudioController.instance.PlayPopUpAlert();
        StartCoroutine(scalingEffect.EntryEffect());
    }

    private IEnumerator DisablePopUp()
    {

        StartCoroutine(scalingEffect.ExitEffect());
        yield return disableDelay;
        btnCallback?.Invoke();
        gameObject.SetActive(false);
    }




    public void ShowView(string message, string gameDetails, string entryFee, Action btnCallback)
    {

        this.btnCallback = btnCallback;

        message = Regex.Match(message, @"\d+").Value;


        winnerMessageText.text = int.Parse(message).ToString();
        gameDetailsText.text = gameDetails;
        gameEntryFeeText.text = float.Parse(entryFee).ToString("0");
        gameObject.SetActive(true);
    }

}
