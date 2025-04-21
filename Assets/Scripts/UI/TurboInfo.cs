using System.Collections;
using UnityEngine;

public class TurboInfo : MonoBehaviour
{
    #region PUBLIC_VARS
    public ScalingEffect[] scalingEffect;
    public FadingColorEffect fadingColorEffect;
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
        StartCoroutine(TurboPanelEntry());
    }
    #endregion

    #region PRIVATE_FUNCTION

    private void PlayStartEffects()
    {
        StartCoroutine(fadingColorEffect.ChangeColor());
        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].EntryEffect());
        }
    }

    private void PlayEndEffect()
    {
        StartCoroutine(fadingColorEffect.SetInitialColor());
        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].ExitEffect());
        }
    }

    #endregion

    #region CO-ROUTINES
    private IEnumerator TurboPanelEntry()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator TurboPanelExit()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    #endregion

    #region UI_CALLBACKS

    public void DisableTurboPanel()
    {
        StartCoroutine(TurboPanelExit());
    }

    #endregion
}
