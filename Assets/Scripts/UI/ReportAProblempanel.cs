using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class ReportAProblempanel : MonoBehaviour
{
    public ScalingEffect[] ScalingEffect;


    [SerializeField] private TMP_InputField commentField;
    [SerializeField] private TextMeshProUGUI issueTypeText;

    private int versionCode = 1;

    
    public Button ReportBtn;
    
    private List<string> IssueTypes = new List<string>();

    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;


    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void Start()
    {
        ReportBtn.onClick.AddListener(OnReportClick);
    }
   
    private void OnEnable()
    {
        commentField.text = "";
        StartCoroutine(ReportPanelViewEntryEffect());
    }
   
    void OnReportClick()
    {

        StartCoroutine(SendProblem());
    }
  


    private IEnumerator ReportPanelViewEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator ReportPanelViewExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }




    public void DisableReportPanelPanelView()
    {
        StartCoroutine(ReportPanelViewExitEffect());
    }


    private void PlayStartEffects()
    {
        for (int i = 0; i < ScalingEffect.Length; i++)
        {
            StartCoroutine(ScalingEffect[i].EntryEffect());
        }
    }

    private void PlayEndEffect()
    {
        for (int i = 0; i < ScalingEffect.Length; i++)
        {
            StartCoroutine(ScalingEffect[i].ExitEffect());
        }
    }   

    public IEnumerator SendProblem()
    {

        var form = new WWWForm();

        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField(Constants.KEYS.type_of_issue, issueTypeText.text); 
        form.AddField(Constants.KEYS.message, commentField.text);               
        form.AddField(Constants.KEYS.version_code, versionCode);

#if UNITY_ANDROID
        form.AddField(Constants.KEYS.device_type, Constants.DEVICE_TYPE.Unity_android);
#elif UNITY_IPHONE
        form.AddField(Constants.KEYS.device_type, Constants.DEVICE_TYPE.Unity_Ios);  
#elif UNITY_WEBGL
        form.AddField(Constants.KEYS.device_type, Constants.DEVICE_TYPE.Unity_Webgl);    
#endif


        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.REPORT_A_PROBLEM, form))
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
                var response = (Response<SubResponse>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<SubResponse>));
                if (response.status == Constants.KEYS.valid)
                {
                    DisableReportPanelPanelView();
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
            }
        }
    }
}
