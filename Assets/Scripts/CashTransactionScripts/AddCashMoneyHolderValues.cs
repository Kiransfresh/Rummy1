using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddCashMoneyHolderValues : MonoBehaviour
{
    public Toggle amountToggle;
    public Color amountActiveTextColor;
    public Color amountDeactiveTextColor;

    public TextMeshProUGUI amountText;

    private void Start()
    {
        SetInitialValue();

        amountToggle.onValueChanged.AddListener((value) =>
        {
            ChangeAmountField();
        });
    }

    public void SetInitialValue()
    {
        amountText.color = amountToggle.isOn ? amountActiveTextColor : amountDeactiveTextColor;
        if (!amountToggle.isOn) return;
        amountToggle.isOn = amountText.text == "100";
        UIManager.instance.lobbyView.addCashPanelView.amountField.text = amountText.text;
    }


    public void ChangeAmountField()
    {
        amountText.color = amountToggle.isOn ? amountActiveTextColor : amountDeactiveTextColor;
        if (!amountToggle.isOn) return;

        if(UIManager.instance.lobbyView.addCashPanelView != null)
        UIManager.instance.lobbyView.addCashPanelView.amountField.text = amountText.text;

        if (UIManager.instance.lobbyView.addCashPanelViewV2 != null)
            UIManager.instance.lobbyView.addCashPanelViewV2.amountField.text = amountText.text;
    }
}
