using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerPrefsManager
{
    #region Auth_Token set and get
    public static void SetAuthToken(string auth_token)
    {
        PlayerPrefs.SetString(Constants.KEYS.auth_token, auth_token);
        PlayerPrefs.Save();
    }

    public static string GetAuthToken()
    {
        var auth_token = PlayerPrefs.GetString(Constants.KEYS.auth_token);
        if (!string.IsNullOrEmpty(auth_token))
        {
            return auth_token;
        }
        else
        {
            return null;
        }
    }
    #endregion


    #region FisrtLaunch set and get
    public static void FisrtLaunchForLocation()
    {
        PlayerPrefs.SetString("FISRT_LAUNCH_LOCATION", "YES");
        PlayerPrefs.Save();
    }

    public static string GetFisrtLaunchForLocation()
    {
        var value = PlayerPrefs.GetString("FISRT_LAUNCH_LOCATION");
        if (!string.IsNullOrEmpty(value))
        {
            return value;
        }
        else
        {
            return "NO";
        }
    }

    public static void FisrtLaunchForStorage()
    {
        PlayerPrefs.SetString("FISRT_LAUNCH_STORAGE", "YES");
        PlayerPrefs.Save();
    }

    public static string GetFisrtLaunchForStorage()
    {
        var value = PlayerPrefs.GetString("FISRT_LAUNCH_STORAGE");
        if (!string.IsNullOrEmpty(value))
        {
            return value;
        }
        else
        {
            return "NO";
        }
    }

    #endregion


    #region Refferal_Code set and get
    public static void SetReferralCode(string refer_code)
    {
        PlayerPrefs.SetString(Constants.KEYS.refer_code, refer_code);
        PlayerPrefs.Save();
    }

    public static string GetReferralCode()
    {
        var refer_code = PlayerPrefs.GetString(Constants.KEYS.refer_code);
        if (!string.IsNullOrEmpty(refer_code))
        {
            Debug.Log("Refer_code: " + Constants.KEYS.refer_code);
            return refer_code;
        }
        else
        {
            return null;
        }
    }
    #endregion

}
