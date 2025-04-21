using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ForgotPasswordView : MonoBehaviour
{
    #region PUBLIC_VARS
    public FadingColorEffect[] fadingColorEffect;
    public ScalingEffect[] scalingEffect;
    public Button closeBtn;
    public TMP_InputField usernameField;
    public Button submitBtn;
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

    private void OnEnable()
    {
        StartCoroutine(ForgotPasswordEntryEffect());
    }

    private void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            StartCoroutine(ForgotPasswordExitEffect());
        });

        submitBtn.onClick.AddListener(() =>
        {
            StartCoroutine(APIManager.instance.ForgotPassword(usernameField.text, () =>
            {
                StartCoroutine(ForgotPasswordExitEffect());
            }));
        });
        
    }

    #endregion

    #region PRIVATE_FUNCTION

    private void PlayStartEffects()
    {
        for (int i = 0; i < fadingColorEffect.Length; i++)
        {
            StartCoroutine(fadingColorEffect[i].ChangeColor());
        }

        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].EntryEffect());
        }
    }

    private void PlayEndEffect()
    {
        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].ExitEffect());
        }
    }

    #endregion

    #region CO-ROUTINES
    private IEnumerator ForgotPasswordEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator ForgotPasswordExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    #endregion

    #region UI_CALLBACKS

    public void EnableForgotPasswordOTPPanel()
    {
        StartCoroutine(ForgotPasswordExitEffect());
    }
    #endregion
}
