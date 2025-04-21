using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// Used this class while only having free games
/// </summary>
public class LoginWithMobileNumber : MonoBehaviour
{
    public TMP_InputField MobileNumberInputField;
    public TextMeshProUGUI ResponseText;

    public IEnumerator LoginAsGuest()
    {
        var form = new WWWForm();

        ServerManager.instance.loader.ShowLoader("validating");

        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.GUEST_LOGIN, form))
        {
            APIManager.instance.SendDeviceType(www);
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
                    UIManager.instance.verificationPopUpView.messageText.text = response.message;
                    PlayerPrefsManager.SetAuthToken(response.data.auth_token);
                    if (PlayerPrefsManager.GetAuthToken() != null)
                    {
                        StartCoroutine(APIManager.instance.GetProfileDetails());
                        StartCoroutine(UIManager.instance.EnableLobby());
                        MobileNumberInputField.text = "";

                    }
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView(response.message);

                }
            }
        }
    }
}
