using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SplashView : MonoBehaviour
{
    #region PUBLIC_VARS
    public SlidingEffect[] startingEffect;
    public Slider loadingSlider;
    public GameObject sliderFillArea;
    #endregion

    #region PRIVATE_VARS
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    private bool isLoginLoaded = false;
    private float timeToStaySplash = 3.0f;
    private float timeToLoadLogin = 0f;
    private bool locationEnabled = false;
    #endregion

    #region UNITY_CALLBACKS
    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        DisableAutoRotation();
        StartCoroutine(LogoEntryONSplash());
    }

    public void DisableAutoRotation()
    {
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    private void Update()
    {
        if (!isLoginLoaded)
        {
            timeToLoadLogin += Time.deltaTime;
            loadingSlider.value = timeToLoadLogin / timeToStaySplash;
            if (loadingSlider.value <= 0)
            {
                sliderFillArea.SetActive(false);
            }
            else if (loadingSlider.value >= 0.1f)
            {
                sliderFillArea.SetActive(true);
            }
            if (loadingSlider.value >= 1f)
            {
                isLoginLoaded = true;
                StartCoroutine(SplashExit());
            }
        }
    }
    #endregion

    #region PRIVATE_FUNCTION

    private void PlayStartEffects()
    {
        for (int i = 0; i < startingEffect.Length; i++)
        {
            StartCoroutine(startingEffect[i].EntryEffect());
        }
    }

    private void PlayEndEffect()
    {
        for (int i = 0; i < startingEffect.Length; i++)
        {
            StartCoroutine(startingEffect[i].ExitEffect());
        }
    }

    #endregion

    #region CO-ROUTINES
    private IEnumerator LogoEntryONSplash()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator SplashExit()
    {
        yield return disableDelay;

        PlayEndEffect();
        yield return new WaitForSeconds(0.5f);
#if UNITY_WEBGL
        string url = "https://test.moonrummy.com/playtool/t/index.html?INIGXD0fGmJSk9atuZNltwkHt1X9EoYC&249";
        if (url != "")
      //  if (Application.absoluteURL != "")
        {
            string[] UrlParams = null;
          //  int flag = Application.absoluteURL.IndexOf("?", StringComparison.Ordinal);
            int flag = url.IndexOf("?", StringComparison.Ordinal);
            if (flag != -1)
            {
              //  UrlParams = Application.absoluteURL.Split('?')[1].Split('&');
                UrlParams = url.Split('?')[1].Split('&');
            }

            string authToken = UrlParams[0];
            PlayerPrefsManager.SetAuthToken(authToken);
        }
#endif

        if (PlayerPrefsManager.GetAuthToken() != null)
        {
            ServerManager.instance.loader.ShowLoader(Constants.MESSAGE.CONNECTING);
            ServerManager.instance.Initialise();
        }
        else
        {
            /*ServerManager.instance.alertPopUp.ShowView("You need to allow your location",
                        () =>
                        {
                           
                        }, "OK", () => { Application.Quit(); }, "Cancel");*/
#if UNITY_WEBGL
            Application.ExternalEval("Quit()");
#else
            UIManager.instance.loginView.gameObject.SetActive(true);
#endif
        }
        

        gameObject.SetActive(false);
    }
#endregion
}
