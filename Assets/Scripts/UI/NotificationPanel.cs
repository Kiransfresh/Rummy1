using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NotificationPanel : MonoBehaviour,IActivePanel
{
    public SlidingEffect[] slidingEffect;
    public GameObject NotificationSlot;
    public GameObject NotificationSlotparent;

    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    public Button backBtn;

    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        StartCoroutine(GetNotification());
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() =>
        {
            DisableNotificationPanelPanelView();
        });
    }

    private IEnumerator NotificationPanelViewEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator NotificationPanelViewExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
        ClearNotification(NotificationSlotparent.transform);
    }

    public void DisableNotificationPanelPanelView()
    {
        StartCoroutine(NotificationPanelViewExitEffect());
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
        DisableNotificationPanelPanelView();
    }

    public IEnumerator GetNotification()
    {

        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
       
        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.FETCH_NOTIFICATIONS, form))
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

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<List<NotificationModel>>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<List<NotificationModel>>));
                if (response.status == Constants.KEYS.valid)
                {
                    StartCoroutine(NotificationPanelViewEntryEffect());
                    ShowNotification(response);
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
            }
        }
    }

    public IEnumerator MakeNotificationReadable(string id)
    {

        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField(Constants.KEYS.id, id);

        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.NOTIFICATION_MARK_READ, form))
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

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<SubResponse>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<SubResponse>));
                if (response.status == Constants.KEYS.valid)
                {
                    
                }
                else
                {
                    
                }
            }
        }
    }

    private void ShowNotification(Response<List<NotificationModel>> response)
    {
        foreach (var model in response.data)
        {
            var temp = Instantiate(NotificationSlot, NotificationSlotparent.transform);
            var notificationSlot = temp.GetComponent<NotificationSlot>();

            string cleaned = model.message.Replace("\n", " ").Replace("\r", " ");

            notificationSlot.messageText.text = cleaned;
           
            notificationSlot.readableNotificationBtn.onClick.AddListener(() =>
            {
                StartCoroutine(MakeNotificationReadable(model.id));
            });
        }
    }

    private void ClearNotification(Transform parent)
    {
        for (var i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
