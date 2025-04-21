using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfilePanelView : MonoBehaviour, IActivePanel
{
    [Header("Animation")]
    [SerializeField] private SlidingEffect slidingEffect;

    [Header("Header buttons")]
    [SerializeField] private Button backBtn;

    [Header("Input fields")]
    [SerializeField] private TMP_InputField firstName;
    [SerializeField] private TMP_InputField lastName;
    [SerializeField] private TMP_InputField mobileNumber;
    [SerializeField] private TMP_InputField emailAddress;
    [SerializeField] private TMP_InputField addressField;

    [Header("Gender toggles")]
    [SerializeField] private Toggle maleToggle;
    [SerializeField] private Toggle femaleToggle;
    [SerializeField] private Toggle transgenderToggle;

    [Header("Profile Image")]
    [SerializeField] public Image profileImage;


    [Header("Buttons For performing actions")]
    [SerializeField] private Button profileUpdateBtn;
    [SerializeField] private Button updateAvatarBtn;
    [SerializeField] private Button updateUserName;


    [Header("Verification text")]
    [SerializeField] private TextMeshProUGUI mobileNumberVerificationText;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI usernameText;






    #region PRIVATE_VARS
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    private string gender;
    private UserModel profiledata;
    #endregion

    #region UNITY_CALLBACKS
    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);


    }
    private void Start()
    {
        profileUpdateBtn.onClick.AddListener(OnUpdateProfile);

        updateAvatarBtn.onClick.AddListener(() => {
            UIManager.instance.usernameupdateView.gameObject.SetActive(true);
            UIManager.instance.usernameupdateView.UpgradeOnlyAvatar();
        });

        updateUserName.onClick.AddListener(() => {
            UIManager.instance.usernameupdateView.gameObject.SetActive(true);
            UIManager.instance.usernameupdateView.UpgradeOnlyUsername(profiledata.unique_name, UpdatedUsernameCallback);
        });
        backBtn.onClick.AddListener(() => { DisableProfilePanelView(); });
    }
    private void OnEnable()
    {
        backBtn.gameObject.SetActive(true);
        ServerManager.instance.loader.ShowLoader("Fetching");
        StartCoroutine(APIManager.instance.GetProfileDetails(GetProfileCallback));

        StartCoroutine(ProfilePanelViewEntryEffect());
        mobileNumber.interactable = false;
    }


    private void UpdatedUsernameCallback(string newUsername)
    {
        if (!string.IsNullOrEmpty(newUsername))
        {
            profiledata.unique_name = newUsername;
            usernameText.text = newUsername;
        }
    }


    #endregion

    #region PRIVATE_FUNCTION

    private void GetProfileCallback(Response<UserModel> response)
    {
        ServerManager.instance.loader.HideLoader();
        if (response != null && response.status == Constants.KEYS.valid)
        {
            if (response.data.mobile_verified == "1")
            {
                mobileNumberVerificationText.transform.GetChild(0).gameObject.SetActive(false);
                mobileNumberVerificationText.transform.GetChild(1).gameObject.SetActive(true);
                mobileNumberVerificationText.text = "Verified";
                mobileNumberVerificationText.color = new Color32(12, 255, 36, 255);
            }
            else
            {
                mobileNumberVerificationText.transform.GetChild(1).gameObject.SetActive(false);
                mobileNumberVerificationText.transform.GetChild(0).gameObject.SetActive(true);
                mobileNumberVerificationText.text = "Verify";
                mobileNumberVerificationText.color = new Color32(255, 12, 12, 255);
            }
            profiledata = response.data;
            SetprofileData();
        }
        else
        {
            ServerManager.instance.alertPopUp.ShowView(response.message);
        }
    }






    #endregion

    #region CO-ROUTINES
    private IEnumerator ProfilePanelViewEntryEffect()
    {
        yield return startDelay;
        StartCoroutine(slidingEffect.EntryEffect());
    }

    private IEnumerator ProfilePanelViewExitEffect()
    {
        StartCoroutine(slidingEffect.ExitEffect());
        yield return disableDelay;
        gameObject.SetActive(false);
    }





    #endregion

    #region UI_CALLBACKS

    public void DisableProfilePanelView()
    {
        StartCoroutine(ProfilePanelViewExitEffect());
    }

    public void DeactivatePanel()
    {
        DisableProfilePanelView();
    }


    #endregion


    private void OnUpdateProfile()
    {

        if (maleToggle.isOn)
        {
            gender = "Male";
        }
        else if (femaleToggle.isOn)
        {
            gender = "Female";
        }
        else
        {
            gender = "Transgender";
        }

        if (mobileNumber.text.Length < 10)
        {
            ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.ENTER_VALID_INFO);
        }
        else if (emailAddress.text.Contains("@") == false || emailAddress.text.Contains(".") == false)
        {
            ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.ENTER_VALID_EMAIL);
        }
        else
        {
            StartCoroutine(UpdateProfileDetails());
            /*if (profiledata != null && profiledata.email != emailAddress.text)
            {
                StartCoroutine(UpdateEmailDetails());
            }*/
        }
    }
    public void Emailnotverified()
    {
        backBtn.gameObject.SetActive(false);
    }





    IEnumerator UpdateProfileDetails()
    {

        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField(Constants.KEYS.gender, gender);
        form.AddField(Constants.KEYS.email, emailAddress.text);

        ServerManager.instance.loader.ShowLoader("Updating...");

        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.UPDATE_PROFILE, form))
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
                ServerManager.instance.loader.HideLoader();
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<SubResponse>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<SubResponse>));
                ServerManager.instance.loader.HideLoader();
                if (response.status == Constants.KEYS.valid)
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                    DisableProfilePanelView();
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
            }
        }
    }


    IEnumerator UpdateEmailDetails()
    {

        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField(Constants.KEYS.new_email, emailAddress.text);

        ServerManager.instance.loader.ShowLoader("Updating...");

        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.UPDATE_EMAIL, form))
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
            ServerManager.instance.loader.HideLoader();

            if (www.result != UnityWebRequest.Result.Success)
            {

                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<SubResponse>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<SubResponse>));
                if (response.status == Constants.KEYS.valid)
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                    DisableProfilePanelView();
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
            }
        }
    }

    private void SetprofileData()
    {
        firstName.text = profiledata.first_name;
        lastName.text = profiledata.last_name;
        mobileNumber.text = profiledata.mobile_number;
        emailAddress.text = profiledata.email;
        addressField.text = profiledata.date_of_birth;
        usernameText.text = profiledata.unique_name;

        if (profiledata.gender.Equals("Male"))
        {
            maleToggle.isOn = true;
        }
        else if (profiledata.gender.Equals("Female"))
        {
            femaleToggle.isOn = true;
        }
        else
        {
            transgenderToggle.isOn = true;
        }

        //if (profiledata.avatar_full_path.Length > 0 && gameObject.activeInHierarchy)
        //    StartCoroutine(APIManager.instance.SetImages(profileImage, profiledata.avatar_full_path));

        if (gameObject.activeInHierarchy)
        {
            UIManager.instance.lobbyView.SetUserAvatarLocally(profileImage);
        }
    }

    public void OnEditField(TMP_InputField field)
    {

        field.interactable = true;
        field.Select();

    }
}
