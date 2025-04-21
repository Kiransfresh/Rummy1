using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class InquiryFormView : MonoBehaviour,IActivePanel
{
    public SlidingEffect[] slidingEffect;
    public TMP_InputField Name;
    public TMP_InputField Mobile;
    public TMP_InputField WhatsAppNum;
    public TMP_InputField Email;
    public TMP_InputField MoreDetails;
    public TMP_Dropdown BudgetDropDown,RequiredTimeDropDown,SolutionsTypesDropDown;
    public Button backBtn;
    public Button SubmitBtn;
    private string SelectedBudget, SelectedTime, SelectedSolution;
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;


    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
        SelectedBudget = BudgetDropDown.options[0].text.ToString();
        SelectedTime = RequiredTimeDropDown.options[0].text.ToString();
        SelectedSolution = SolutionsTypesDropDown.options[0].text.ToString();
    }
    private void OnEnable()
    {
        Name.text = "";
        Mobile.text = "";
        WhatsAppNum.text = "";
        Email.text = "";
        MoreDetails.text = "";
        BudgetDropDown.value = 0;
        RequiredTimeDropDown.value = 0;
        SolutionsTypesDropDown.value = 0;

        StartCoroutine(InquiryFormPanelViewEntryEffect());
    }

    private IEnumerator InquiryFormPanelViewEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator InquiryFormPanelViewExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }




    public void DisableInquiryFormPanelPanelView()
    {
        StartCoroutine(InquiryFormPanelViewExitEffect());
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
        DisableInquiryFormPanelPanelView();
    }
    private void Start()
    {
        BudgetDropDown.onValueChanged.AddListener(delegate { SelectBudget(BudgetDropDown.value);});
        RequiredTimeDropDown.onValueChanged.AddListener(delegate { SelectTime(RequiredTimeDropDown.value);});
        SolutionsTypesDropDown.onValueChanged.AddListener(delegate { SelectSolution(SolutionsTypesDropDown.value);});
        SubmitBtn.onClick.AddListener(OnSubmit);
        backBtn.onClick.AddListener(() =>
        {
            StartCoroutine(InquiryFormPanelViewExitEffect());
        });
    }
    void SelectBudget(int value) {

        SelectedBudget = BudgetDropDown.options[value].text.ToString();
    }

    void SelectTime(int value) {

        SelectedTime = RequiredTimeDropDown.options[value].text.ToString();
    }

    void SelectSolution(int value) {

        SelectedSolution = SolutionsTypesDropDown.options[value].text.ToString();
    }

    void OnSubmit()
    {

        if(ValidateFields())
        StartCoroutine(SubmitClientForm());
        
       // ResetFields();
    }
    void ResetFields() {

        Name.text = "";
        Mobile.text = "";
        Email.text = "";
        WhatsAppNum.text = "";
        MoreDetails.text = "";
        BudgetDropDown.value = 0;
        RequiredTimeDropDown.value = 0;
        SolutionsTypesDropDown.value = 0;
    }
    bool ValidateFields() {

        if (Mobile.text.Length < 10 || WhatsAppNum.text.Length < 10) {
            ServerManager.instance.alertPopUp.ShowView("Please Enter valid Phone No ");
            return false;
        }
        else if (!Email.text.Contains("@")) {
            ServerManager.instance.alertPopUp.ShowView("Please Enter valid Email Address ");
            return false;

        }
        

        return true;    
    }
    public IEnumerator SubmitClientForm()
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.name, Name.text);
        form.AddField(Constants.KEYS.email, Email.text);
        form.AddField(Constants.KEYS.mobile, Mobile.text);
        form.AddField(Constants.KEYS.whats_app_number, WhatsAppNum.text);
        form.AddField(Constants.KEYS.budget, SelectedBudget);
        form.AddField(Constants.KEYS.how_soon_required, SelectedTime);
        form.AddField(Constants.KEYS.type_of_solutions, SelectedSolution);
        form.AddField(Constants.KEYS.captcha, "moon_rummy_4556465");
        form.AddField(Constants.KEYS.message, MoreDetails.text);

        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.ENQUIRY_FORM, form))
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
                var response = (Response<UserModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<UserModel>));
                Debug.Log(www.downloadHandler.text);
                if (response.status == Constants.KEYS.valid)
                {
                    ResetFields();
                    DisableInquiryFormPanelPanelView();
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
