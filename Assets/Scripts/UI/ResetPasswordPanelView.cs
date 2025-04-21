using System.Collections;
using UnityEngine;

public class ResetPasswordPanelView : MonoBehaviour
{
    #region PUBLIC_VARS
    public ScalingEffect[] scalingEffect;
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
        StartCoroutine(ResetPasswordEntryEffect());
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
        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].ExitEffect());
        }
    }

    #endregion

    #region CO-ROUTINES
    private IEnumerator ResetPasswordEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator ResetPasswordExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
      //  UIManager.instance.alertView.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    #endregion

    #region UI_CALLBACKS

    public void EnableAlertPanel()
    {
        StartCoroutine(ResetPasswordExitEffect());
    }
    #endregion
}
