using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ReadOnlyPanel : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField] private SlidingEffect[] slidingEffect;

    [Header("Buttons")]
    [SerializeField] private Button backBtn;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI TitleText;
    [SerializeField] private TextMeshProUGUI DescriptionText;
    
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        StartCoroutine(ReadOnlyPanelViewEntryEffect());
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() =>
        {
            StartCoroutine(ReadOnlyPanelViewExitEffect());
        });
    }

    private IEnumerator ReadOnlyPanelViewEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator ReadOnlyPanelViewExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
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

   

    public void SetTermsConditionsText() {
        TitleText.text = "Terms and conditions";
        StartCoroutine(GetTermsConditions());
    
    }

    public void SetPrivatePolicyText()
    {
        TitleText.text = "Privacy policy";
        StartCoroutine(GetPrivatePolicy());
    }

    public void SetLegalitiesText()
    {
        TitleText.text = "Legalities";
        StartCoroutine(GetLegality());

    }

    public void SetResponsibleGamingText()
    {
        TitleText.text = "Responsible gaming";
        StartCoroutine(GetResponsibleGaming());

    }

    public void SetAboutUsText()
    {
        TitleText.text = "About us";
        StartCoroutine(GetAboutUs());
    }

    IEnumerator GetTermsConditions()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Constants.API_METHODS.TERMS_CONDITIONS))
        {
#if UNITY_ANDROID
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_android);
#elif UNITY_IPHONE
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_Ios);
#elif UNITY_WEBGL
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_Webgl);
#else
            www.SetRequestHeader(Constants.KEYS.requesting_source, "Website");
#endif
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response <ReadOnlyPanelModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<ReadOnlyPanelModel>));

                if (response.status == Constants.KEYS.valid)
                {
                    DescriptionText.text = response.data.html_content.plain_description;
                }
                else
                {

                }
            }
        }
    }

    IEnumerator GetPrivatePolicy()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Constants.API_METHODS.PRIVATE_POLICY))
        {
#if UNITY_ANDROID
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_android);
#elif UNITY_IPHONE
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_Ios);
#elif UNITY_WEBGL
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_Webgl);
#else
            www.SetRequestHeader(Constants.KEYS.requesting_source, "Website");
#endif
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<ReadOnlyPanelModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<ReadOnlyPanelModel>));

                if (response.status == Constants.KEYS.valid)
                {
                    DescriptionText.text = response.data.html_content.plain_description;
                }
                else
                {

                }
            }
        }
    }

    IEnumerator GetLegality()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Constants.API_METHODS.Legality))
        {
#if UNITY_ANDROID
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_android);
#elif UNITY_IPHONE
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_Ios);
#elif UNITY_WEBGL
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_Webgl);
#else
            www.SetRequestHeader(Constants.KEYS.requesting_source, "Website");
#endif
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<ReadOnlyPanelModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<ReadOnlyPanelModel>));

                if (response.status == Constants.KEYS.valid)
                {
                    DescriptionText.text = response.data.html_content.plain_description;
                }
                else
                {

                }
            }
        }
    }

    IEnumerator GetResponsibleGaming()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Constants.API_METHODS.Responsible_Gaming))
        {
#if UNITY_ANDROID
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_android);
#elif UNITY_IPHONE
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_Ios);
#elif UNITY_WEBGL
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_Webgl);
#else
            www.SetRequestHeader(Constants.KEYS.requesting_source, "Website");
#endif
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<ReadOnlyPanelModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<ReadOnlyPanelModel>));

                if (response.status == Constants.KEYS.valid)
                {
                    DescriptionText.text = response.data.html_content.plain_description;
                }
                else
                {

                }
            }
        }
    }

    IEnumerator GetAboutUs()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Constants.API_METHODS.ABOUT_US))
        {
#if UNITY_ANDROID
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_android);
#elif UNITY_IPHONE
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_Ios);
#elif UNITY_WEBGL
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_Webgl);
#else
            www.SetRequestHeader(Constants.KEYS.requesting_source, "Website");
#endif
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<ReadOnlyPanelModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<ReadOnlyPanelModel>));

                if (response.status == Constants.KEYS.valid)
                {
                    DescriptionText.text = response.data.html_content.plain_description;
                }
                else
                {

                }
            }
        }
    }


}
