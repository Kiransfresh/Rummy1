using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class LocationManager : MonoBehaviour
{
    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
    {
        ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.Location_Access_Message,
                        () =>
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
                        }, "OK", null, null);
    }

    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        AskForPermissionInAndroid();
    }

    internal void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.Location_Access_Message,
                        () =>
                        {
                            AskForPermissionInAndroid();
                        }, "OK", null, null);
    }

    public static LocationManager instance;

    public string latitude = null;
    public string longitude = null;


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
        if (Application.platform == RuntimePlatform.Android)
        {
            if (PlayerPrefsManager.GetFisrtLaunchForLocation() != null && PlayerPrefsManager.GetFisrtLaunchForLocation() == "NO")
            {
                PlayerPrefsManager.FisrtLaunchForLocation();
                ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.Location_Access_Message,
                           () =>
                           {
                               CheckForLocation();
                           }, "OK", null, null);
            }
            else
            {
                CheckForLocation();
            }
        }
        
    }



    public void CheckForLocation()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.location.isEnabledByUser)
            {
                if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    AskForPermissionInAndroid();
                }
                else
                {
                    StartCoroutine(LocationFetchingStart());
                }
            }
            else
            {
                if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation)) {
                    AskForPermissionInAndroid();
                }
                else {
                    OpenIfLocationIsNotEnable();
                }
            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            StartCoroutine(LocationFetchingStart());
        }
    }

    private IEnumerator LocationFetchingStart()
    {

        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
        {
            OpenIfLocationIsNotEnable();
            yield break;
        }
           

        // Starts the location service.
        Input.location.Start();

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

            latitude = Input.location.lastData.latitude.ToString();
            longitude = Input.location.lastData.longitude.ToString();
        }
       // Input.location.Stop();
    }

    private void AskForPermissionInAndroid()
    {
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
        callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
        callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
        Permission.RequestUserPermission(Permission.FineLocation, callbacks);
    }

    private void OpenIfLocationIsNotEnable()
    {
        ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.Location_Access_Message,
                       () =>
                       {
                           if (Application.platform == RuntimePlatform.Android)
                           {
                               AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                               AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.settings.LOCATION_SOURCE_SETTINGS");
                               activity.Call("startActivity", intent);
                           }
                       }, "OK", null, null);
        
    }
}
