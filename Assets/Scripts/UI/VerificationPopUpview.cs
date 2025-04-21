using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VerificationPopUpview : MonoBehaviour
{
    public ScalingEffect[] scalingEffect;
    public TMP_InputField OTPField;
    public TextMeshProUGUI messageText;
    public Button submitBtn;
    public Button resendOTP;

    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    public bool isGoogleSignIn = false;


    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        OTPField.text = "";
        StartCoroutine(VerificationPanelEntry());
    }

    private void Start()
    {
        submitBtn.onClick.AddListener(() =>
        {
            StartCoroutine(APIManager.instance.VerifyMobileNumber(CacheMemory.MobileNumber, OTPField.text));
        });

        resendOTP.onClick.AddListener(() =>
        {
            Debug.Log("Mobile number at resend otp : "  + CacheMemory.MobileNumber);
            StartCoroutine(APIManager.instance.LoginWithMobileNumber(CacheMemory.MobileNumber, ResendOTPCallBack));
        });
    }

    private void ResendOTPCallBack(Response<SubResponse> response) {
        if (response != null) {
            ServerManager.instance.alertPopUp.ShowView(response.message);
        }
        
    }

    private void PlayStartEffects()
    {
        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].EntryEffect());
        }
    }

    public void PlayEndEffect()
    {
        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].ExitEffect());
        }
    }


    private IEnumerator VerificationPanelEntry()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator VerificationPanelExit()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }


    public void DisableVerificationPanel()
    {
        StartCoroutine(VerificationPanelExit());
    }

}
