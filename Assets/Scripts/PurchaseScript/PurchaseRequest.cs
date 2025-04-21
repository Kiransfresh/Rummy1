
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class PurchaseRequest : MonoBehaviour
{
	public Button Buybtn;
	public TextMeshProUGUI PurchaseAmount;
	public TextMeshProUGUI numberCoinsCount;
	public PurchaseController purchasecontroller;

	private void Start()
	{
		Buybtn.onClick.AddListener(OnBuyClick);
	}

	private void OnEnable()
	{
		CalculateAmountToBuyCoins();
	}
	void OnBuyClick()
	{
		if (!string.IsNullOrEmpty(PurchaseAmount.text))
		{
			Decimal Amount = 0;
			try
			{
				Amount = Decimal.Parse(PurchaseAmount.text);
			}
			catch (Exception e)
			{
				ServerManager.instance.alertPopUp.ShowView("Please enter a valid number");
				return;
			}
			var numberOfCoins = numberCoinsCount.text.Split();
			purchasecontroller.OnBuyClicked(PurchaseAmount.text, numberOfCoins[0]);
		}
	}

	public void CalculateAmountToBuyCoins() 
	{ 
		var values = numberCoinsCount.text.Split();
		var numberOfCoins = int.Parse(values[0]);
		var oneCoinPrize = int.Parse(CacheMemory.coinprice);
		PurchaseAmount.text = (numberOfCoins * oneCoinPrize).ToString();
	}
}
