using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangePasswordPanelView : MonoBehaviour,IActivePanel
{
    #region PUBLIC_VARS
    public SlidingEffect[] slidingEffect;
    public Button closeBtn;
    public Button confirmBtn;
    public TMP_InputField currentPasswordField;
    public TMP_InputField newPasswordField;
    public TMP_InputField confirmPasswordField;
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
        closeBtn.onClick.AddListener(() =>
        {
            DisableChangePasswordPanelView();
        });

        confirmBtn.onClick.AddListener(() =>
        {
          StartCoroutine(APIManager.instance.ChangePassword(currentPasswordField.text, newPasswordField.text,
                confirmPasswordField.text,
                () =>
                {
                    DisableChangePasswordPanelView();
                }));
        });
    }

    private void OnEnable()
    {
        StartCoroutine(ChangePasswordPanelViewEntryEffect());
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

    private void PlayEndEffect()
    {
        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].ExitEffect());
        }
    }
    #endregion

    #region CO-ROUTINES
    private IEnumerator ChangePasswordPanelViewEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator ChangePasswordPanelViewExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    #endregion

    #region UI_CALLBACKS

    public void DisableChangePasswordPanelView()
    {
        StartCoroutine(ChangePasswordPanelViewExitEffect());
    }

    public void DeactivatePanel()
    {
        DisableChangePasswordPanelView();
    }

    #endregion
}
