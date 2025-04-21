using Kakera;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankStatementRequestPanelView : MonoBehaviour, IActivePanel
{
    [SerializeField]
    SunshineNativeGalleryHandler _sunshineScript;

    public SlidingEffect[] slidingEffect;

    [Header("Input Values")]
    [SerializeField] private TMP_InputField accountHolderName;
    [SerializeField] private TMP_InputField bankName;
    [SerializeField] private TMP_InputField accountNumber;
    [SerializeField] private TMP_InputField ifscCode;
    [SerializeField] private TMP_InputField branchName;

    [Header("Buttons for click actions")]
    [SerializeField] private Button bankProof;
    [SerializeField] private Button uploadBtn;
    [SerializeField] private Button submitBtn;

    [Header("Gallery Picker")]
    [SerializeField] private PickerController pickerController;

    private Texture2D deposit_attachment;
    public TextMeshProUGUI bankdetailsTxt;

    #region PRIVATE_VARS
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    private BankDetailsModel detailsModel;
    private string bankProofFilePath = null;
    #endregion

    #region UNITY_CALLBACKS
    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void Start()
    {
       /* uploadBtn.onClick.AddListener(() =>
        {
           // OpenGallery();
        }); 

        bankProof.onClick.AddListener(() =>
        {
            if (detailsModel != null && detailsModel.bank_proof != null && detailsModel.bank_proof != "")
            {
                Application.OpenURL(detailsModel.bank_proof);
            }
            else {
                ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.BANK_PRROF_WARNING);
            }
        });*/

        submitBtn.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(accountHolderName.text)) {
                ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.ENTER_ACCOUNT_HOLDER);
            }
            else if (string.IsNullOrEmpty(accountNumber.text))
            {
                ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.ENTER_ACCOUNT_NUMBER);
            }
            else if (string.IsNullOrEmpty(ifscCode.text))
            {
                ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.ENTER_IFSC);
            }
            else
            {
                deposit_attachment = pickerController.bankProofImage.sprite.texture;
                StartCoroutine(APIManager.instance.AddBankDetails(accountNumber.text, accountHolderName.text, ifscCode.text,
                    bankName.text, branchName.text));
            }
        });
    }

    private void OnEnable()
    {
        ServerManager.instance.loader.ShowLoader("Fetching...");
        StartCoroutine(APIManager.instance.FetchBankDetails(SetBankDetails));
        StartCoroutine(BankStatementPanelViewEntryEffect());
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
    private IEnumerator BankStatementPanelViewEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator BankStatementPanelViewExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    #endregion

    #region UI_CALLBACKS

    public void DisableBankAccoutnPanelView()
    {
        StartCoroutine(BankStatementPanelViewExitEffect());
    }

    public void DeactivatePanel()
    {
        DisableBankAccoutnPanelView();
    }

    #endregion


  /*  public void OpenGallery()
    {
        _sunshineScript.OpenGallery((bool success, string[] paths) =>
        {
            if (success)
            {
                var readableTexture = LoadJPG(paths[0]);
                ServerManager.instance.loader.ShowLoader("Uploading...");
                StartCoroutine(APIManager.instance.UploadBankProof(readableTexture, UploadProofResponse));
            }
        });
    }

    public Texture2D LoadJPG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(512, 512);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

   /* private void UploadProofResponse(Response<BankProofUploadModel> response) {
        ServerManager.instance.loader.HideLoader();
        if (response.status == Constants.KEYS.valid)
        {
            bankProofFilePath = response.data.path;
            UIManager.instance.accountMenuView.bankStatementRequestPanelView.SetBankProof(response.data.full_path);
            ServerManager.instance.alertPopUp.ShowView(response.message);
        }
        else
        {
            ServerManager.instance.alertPopUp.ShowView(response.message);

        }
    } 

    private void SetBankProof(string proof_path) {
        if (detailsModel != null)
        {
            detailsModel.bank_proof = proof_path;
        }
        else {
            detailsModel = new BankDetailsModel();
            detailsModel.bank_proof = proof_path;
        }
    }
      */
    public void SetBankDetails(Response<BankDetailsModel> response)
    {
        ServerManager.instance.loader.HideLoader();
        if (response.status == Constants.KEYS.valid)
        {
            detailsModel = response.data;
            accountHolderName.text = detailsModel.account_holder_name;
            bankName.text = detailsModel.bank_name;
            accountNumber.text = detailsModel.account_number;
            ifscCode.text = detailsModel.ifsc_code;
            branchName.text = detailsModel.branch_name;
        }
        else
        {
            ServerManager.instance.alertPopUp.ShowView(response.message);
        }
    }
}
