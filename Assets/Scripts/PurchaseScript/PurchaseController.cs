using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
public class PurchaseController : MonoBehaviour
{
    public SlidingEffect[] slidingEffect;
    
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    public Button backBtn;
    public Transform parentOfPurchaseRequest;

    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        StartCoroutine(PurchasePanelViewEntryEffect());
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() =>
        {
            DisablePurchasePanelPanelView();
        });
    }

    private IEnumerator PurchasePanelViewEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator PurchasePanelViewExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
       
    }

    public void DisablePurchasePanelPanelView()
    {
        StartCoroutine(PurchasePanelViewExitEffect());
    }

    private void PlayStartEffects()
    {
        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].EntryEffect());
        }
    }

    private void PlayEndEffect()
    {
        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].ExitEffect());
        }
    }

    public void DeactivatePanel()
    {
        DisablePurchasePanelPanelView();
    }

    public void OnBuyClicked(string amount, string numberOfCoins)
    {
        StartCoroutine(APIManager.instance.CoinPurchasing(amount, numberOfCoins));
    }

    public void UpdatePriceOfCoins() 
    {
        for (int i = 0; i < parentOfPurchaseRequest.childCount; i++) 
        {
            var purchaseRequest = parentOfPurchaseRequest.GetChild(i).GetComponent<PurchaseRequest>();
            purchaseRequest.CalculateAmountToBuyCoins();
        }
    }
}
