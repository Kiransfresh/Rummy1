using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SharePrivateTableCode : MonoBehaviour
{
    [SerializeField] private Button shareCodeBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button copyCodeBtn;
    [SerializeField] private ShareMessage shareMessage;
    [SerializeField ]private TextMeshProUGUI tableIdCode;
    private string message;

    private void OnEnable() 
    {
        tableIdCode.text = "Table Code: " + CacheMemory.privateTableCode;
        message = "Come, lets play rummy. Join our private table using this Table ID " +
                CacheMemory.privateTableCode +"\n"+ Constants.SERVER_DETAILS.Mobile_Url;
    }

    private void Start() 
    {
        shareCodeBtn.onClick.AddListener(() =>
        {
#if !UNITY_EDITOR
            shareMessage.OnAndroidTextSharingClick(message);
#endif
        });

        closeBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        copyCodeBtn.onClick.AddListener(() =>
        {
            CopyCodeIntoClipboard(CacheMemory.privateTableCode);
        });
    }

    private void CopyCodeIntoClipboard(string referralCode)
    {
        var textEditor = new TextEditor();
        textEditor.text = referralCode;
        textEditor.SelectAll();
        textEditor.Copy();
        SSTools.ShowMessage("copied", SSTools.Position.bottom, SSTools.Time.oneSecond);
        SSTools.FindObjectOfType<Canvas>().sortingOrder = 10;
    }
}


