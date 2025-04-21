using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class StorageManager : MonoBehaviour
{
    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
    {
        ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.Storage_Access_Message,
                        () =>
                        {
                            OpenIfPermissionIsNotEnable();
                        }, "OK", null, null);
    }

    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        //AskForPermissionInAndroid();
        permissionAllowed.Invoke(true);
    }

    internal void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.Storage_Access_Message,
                        () =>
                        {
                            OpenIfPermissionIsNotEnable();
                        }, "OK", null, null);
    }

    public static StorageManager instance;
    public Action<bool> permissionAllowed;


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


    public void CheckForPermission()
    {
        if (Application.platform == RuntimePlatform.Android)
        {

            if (PlayerPrefsManager.GetFisrtLaunchForStorage() != null && PlayerPrefsManager.GetFisrtLaunchForStorage() == "NO")
            {
                PlayerPrefsManager.FisrtLaunchForStorage();
                ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.Storage_Access_Message,
                           () =>
                           {
                               if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
                               {
                                   AskForPermissionInAndroid();
                               }
                               else
                               {
                                   permissionAllowed.Invoke(true);
                               }
                           }, "OK", null, null);
            }
            else
            {
                if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
                {
                    AskForPermissionInAndroid();
                }
                else
                {
                    permissionAllowed.Invoke(true);
                }
            }
        }
    }

    

    private void AskForPermissionInAndroid()
    {
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
        callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
        callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
        Permission.RequestUserPermission(Permission.ExternalStorageRead, callbacks);
    }

    private void OpenIfPermissionIsNotEnable()
    {
        using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
        {
            string packageName = currentActivityObject.Call<string>("getPackageName");

            using (var uriClass = new AndroidJavaClass("android.net.Uri"))
            using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
            using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
            {
                intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                currentActivityObject.Call("startActivity", intentObject);
            }
        }
    }
}
