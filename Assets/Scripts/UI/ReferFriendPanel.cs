using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReferFriendPanel : MonoBehaviour
{
    public SlidingEffect[] slidingEffect;

    
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    [Header("Buttons")]
    [SerializeField] private Button backBtn;
    [SerializeField] private Button copyReferCodeBtn;
    [SerializeField] private Button referFriend;

    [Header("Refferal Code")]
    [SerializeField] private TextMeshProUGUI refferalCode;

    [Header("Share Message Script")]
    [SerializeField] private ShareMessage shareMessage;

    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
#if !UNITY_WEBGL
        StartCoroutine(APIManager.instance.MessageToSend());
#endif
        refferalCode.text = PlayerPrefsManager.GetReferralCode();
        StartCoroutine(ReferFriendEntryEffect());
    }

    private void Start()
    {
        referFriend.onClick.AddListener(() =>
        {
#if !UNITY_EDITOR
            shareMessage.OnAndroidTextSharingClick(CacheMemory.ReferAFriend);
#endif
        });

        copyReferCodeBtn.onClick.AddListener(() =>
        {
            CopyCodeIntoClipboard(refferalCode.text);
        });

        backBtn.onClick.AddListener(() =>
        {
            StartCoroutine(ReferFriendExitEffect());
        });
    }

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

    public void DisableReferFriend()
    {
        StartCoroutine(ReferFriendExitEffect());
    }

    private IEnumerator ReferFriendEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    public IEnumerator ReferFriendExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    private void CopyCodeIntoClipboard(string referralCode)
    {
        var textEditor = new TextEditor();
        textEditor.text = referralCode;
        textEditor.SelectAll();
        textEditor.Copy();
        SSTools.ShowMessage("Copied Code!", SSTools.Position.bottom, SSTools.Time.oneSecond);
    }
}
