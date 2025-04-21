using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class UpdateUsername : MonoBehaviour
{
    public TMP_InputField UpdatedUsernameInputField;

    public string Avartarpath;




    public IEnumerator SendUpdatedUsername(Action<string> NewUserCallBack)
    {

        var form = new WWWForm();

        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());

        form.AddField(Constants.KEYS.unique_name, UpdatedUsernameInputField.text);
        Debug.Log("unique_name=" + UpdatedUsernameInputField.text + "&auth_token=" + PlayerPrefsManager.GetAuthToken());

        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.UPDATE_USERNAME, form))
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
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("UPDATE_USERNAME : " + www.downloadHandler.text);
                var response = (Response<SubResponse>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<SubResponse>));
                if (response.status == Constants.KEYS.valid)
                {
                    NewUserCallBack?.Invoke(UpdatedUsernameInputField.text);
                    UIManager.instance.lobbyView.UpdateHeader();
                    UIManager.instance.usernameupdateView.CloseUsernamePanel();
                }
                ServerManager.instance.alertPopUp.ShowView(response.message);
            }
        }
    }
    public IEnumerator SendUpdatedAvatar()
    {

        var form = new WWWForm();

        form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());

        //form.AddField(Constants.KEYS.avatar_path, Avartarpath);
        Debug.Log(UpdatedUsernameInputField.text);

        string username = UpdatedUsernameInputField.text;

        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.UPDATE_AVATAR, form))
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
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Response: " + www.downloadHandler.text);
                var response = (Response<SubResponse>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<SubResponse>));
                if (response.status == Constants.KEYS.valid)
                {
                    if (username.Trim() == "")
                    {
                        UIManager.instance.lobbyView.UpdateHeader();
                        UIManager.instance.usernameupdateView.CloseUsernamePanel();
                    }
                }
            }
        }
    }
}

