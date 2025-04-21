using System.Collections;
using SignInSample;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LoginView : MonoBehaviour
{
    #region PUBLIC_VARS
    [HideInInspector] public GoogleLogin googleLogin;
    public SlidingEffect[] slidingEffect;
    public GameObject ForgotPasswordPanel;
    public GameObject OTPVerificationPanel;
    [HideInInspector]  public TMP_InputField usernameField;
    [HideInInspector]  public TMP_InputField passwordField;
    public TMP_InputField mobileNumberField;
    public Button loginBtn;
    public Toggle termsPrivacyCheckBox;
    [HideInInspector] public Button gmailLoginBtn;
    #endregion

    #region PRIVATE_VARS
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    #endregion

    [Header("Buttons")]
    [SerializeField] private Button termsAndConditionBtn;
    [SerializeField] private Button privacyPolicyBtn;

    public ReadOnlyPanel readOnlyPanel;

    #region UNITY_CALLBACKS
    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void Start()
    {
        termsAndConditionBtn.onClick.AddListener(OpenTermsWebURL);
        privacyPolicyBtn.onClick.AddListener(OpenPrivacyPolicyWebURL);
        loginBtn.onClick.AddListener(() =>
        {
            LoginWithMobileNumber();
        });
    }

    public void OpenTermsWebURL()
    {
        var url = Constants.SERVER_DETAILS.Mobile_Url + "terms_of_service";
        Application.OpenURL(url);
    }

    public void OpenPrivacyPolicyWebURL()
    {
        var url = Constants.SERVER_DETAILS.Mobile_Url + "privacy_policy";
        Application.OpenURL(url);
    }

    private void OnEnable()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        StartCoroutine(LoginPanelEntryEffect());
    }

    private void OnDisable()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
    #endregion

    #region PRIVATE_FUNCTION

    private void PlayStartEffects()
    {
        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].EntryEffect());
        }
    }

    public void PlayEndEffect()
    {
        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].ExitEffect());
        }
    }

    #endregion

    #region CO-ROUTINES
    private IEnumerator LoginPanelEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator LoginPanelExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    #endregion

    #region UI_CALLBACKS

    public void EnableForgotPasswordPanel()
    {
        ForgotPasswordPanel.SetActive(true);
        UIManager.instance.forgotPasswordView.gameObject.SetActive(true);
    }

    public void EnableOTPVerificationPanel()
    {
        OTPVerificationPanel.SetActive(true);
        UIManager.instance.verificationPopUpView.gameObject.SetActive(true);
    }

    public void EnableRegisterPanel()
    {
        StartCoroutine(LoginPanelExitEffect());
        UIManager.instance.registerView.gameObject.SetActive(true);
    }

    // Cash Games
    public void Login()
    {
        StartCoroutine(APIManager.instance.Login(usernameField.text, passwordField.text, () =>
        {
            UIManager.instance.verificationPopUpView.gameObject.SetActive(true);
        }));
    }

    // while only free games
    public void LoginWithMobileNumber()
    {
        if(string.IsNullOrEmpty(mobileNumberField.text))
        {
            ServerManager.instance.alertPopUp.ShowView("Please enter mobile number");
        }
        else if (mobileNumberField.text.Length != 10)
        {
            ServerManager.instance.alertPopUp.ShowView("Make sure the mobile number need to be 10 digits");
        }
        else if (!termsPrivacyCheckBox.isOn)
        {
            ServerManager.instance.alertPopUp.ShowView("Please accept terms and privacy policy");
        }
        else
        {
            StartCoroutine(APIManager.instance.LoginWithMobileNumber(mobileNumberField.text));
        }
    }
    #endregion
}
