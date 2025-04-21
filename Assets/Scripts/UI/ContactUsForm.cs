using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContactUsForm : MonoBehaviour,IActivePanel
{

    [Header("Animations")]
    [SerializeField] private SlidingEffect[] slidingEffect;

    [Header("Category Text")]
    [SerializeField] private TextMeshProUGUI categoryText;

    [Header("Message Input fields")]
    [SerializeField] private TMP_InputField messageField;


    [Header("Buttons")]
    [SerializeField] private Button SubmitBtn;
    [SerializeField] private Button backBtn;

    #region PRIVATE_VARIABLES
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    #endregion

    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        StartCoroutine(ContactUsPanelViewEntryEffect());
    }

    private void Start()
    {
        SubmitBtn.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(messageField.text))
            {
                ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.CONTACT_US_ERROR);
                
            }
            else
            {
                ServerManager.instance.loader.ShowLoader("Submitting...");
                StartCoroutine(APIManager.instance.ContactUsForm(categoryText.text, messageField.text));
                messageField.text = "";
            }

           
        });
        backBtn.onClick.AddListener(() =>
        {
            DisableContactUsPanelView();
        });
    }

    private IEnumerator ContactUsPanelViewEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator ContactUsPanelViewExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }




    public void DisableContactUsPanelView()
    {
        StartCoroutine(ContactUsPanelViewExitEffect());
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

    public void DeactivatePanel()
    {
        DisableContactUsPanelView();
    }

    
}
