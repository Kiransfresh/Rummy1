using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UpgradPopUp : MonoBehaviour
{
    public ScalingEffect OverlayScaling, PanelScaling;
    public FadingColorEffect fadingColorEffect;

    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    public Button UpgradeNowbtn;
    public TextMeshProUGUI Versiontext;

    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
        UpgradeNowbtn.onClick.AddListener(OnUpgradeNow);
    }
    private void OnEnable()
    {
        Versiontext.text = "Current Version - " + Application.version;
        StartCoroutine(EnablePopUp());
    }
    private void PlayStartEffects()
    {
        StartCoroutine(fadingColorEffect.ChangeColor());
        StartCoroutine(OverlayScaling.EntryEffect());
        StartCoroutine(PanelScaling.EntryEffect());

    }

    public void PlayEndEffect()
    {
        StartCoroutine(fadingColorEffect.SetInitialColor());
        StartCoroutine(OverlayScaling.ExitEffect());
        StartCoroutine(PanelScaling.ExitEffect());
    }
    private IEnumerator EnablePopUp()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator DisablePopUp()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    public void DisableUpgradepanelView()
    {

        StartCoroutine(DisablePopUp());
    }

    void OnUpgradeNow()
    {

        Application.OpenURL(Constants.SERVER_DETAILS.APK_LINK);
        DisableUpgradepanelView();
    }

}
