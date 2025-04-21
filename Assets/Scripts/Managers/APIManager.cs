using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class APIManager : MonoBehaviour
{
    public static APIManager instance;

    private void Awake()
    {
        if (instance == null)
        {

            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
#if !UNITY_WEBGL 
        StartCoroutine(MessageToSend());
#endif
    }

    public void CheckVersion()
    {

        StartCoroutine(CheckForLatestVersion());
    }

    public void SendDeviceType(UnityWebRequest www)
    {
#if UNITY_ANDROID
        www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_android);
#elif UNITY_IPHONE
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_Ios);
#elif UNITY_WEBGL
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_Webgl);
#else
            www.SetRequestHeader(Constants.KEYS.requesting_source, "Website");
#endif
    }

    public IEnumerator GetProfileDetails(Action<Response<UserModel>> callback = null)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.PROFILE, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                callback?.Invoke(null);
            }
            else
            {
                Debug.Log("PROFILE RESPONSE - " + www.downloadHandler.text);
                var response = (Response<UserModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<UserModel>));
                callback?.Invoke(response);
                if (response.status == Constants.KEYS.valid)
                {
                    CacheMemory.IsGuestUser = response.data.guest_player;
                    CacheMemory.PlayerStatus = response.data.player_status;
                    PlayerPrefsManager.SetReferralCode(response.data.refer_code);
                    if (response.data.is_username_updated_on_first_signup == "0")
                    {
                        UIManager.instance.usernameupdateView.gameObject.SetActive(true);
                    }
                    if(response.data.email_verified == "0")
                    {
                        UIManager.instance.EmailVerificationProfilePage();
                    }
                }
            }
        }
    }

    public IEnumerator SetImages(Image avatarImg, string Avatarurl)
    {
        WWW www = new WWW(Avatarurl);
        yield return www;
        avatarImg.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }



    IEnumerator CheckForLatestVersion()
    {
        string url = Constants.API_METHODS.CHECK_VERSION + Application.version;
        Debug.Log(url);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("CHECK_VERSION " + www.downloadHandler.text);
                var response = (Response<SubResponse>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<SubResponse>));

                if (response.status == Constants.KEYS.valid)
                {

                    UIManager.instance.ShowUpgradePanel();
                }
            }
        }
    }

    public IEnumerator MessageToSend()
    {

        var form = new WWWForm();
        if (PlayerPrefsManager.GetAuthToken() != null)
            form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());

        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.REFER_FRIEND_MESSAGE, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<SubResponse>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<SubResponse>));
                if (response.status == Constants.KEYS.valid)
                {
                    CacheMemory.ReferAFriend =
                        response.message + " \nWhile SignUp, don't forget to enter referral code - "
                                         + PlayerPrefsManager.GetReferralCode();
                }
                else
                {
                    CacheMemory.ReferAFriend = response.message;
                }
            }
        }
    }

    public IEnumerator Register(string username, string password, string mobileNumber, string referralCode, Action verifyMobile)
    {
        var form = new WWWForm();
        form.AddField("username", username);
        form.AddField("mobile_number", mobileNumber);
        form.AddField("password", password);
        if (!string.IsNullOrEmpty(referralCode))
        {
            form.AddField("refer_code", referralCode);
        }
        else
        {
            form.AddField("refer_code", " ");
        }



        using (var www = UnityWebRequest.Post(Constants.API_METHODS.REGISTER, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<UserModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<UserModel>));
                if (response.status == Constants.KEYS.valid)
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                    PlayerPrefsManager.SetAuthToken(response.data.auth_token);

                    /*  if (response.data.mobile_verified == "1")
                      { */
                    UIManager.instance.verificationPopUpView.messageText.text = response.message;

                    if (PlayerPrefsManager.GetAuthToken() == null) yield break;
                    StartCoroutine(APIManager.instance.GetProfileDetails());
                    StartCoroutine(UIManager.instance.EnableLobby());

                    /*   }
                       else
                       {
                           verifyMobile.Invoke();
                       } */
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
            }

        }
    }

    public IEnumerator Login(string username, string password, Action verifyMobile)
    {
        var form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        ServerManager.instance.loader.ShowLoader("validating");

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.LOGIN, form))
        {

            SendDeviceType(www);

            yield return www.SendWebRequest();

            ServerManager.instance.loader.HideLoader();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<UserModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<UserModel>));
                if (response.status == Constants.KEYS.valid)
                {
                    PlayerPrefsManager.SetAuthToken(response.data.auth_token);
                    if (response.data.mobile_verified == "1")
                    {
                        UIManager.instance.verificationPopUpView.messageText.text = response.message;

                        if (PlayerPrefsManager.GetAuthToken() == null) yield break;
                        StartCoroutine(APIManager.instance.GetProfileDetails());
                        StartCoroutine(UIManager.instance.EnableLobby());

                    }
                    else
                    {
                        verifyMobile.Invoke();
                    }
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
            }

        }
    }

    // user this API in free games for both register and login
    public IEnumerator LoginWithMobileNumber(string mobileNumber, Action<Response<SubResponse>> callback = null)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.mobile_number, mobileNumber);
        CacheMemory.MobileNumber = mobileNumber;

        ServerManager.instance.loader.ShowLoader("Sending OTP...");
        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.LOGIN_WITH_MOBILE, form))
        {
            SendDeviceType(www);

            yield return www.SendWebRequest();

            ServerManager.instance.loader.HideLoader();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<SubResponse>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<SubResponse>));
                Debug.Log("Response is: " + www.downloadHandler.text);
                if (response.status == Constants.KEYS.valid)
                {
                    // ResponseText.text = response.data.message;
                    UIManager.instance.verificationPopUpView.messageText.text = response.data.message;
                    UIManager.instance.verificationPopUpView.gameObject.SetActive(true);
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }

                callback?.Invoke(response);
            }
        }
        //  ResponseText.text = "";
    }

    public IEnumerator EmailLogin(string email, Action verifyMobile)
    {
        var form = new WWWForm();
        form.AddField("email", email);

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.EMAIL_LOGIN, form))
        {

            SendDeviceType(www);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<UserModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<UserModel>));

                if (response.status == Constants.KEYS.valid)
                {
                    PlayerPrefsManager.SetAuthToken(response.data.auth_token);
                    if (response.data.mobile_verified == "1")
                    {
                        UIManager.instance.verificationPopUpView.messageText.text = response.message;

                        if (PlayerPrefsManager.GetAuthToken() == null) yield break;
                        StartCoroutine(APIManager.instance.GetProfileDetails());
                        StartCoroutine(UIManager.instance.EnableLobby());
                    }
                    else
                    {
                        verifyMobile.Invoke();
                    }
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
            }
        }
    }

    public IEnumerator SendOTP(string mobileNumber, TextMeshProUGUI message)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField(Constants.KEYS.mobile_number, mobileNumber);
        CacheMemory.MobileNumber = mobileNumber;
        message.text = "OTP sent to +91 " + "";
        ServerManager.instance.loader.ShowLoader("Sending OTP...");
        using (var www = UnityWebRequest.Post(Constants.API_METHODS.SEND_OTP, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();
            ServerManager.instance.loader.HideLoader();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                ServerManager.instance.alertPopUp.ShowView(www.error);
            }
            else
            {
                var response = (Response<SubResponse>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<SubResponse>));
                if (response.status == Constants.KEYS.valid)
                {
                    message.text = response.message;
                    if (!UIManager.instance.verificationPopUpView.gameObject.activeInHierarchy)
                    {
                        UIManager.instance.verificationPopUpView.gameObject.SetActive(true);
                    }
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
            }
        }
    }

    public IEnumerator VerifyOTP(string mobileNumber)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.mobile_number, CacheMemory.MobileNumber);
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField(Constants.KEYS.otp_code, UIManager.instance.verificationPopUpView.OTPField.text);

        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.OTP_VERIFICATION, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<UserModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<UserModel>));
                if (response.status == Constants.KEYS.valid)
                {
                    UIManager.instance.verificationPopUpView.messageText.text = response.message;
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                    if (PlayerPrefsManager.GetAuthToken() != null)
                    {
                        StartCoroutine(APIManager.instance.GetProfileDetails());
                        StartCoroutine(UIManager.instance.EnableLobby());
                        mobileNumber = "";
                        if (UIManager.instance.verificationPopUpView.isGoogleSignIn)
                        {
                            UIManager.instance.verificationPopUpView.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
            }
        }
    }

    public IEnumerator VerifyMobileNumber(string mobileNumber, string otpCode)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.mobile_number, mobileNumber);
        form.AddField(Constants.KEYS.otp_code, otpCode);
        ServerManager.instance.loader.ShowLoader("validating");
        Debug.Log("API" + Constants.API_METHODS.VERIFY_OTP + "?mobile_number=" + mobileNumber + "&otp_code=" + otpCode);
        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.VERIFY_OTP, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            ServerManager.instance.loader.HideLoader();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                ServerManager.instance.alertPopUp.ShowView(www.error);
            }
            else
            {
                Debug.Log("LOGIN WITH MOBILE - " + www.downloadHandler.text);
                var response = (Response<UserModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<UserModel>));
                if (response.status == Constants.KEYS.valid)
                {
                    UIManager.instance.verificationPopUpView.messageText.text = response.message;
                    PlayerPrefsManager.SetAuthToken(response.data.auth_token);

                    if (PlayerPrefsManager.GetAuthToken() != null)
                    {
                        StartCoroutine(instance.GetProfileDetails());
                        StartCoroutine(UIManager.instance.EnableLobby());
                    }
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);

                }
            }
        }
    }

    public IEnumerator Deposit()
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.amount, UIManager.instance.lobbyView.addCashPanelView.amountField.text);
        form.AddField(Constants.KEYS.version_code, Application.version);
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        //form.AddField(Constants.KEYS.bonus_code, UIManager.instance.lobbyView.addCashPanelView.bonusField.text);

        /*if (attach_deposit != null)
        {
            form.AddBinaryData(Constants.KEYS.attatchment, attach_deposit.EncodeToJPG(), "attach_deposit.jpg", "image/*");
        }*/

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.DEPOSIT, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<UserModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<UserModel>));
                if (response.status == Constants.KEYS.valid)
                {
                    Application.OpenURL(response.data.deposite_url);
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                    Debug.Log(www.downloadHandler.text);
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);

                }
            }
        }
    }

    public IEnumerator ForgotPassword(string username, Action forgotPassowrd)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.username, username);

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.FORGOT_PASSWORD, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<object>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<object>));
                if (response.status == Constants.KEYS.valid)
                {
                    forgotPassowrd.Invoke();
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);

                }
            }
        }
    }

    public IEnumerator ChangePassword(string currentPassword, string newPassword, string confirmPassword, Action changePassword)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField(Constants.KEYS.current_password, currentPassword);
        form.AddField(Constants.KEYS.new_password, newPassword);
        form.AddField(Constants.KEYS.confirm_new_password, confirmPassword);

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.CHANGE_PASSWORD, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<object>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<object>));
                if (response.status == Constants.KEYS.valid)
                {
                    changePassword.Invoke();
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);

                }
            }
        }
    }

    public IEnumerator AddKYC(string aadhar, string pan, Texture2D aadhar_front_image, Texture2D aadhar_back_image, Texture2D pan_card_image, Action<Response<object>> callback)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField(Constants.KEYS.aadhar, aadhar);
        form.AddField(Constants.KEYS.pan, pan);

        if (aadhar_front_image != null)
        {
            form.AddBinaryData(Constants.KEYS.aadhar_front_image, aadhar_front_image.EncodeToJPG(), "aadhar_front_image.jpg", "image/*");
        }

        if (aadhar_back_image != null)
        {
            form.AddBinaryData(Constants.KEYS.aadhar_back_image, aadhar_back_image.EncodeToJPG(), "aadhar_back_image.jpg", "image/*");
        }

        if (pan_card_image != null)
        {
            form.AddBinaryData(Constants.KEYS.pan_card_image, pan_card_image.EncodeToJPG(), "pan_card_image.jpg", "image/*");
        }

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.ADD_KYC, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                ServerManager.instance.alertPopUp.ShowView(www.error);
            }
            else
            {
                var response = (Response<object>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<object>));
                callback(response);
            }
        }
    }

    public IEnumerator FetchKYC(Action<Response<KYCModel>> callback)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        Debug.Log(Constants.API_METHODS.FETCH_KYC_DETAILS + "?auth_token=" + PlayerPrefsManager.GetAuthToken());
        using (var www = UnityWebRequest.Post(Constants.API_METHODS.FETCH_KYC_DETAILS, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("KYC DETAIL - " + www.downloadHandler.text);
                var response = (Response<KYCModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<KYCModel>));
                callback(response);
            }
        }
    }

    public IEnumerator UploadBankProof(Texture2D proofImage, Action<Response<BankProofUploadModel>> callback)
    {
        var form = new WWWForm();
        form.AddBinaryData(Constants.KEYS.file, proofImage.EncodeToJPG(), "proofpic.jpg", "image/*");

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.UPLOAD_BANK_PROOF, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                var response = (Response<BankProofUploadModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<BankProofUploadModel>));
                callback(response);
            }
        }
    }

    public IEnumerator AddBankDetails(string accountNumber, string accountHolderName, string ifscCode, string bankName, string branchName)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField(Constants.KEYS.account_number, accountNumber);
        form.AddField(Constants.KEYS.account_holder_name, accountHolderName);
        form.AddField(Constants.KEYS.ifsc_code, ifscCode);
        form.AddField(Constants.KEYS.bank_name, bankName);
        form.AddField(Constants.KEYS.branch_name, branchName);
      /*  if (attach_deposit != null)
        {
            form.AddBinaryData(Constants.KEYS.bank_proof, attach_deposit.EncodeToJPG(), "attach_deposit.jpg", "image/*");
        } */
        
        using (var www = UnityWebRequest.Post(Constants.API_METHODS.ADD_BANK_DETAILS, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Deposit - " + www.downloadHandler.text);
                var response = (Response<UserModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<UserModel>));
                if (response.status == Constants.KEYS.valid)
                {
                   // ServerManager.instance.alertPopUp.ShowView(bankProof);
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);

                }
            }
        }
    }

    public IEnumerator FetchBankDetails(Action<Response<BankDetailsModel>> callbak)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.FETCH_BANK_DETAILS, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Bank Details - " + www.downloadHandler.text);
                var response = (Response<BankDetailsModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<BankDetailsModel>));
                callbak(response);
            }
        }
    }

    public IEnumerator ValidationDeposit()
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.DEPOSIT_VALIDATION, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<object>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<object>));
                if (response.status == Constants.KEYS.valid)
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);

                }
            }
        }
    }

    public IEnumerator WithdrawVerification(string amount)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField(Constants.KEYS.amount, amount);

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.VERIFY_WITHDRAW, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();
            ServerManager.instance.loader.HideLoader();
            if (www.result != UnityWebRequest.Result.Success)
            {
                ServerManager.instance.loader.ShowLoader(www.error);
            }
            else
            {
                Debug.Log("VERIFY_WITHDRAW " + www.downloadHandler.text);
                var response = (Response<WithdrawModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<WithdrawModel>));
                if (response.status == Constants.KEYS.valid)
                {
                    ServerManager.instance.alertPopUp.ShowView("Would you like to proceed for " + response.data.amount + " withdraw?",
                        () =>
                        {
                            ServerManager.instance.loader.ShowLoader("Submitting...");
                            StartCoroutine(WithdrawRequest(response.data.amount));
                        }, "Accept", () => { }, "Cancel");
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                }
            }
        }
    }

    public IEnumerator WithdrawRequest(string amount)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField(Constants.KEYS.amount, amount);

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.WITHDRAW_REQUEST, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                ServerManager.instance.loader.HideLoader();
                Debug.Log(www.error);
            }
            else
            {
                ServerManager.instance.loader.HideLoader();
                var response = (Response<WithdrawModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<WithdrawModel>));
                ServerManager.instance.alertPopUp.ShowView(response.message);
                if (response != null && response.status == Constants.KEYS.valid) {
                    UIManager.instance.lobbyView.UpdateHeader();
                }
            }
        }
    }


    public IEnumerator ContactUsForm(string category, string message)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField("category", category);
        form.AddField(Constants.KEYS.message, message);

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.CONTACT, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();
            ServerManager.instance.loader.HideLoader();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Response : " + www.downloadHandler.text);
                var response = (Response<UserModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<UserModel>));
                ServerManager.instance.alertPopUp.ShowView(response.message);
            }
        }
    }

    public IEnumerator DepositTransactionsList(Action<Response<List<DepositTransactionsModel>>> callback)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField("start", "");
        form.AddField("per_page", "");

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.DEPOSIT_LIST, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<List<DepositTransactionsModel>>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<List<DepositTransactionsModel>>));
                callback?.Invoke(response);
            }
        }
    }

    public IEnumerator WithdrawTransactionsList(Action<Response<List<WithdrawTransactionsModel>>> callback)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField("start", "");
        form.AddField("per_page", "");

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.WITHDRAW_LIST, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<List<WithdrawTransactionsModel>>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<List<WithdrawTransactionsModel>>));
                callback?.Invoke(response);
            }
        }
    }

    public IEnumerator BonusTransactionsList(Action<Response<List<BonusTransactionsModel>>> callback)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField("start", "");
        form.AddField("per_page", "");

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.BONUS_LIST, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<List<BonusTransactionsModel>>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<List<BonusTransactionsModel>>));
                callback?.Invoke(response);
            }
        }
    }

    public IEnumerator GameHistory()
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField("start", "");
        form.AddField("per_page", "");

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.GAME_HISTORY, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<List<GameHistoryModel>>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<List<GameHistoryModel>>));
                if (response.status == Constants.KEYS.valid)
                {
                    UIManager.instance.accountMenuView.gameHistoryPanel.SetGameInfo(response);
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);

                }
            }
        }
    }

    public IEnumerator VerifyReferCode(string referCode)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField("refer_code", referCode);

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.EMAIL_REFER, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<object>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<object>));
                if (response.status == Constants.KEYS.valid)
                {
                    ServerManager.instance.alertPopUp.ShowView(response.status);
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.status);

                }
            }
        }
    }

    public IEnumerator RegisterTournament(string tournamentId)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField(Constants.KEYS.tournament_id, tournamentId);

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.TOURNAMENT_REGISTER, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<object>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<object>));
                if (response.status == Constants.KEYS.valid)
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);
                    UIManager.instance.lobbyView.UpdateHeader();
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);

                }
            }
        }

    }

    public IEnumerator HostedGameDetails(Action panelCallBack)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.HOSTED_PRIVATE_GAMES_DETAILS, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<List<HostedGameDetailsModel>>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<List<HostedGameDetailsModel>>));
                if (response.status == Constants.KEYS.valid)
                {
                    panelCallBack.Invoke();
                    UIManager.instance.accountMenuView.hostedPrivateGamesPanel.SetGameInfo(response);
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);

                }
            }
        }
    }

    public IEnumerator CoinPurchasing(string amount, string coins)
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.amount, amount);
        form.AddField(Constants.KEYS.version_code, Application.version);
        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        form.AddField(Constants.KEYS.coins, coins);

        using (var www = UnityWebRequest.Post(Constants.API_METHODS.ADD_COINS, form))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("PURCHASE COINT");
                var response = (Response<UserModel>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<UserModel>));
                if (response.status == Constants.KEYS.valid)
                {
                    StartCoroutine(ServerManager.instance.commonWebView.WebViewPanelViewEntryEffect("Purchase Coins", response.data.deposite_url));
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);

                }
            }
        }
    }

    public IEnumerator CheckLocation(WWWForm parmeter, Action<Response<object>> callBack)
    {
        
        ServerManager.instance.loader.ShowLoader("validating");
        using (var www = UnityWebRequest.Post(Constants.API_METHODS.CHECK_LOCATION, parmeter))
        {
            SendDeviceType(www);
            yield return www.SendWebRequest();
            ServerManager.instance.loader.HideLoader();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                callBack?.Invoke(null);
            }
            else
            {
                var response = (Response<object>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<object>));
                callBack?.Invoke(response);
            }
        }
    }
}
