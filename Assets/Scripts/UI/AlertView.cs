using System.Collections;
using UnityEngine;

public class AlertView : MonoBehaviour
{
    #region PUBLIC_VARS
    public FadingColorEffect[] fadingColorEffect;
    public ScalingEffect[] scalingEffect;
    public GameObject ForgotPasswordPanel;
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
        StartCoroutine(AlertViewEntryEffect());
    }
    #endregion

    #region PRIVATE_FUNCTION

    private void PlayStartEffects()
    {
        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].EntryEffect());
        }
    }

    private void PlayEndEffect()
    {
        for (int i = 0; i < fadingColorEffect.Length; i++)
        {
            StartCoroutine(fadingColorEffect[i].SetInitialColor());
        }

        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].ExitEffect());
        }
    }

    #endregion

    #region CO-ROUTINES
    private IEnumerator AlertViewEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator AlertViewExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
        ForgotPasswordPanel.SetActive(false);
    }

    #endregion

    #region UI_CALLBACKS

    public void DisableAlertPanel()
    {
        StartCoroutine(AlertViewExitEffect());
    }

    #endregion
}
