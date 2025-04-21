using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using System;

public class UpdateUsernamePopUp : MonoBehaviour
{
    public ScalingEffect[] scalingEffect;
    public UpdateUsername updateusername;
    public TMP_InputField UsernameText;
    public GameObject userNamePart;
    public GameObject AvatarsView;

    public Button submitBtn, CancelBtn, LeftArrowBtn, RightArrowbtn;
    public Scrollbar HorizontalScroll;
    public GameObject ProfilePicsHolder, AvatarPrefab, AvatarPanel;
    public float ScrollAmount;
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    List<AvatarModel> AvatarOptions = new List<AvatarModel>();
    public List<AvatarImageLocal> avatarSprites;
    ToggleGroup AvatarGroupToggle;
    int SelectedAvatarIndex;
    public Transform AvatarLocPoint;
    bool ActivateOnlyAvatar;
    Vector3 AvatarpanelDefaultpos;


    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);

    }

    private void OnEnable()
    {
        CancelBtn.gameObject.SetActive(UIManager.instance.accountMenuView
            .profilePanelView.gameObject.activeInHierarchy);


        AvatarpanelDefaultpos = AvatarPanel.transform.localPosition;


        UsernameText.text = "";
        StartCoroutine(UsernamePanelPanelEntry());
        if (ProfilePicsHolder.transform.childCount == 0)
        {
            StartCoroutine(GetDefaultAvatars());
            GetDefaultAtatarsLocally();
        }

    }

    private void Start()
    {

        submitBtn.onClick.AddListener(() =>
        {
            if (AvatarPanel.gameObject.activeInHierarchy == false)
            {
                if (userNamePart.gameObject.activeInHierarchy)
                {
                    if (string.IsNullOrEmpty(UsernameText.text) || string.IsNullOrWhiteSpace(UsernameText.text))
                    {
                        ServerManager.instance.alertPopUp.ShowView("Please enter username to proceed further");
                        return;
                    }
                    StartCoroutine(updateusername.SendUpdatedUsername(NewUsernameCallback));
                }
                else
                {
                    ServerManager.instance.alertPopUp.ShowView("Something went wrong, Please contact to developer");
                    return;
                }
            }
            else
            {
                if (GetActiveToggleIndex() < 0)
                {
                    ServerManager.instance.alertPopUp.ShowView("Please select an avatar");
                    return;
                }
                else
                {
                    #region Code to update avatar data to backend
                   // updateusername.Avartarpath = AvatarOptions[GetActiveToggleIndex()].image_path;
                    //StartCoroutine(UIManager.instance.lobbyView.SetUserAvatar
                    //    (UIManager.instance.lobbyView.PlayerAvatar, AvatarOptions[GetActiveToggleIndex()].image));

                    //StartCoroutine(UIManager.instance.lobbyView.SetUserAvatar
                    //        (UIManager.instance.accountMenuView.profilePanelView.profileImage, AvatarOptions[GetActiveToggleIndex()].image));

                   // StartCoroutine(updateusername.SendUpdatedAvatar());
                    #endregion
                    PlayerPrefs.SetInt(Constants.PLAYER_PREFS_CONSTANTS.AVATARIMAGE_ID, GetActiveToggleIndex());

                    UIManager.instance.lobbyView.SetUserAvatarLocally(UIManager.instance.lobbyView.PlayerAvatar);
                    UIManager.instance.lobbyView.SetUserAvatarLocally(UIManager.instance.accountMenuView.profilePanelView.profileImage);



                    if (string.IsNullOrEmpty(UsernameText.text) == false)
                        StartCoroutine(updateusername.SendUpdatedUsername(null));
                }
            }

        });
        CancelBtn.onClick.AddListener(() =>
        {
            StartCoroutine(UsernamePanelExit());
        });
        LeftArrowBtn.onClick.AddListener(() =>
        {
            if (HorizontalScroll.value - ScrollAmount > 0)
                HorizontalScroll.value -= ScrollAmount;
            else
            {
                HorizontalScroll.value = 0;
            }

        });
        RightArrowbtn.onClick.AddListener(() =>
        {
            if (HorizontalScroll.value + ScrollAmount < 1)
                HorizontalScroll.value += ScrollAmount;
            else
            {
                HorizontalScroll.value = 1;
            }
        });
        HorizontalScroll.value = 0;
        AvatarGroupToggle = ProfilePicsHolder.GetComponent<ToggleGroup>();

    }

    public void CloseUsernamePanel()
    {


        StartCoroutine(UsernamePanelExit());

    }
    private void PlayStartEffects()
    {
        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].EntryEffect());
        }

    }

    public void PlayEndEffect()
    {
        for (int i = 0; i < scalingEffect.Length; i++)
        {
            StartCoroutine(scalingEffect[i].ExitEffect());
        }
    }


    private IEnumerator UsernamePanelPanelEntry()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator UsernamePanelExit()
    {
        PlayEndEffect();
        yield return disableDelay;
        userNamePart.SetActive(true);
        AvatarPanel.transform.localPosition = AvatarpanelDefaultpos;
        gameObject.SetActive(false);
    }


    public void DisableVerificationPanel()
    {
        StartCoroutine(UsernamePanelExit());
    }

    public int GetActiveToggleIndex()
    {
        int index = 0;
        try
        {
            index = AvatarGroupToggle.ActiveToggles().FirstOrDefault().gameObject.transform.GetSiblingIndex();
        }
        catch (Exception)
        {
            return -1;
        }
        return index;
    }


    public void GetDefaultAtatarsLocally()
    {
        SetDefaultAvatarsLocally();
    }
    public IEnumerator GetDefaultAvatars()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Constants.API_METHODS.DEFAULT_AVATARS))
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
                Debug.Log("Avatar list - " + www.downloadHandler.text);
                var response = (Response<List<AvatarModel>>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<List<AvatarModel>>));
                if (response.status == Constants.KEYS.valid)
                {

                    foreach (var model in response.data)
                    {
                        AvatarOptions.Add(model);
                    }

                  //  SetDefaultAvatars();
                    
                }
                else
                {

                }
            }
        }
    }

    void SetDefaultAvatars()
    {
        if (AvatarOptions.Count > 0)
        {

            StartCoroutine(SetUserAvatar());
            ScrollAmount = (float)1 / (AvatarOptions.Count / 2);
        }


    }

    void SetDefaultAvatarsLocally()
    {
        if(avatarSprites.Count > 0)
        {
            SetUserAvatarLocally();
            ScrollAmount = (float)1 / (avatarSprites.Count / 2);
        }
    }

    IEnumerator SetUserAvatar()
    {
        foreach (var Avatar in AvatarOptions)
        {
            WWW www = new WWW(Avatar.image);
            yield return www;
            var ProfilePic = Instantiate(AvatarPrefab, ProfilePicsHolder.transform);
            Image avatarImg = ProfilePic.GetComponentInChildren<Image>();
            ProfilePic.GetComponent<Toggle>().group = ProfilePicsHolder.GetComponent<ToggleGroup>();
            avatarImg.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
            if (ProfilePicsHolder.transform.childCount == 0)
            {
                ProfilePic.GetComponent<Toggle>().isOn = true;
            }

        }

    }



    void SetUserAvatarLocally()
    {
        foreach (var item in avatarSprites)
        {
            var ProfilePic = Instantiate(AvatarPrefab, ProfilePicsHolder.transform);
            Image avatarImg = ProfilePic.GetComponentInChildren<Image>();
            ProfilePic.GetComponent<Toggle>().group = ProfilePicsHolder.GetComponent<ToggleGroup>();
            avatarImg.sprite = item.sprite;
            if (ProfilePicsHolder.transform.childCount == 0)
            {
                ProfilePic.GetComponent<Toggle>().isOn = true;
            }
        }
    }



    public void UpgradeOnlyAvatar()
    {
        userNamePart.SetActive(false);
        AvatarPanel.gameObject.SetActive(true);
        AvatarPanel.transform.localPosition = AvatarLocPoint.localPosition;
    }

    private Action<string> NewUsernameCallback;
    public void UpgradeOnlyUsername(string username, Action<string> NewUsername)
    {
        NewUsernameCallback = NewUsername;
        UsernameText.text = username;
        userNamePart.SetActive(true);
        AvatarPanel.gameObject.SetActive(false);
    }
}

[Serializable] 
public class AvatarImageLocal
{
    public int id;
    public Sprite sprite;
}