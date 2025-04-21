using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommonWebView : MonoBehaviour, IActivePanel
{
    public SlidingEffect[] slidingEffect;
    public TextMeshProUGUI titleText;

    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    private UniWebView webView;
    [SerializeField] private RectTransform webViewRectTransform;
    [SerializeField] private Button backBtn;

    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        //  StartCoroutine(WebViewPanelViewEntryEffect());
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() =>
        {
            DeactivatePanel();
        });
    }

    public IEnumerator WebViewPanelViewEntryEffect(string panelTitle, string url)
    {
        gameObject.SetActive(true);
        yield return startDelay;
        OpenWebView(panelTitle, url);
        PlayStartEffects();

    }

    private IEnumerator WebViewPanelViewExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    public void DisableWebViewPanel()
    {
        StartCoroutine(WebViewPanelViewExitEffect());
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
        if (titleText.text == "Deposit" || titleText.text == "Purchase Coins")
        {
            StartCoroutine(APIManager.instance.ValidationDeposit());
            UIManager.instance.lobbyView.UpdateHeader();
        }
        DisableWebViewPanel();
        CloseWebView();
    }

    public void OpenWebView(string panelTitle, string url)
    {
        ServerManager.instance.loader.ShowLoader("Loading");
        var webViewGameObject = new GameObject("UniWebView");
        webView = webViewGameObject.AddComponent<UniWebView>();
        webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
        titleText.text = panelTitle;
        var webUrl = url;
        webView.BackgroundColor = Color.clear;
        webView.Load(webUrl);
        Debug.Log("our url:" + webUrl);
        webView.Show();


        webView.OnPageFinished += (view, statusCode, url) =>
        {
            if (statusCode == 200)
            {
                ServerManager.instance.loader.HideLoader();
            }

            if (statusCode == 200 && (url.Contains("payment_success") || url.Contains("payment_failed")))
            {
                DeactivatePanel();
            }

        };

        webView.OnShouldClose += (view) =>
        {
            DeactivatePanel();
            return true;
        };

        webView.OnMessageReceived += (view, message) =>
        {
            if (message.Path.Equals("close"))
            {
                DeactivatePanel();
            }
        };
    }

    public void CloseWebView()
    {
        Destroy(webView);
        webView = null;
    }

    void OnRectTransformDimensionsChange()
    {
        if (webView == null) return;
        webView.UpdateFrame();
    }


}
