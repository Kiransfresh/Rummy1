using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class KYCPanelView : MonoBehaviour,IActivePanel

{
    [Header("Animation")]
    [SerializeField] private SlidingEffect slidingEffect;

    [Header("Gallery Picker")]
    [SerializeField] private SunshineNativeGalleryHandler _sunshineScript;


    [Header("Header buttons")]
    [SerializeField] private Button backBtn;


    [Header("Input fields")]
    [SerializeField] private TMP_InputField aadharField;
    [SerializeField] private TMP_InputField panField;
    [SerializeField] private TextMeshProUGUI documentDisplayText;

    [Header("Validation game objects")]
    [SerializeField] private GameObject verifyObject;
    [SerializeField] private GameObject notVerifyObject;
    [SerializeField] private TextMeshProUGUI verifyText;

    [Header("Upload Documents Buttons")]
    [SerializeField] private Button aadharFrontBtn;
    [SerializeField] private Button aadharBackBtn;
    [SerializeField] private Button panCardBtn;


    [Header("Footer buttons")]
    [SerializeField] private Button submitBtn;


   


    #region PRIVATE_VARS
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    private Texture2D aadhar_front_image;
    private Texture2D aadhar_back_image;
    private Texture2D pan_card_image;
    #endregion

    #region UNITY_CALLBACKS
    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() =>
        {
            StartCoroutine(KYCPanelViewExitEffect());
        });

        submitBtn.onClick.AddListener(() =>
        {
            ServerManager.instance.loader.ShowLoader("");
            StartCoroutine(APIManager.instance.AddKYC(aadharField.text, panField.text, aadhar_front_image, aadhar_back_image, pan_card_image, AddKYCCallback));
        });

        aadharFrontBtn.onClick.AddListener(()=> {
            OpenGallery(type: "aadhar_front");
        });

        aadharBackBtn.onClick.AddListener(() => {
            OpenGallery(type: "aadhar_back");
        });

        panCardBtn.onClick.AddListener(() => {
            OpenGallery(type: "pan_card");
        });
    }

    private void OnEnable()
    {
        StartCoroutine(KYCPanelViewEntryEffect());

        ServerManager.instance.loader.ShowLoader("Fetching...");
        StartCoroutine(APIManager.instance.FetchKYC(FetchKYCCallback));
        //string latitude = LocationManager.instance.latitude;
        //string longitude = LocationManager.instance.longitude;
        //if (latitude == null || longitude == null)
        //{
        //    LocationManager.instance.CheckForLocation();
        //}
    }

    private void FetchKYCCallback(Response<KYCModel> response) {
        ServerManager.instance.loader.HideLoader();
        if (response.status == Constants.KEYS.valid)
        {
            
            aadharField.text = response.data.aadhar;
            panField.text = response.data.pan;

            if (string.IsNullOrEmpty(response.data.aadhar_front_image) == false) {
                StartCoroutine(DownloadImage(aadharFrontBtn.transform.GetChild(1).GetChild(0).GetComponent<RawImage>(), response.data.aadhar_front_image));
            }

            if (string.IsNullOrEmpty(response.data.aadhar_back_image) == false)
            {
                StartCoroutine(DownloadImage(aadharBackBtn.transform.GetChild(1).GetChild(0).GetComponent<RawImage>(), response.data.aadhar_back_image));
            }
           

            if (string.IsNullOrEmpty(response.data.pan_card_image) == false)
            {
                StartCoroutine(DownloadImage(panCardBtn.transform.GetChild(1).GetChild(0).GetComponent<RawImage>(), response.data.pan_card_image));
                documentDisplayText.gameObject.SetActive(false);
            }
            else
            {
                documentDisplayText.gameObject.SetActive(true);
            }
            KYCVerifyValidationUI(response.data.verify_kyc == "1");
        }
        else
        {
            documentDisplayText.gameObject.SetActive(true);
            ServerManager.instance.alertPopUp.ShowView(response.message);
            UIManager.instance.accountMenuView.kycPanelView.KYCVerifyValidationUI(false);
        }
    }

    private void AddKYCCallback(Response<object> response) {
        ServerManager.instance.loader.HideLoader();
        if (response.status == Constants.KEYS.valid)
        {
            ServerManager.instance.alertPopUp.ShowView(response.message);
        }
        else
        {
            ServerManager.instance.alertPopUp.ShowView(response.message);
        }
    }

    #endregion

    

    #region CO-ROUTINES
    private IEnumerator KYCPanelViewEntryEffect()
    {
        yield return startDelay;
        StartCoroutine(slidingEffect.EntryEffect());
    }

    private IEnumerator KYCPanelViewExitEffect()
    {
        StartCoroutine(slidingEffect.ExitEffect());
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    #endregion

    #region UI_CALLBACKS

    public void DisableKYCPanelView()
    {
        StartCoroutine(KYCPanelViewExitEffect());
    }

    public void DeactivatePanel()
    {
        DisableKYCPanelView();
    }

    public void KYCVerifyValidationUI(bool isVerify)
    {
        if (isVerify)
        {
            notVerifyObject.SetActive(false);
            verifyObject.SetActive(true);
            verifyText.text = "Verified";
            aadharField.interactable = false;
            panField.interactable = false;

            aadharFrontBtn.interactable = false;
            aadharBackBtn.interactable = false;
            panCardBtn.interactable = false;
            submitBtn.interactable = false;
        }
        else
        {
            verifyObject.SetActive(false);
            notVerifyObject.SetActive(true);
            verifyText.text = "Not Verified";


            aadharField.interactable = true;
            panField.interactable = true;

            aadharFrontBtn.interactable = true;
            aadharBackBtn.interactable = true;
            panCardBtn.interactable = true;
            submitBtn.interactable = true;
        }
    }

    public void OpenGallery(string type)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            StorageManager.instance.permissionAllowed = (status) =>
            {
                OpenSunShineGallery(type);
            };
            StorageManager.instance.CheckForPermission();
        }
        
    }


    private void OpenSunShineGallery(string type)
    {
        _sunshineScript.OpenGallery((bool success, string[] paths) =>
        {
            if (success)
            {
                var readableTexture = LoadJPG(paths[0]);
                if (type == "aadhar_front")
                {
                    aadhar_front_image = readableTexture;
                    aadharFrontBtn.transform.GetChild(1).GetChild(0).GetComponent<RawImage>().texture = readableTexture;
                }
                else if (type == "aadhar_back")
                {
                    aadhar_back_image = readableTexture;
                    aadharBackBtn.transform.GetChild(1).GetChild(0).GetComponent<RawImage>().texture = readableTexture;
                }
                else if (type == "pan_card")
                {
                    pan_card_image = readableTexture;
                    panCardBtn.transform.GetChild(1).GetChild(0).GetComponent<RawImage>().texture = readableTexture;
                    documentDisplayText.gameObject.SetActive(true);
                }
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
            tex = new Texture2D(800, 400);
            tex.LoadImage(fileData);
        }
        return tex;
    }


    private IEnumerator DownloadImage(RawImage rawImage, string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
            
    }

    #endregion
}
