using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegisterView : MonoBehaviour
{
    #region PUBLIC_VARS
    public SlidingEffect[] slidingEffect;
    public TMP_InputField usernameField;
    public TMP_InputField mobileNumberField;
    public TMP_InputField passwordField;
    public TMP_InputField referralCodeField;
    public Button registerBtn;
    public Button gmailLoginBtn;
    #endregion

    #region PRIVATE_VARS
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    #endregion

    #region UNITY_CALLBACKS
    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void Start()
    {
        
        registerBtn.onClick.AddListener(() =>
        {
            Register();
        });

        gmailLoginBtn.onClick.AddListener(() =>
        {
            UIManager.instance.loginView.googleLogin.OnSignIn();
        });
    }

    private void OnEnable()
    {
        StartCoroutine(RegisterPanelEntryEffect());
    }
    #endregion

    #region PRIVATE_FUNCTION

    public void PlayStartEffects()
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
    private IEnumerator RegisterPanelEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator RegisterPanelExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        UIManager.instance.loginView.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    #endregion

    #region UI_CALLBACKS

    public void EnableLoginPanel()
    {
        StartCoroutine(RegisterPanelExitEffect());
    }

    public void EnableVerificationPanel()
    {
        UIManager.instance.verificationPopUpView.gameObject.SetActive(true);
    }

    private void Register()
    {
        StartCoroutine(APIManager.instance.Register(usernameField.text, passwordField.text, mobileNumberField.text, referralCodeField.text,  () =>
        {
            Debug.Log("Write the code to verify mobile number");
            EnableVerificationPanel();
            StartCoroutine(APIManager.instance.SendOTP(mobileNumberField.text, UIManager.instance.verificationPopUpView.messageText));
            Debug.Log("Mobile Number is: " + mobileNumberField.text + " Message Text is: " +
                      UIManager.instance.verificationPopUpView.messageText.text);
        }));
    }

    #endregion
}

