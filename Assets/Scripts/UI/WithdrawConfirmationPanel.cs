using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WithdrawConfirmationPanel : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private ScalingEffect scalingEffect;

    [Header("Buttons")]
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button confirmBtn;

    [Header("Information Texts")]
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField amountInputField;

    

    void Start()
    {
        messageText.text = "Enter amount, Which you want to withdraw";
        confirmBtn.onClick.AddListener(OnConfirmClick);

        closeBtn.onClick.AddListener(()=> {
            StartCoroutine(DisablePopUp());
        });
    }

    private void OnEnable()
    {

        StartCoroutine(EnablePopUp());
    }

    private IEnumerator EnablePopUp()
    {
        yield return new WaitForSeconds(0.15f);
        AudioController.instance.PlayPopUpAlert();
        StartCoroutine(scalingEffect.EntryEffect());
    }

    private IEnumerator DisablePopUp()
    {

        StartCoroutine(scalingEffect.ExitEffect());
        yield return new WaitForSeconds(0.6f);
        gameObject.SetActive(false);
    }

    private void OnConfirmClick() {
        try
        {
            int amount = int.Parse(amountInputField.text.ToString());
            ServerManager.instance.loader.ShowLoader("Submitting...");
            StartCoroutine(APIManager.instance.WithdrawVerification(amount.ToString()));
            amountInputField.text = "";
            StartCoroutine(DisablePopUp());
        } catch (Exception e) {
            ServerManager.instance.alertPopUp.ShowView("Please enter the withdraw amount");
        }
    }

}
