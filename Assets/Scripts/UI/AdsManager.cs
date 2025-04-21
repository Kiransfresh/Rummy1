using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using MagneticScrollView;
public class AdsManager : MonoBehaviour
{
    private List<string> AdImagespaths = new List<string>();
    public Transform AdImageHolder;
    public GameObject AdImageprefab;
    public float AutoScrollTimeGap;
    private float ScrollTimer;
    private MagneticScrollRect HorizontalScroll;



    private void Awake()
    {
        ScrollTimer = AutoScrollTimeGap;
        StartCoroutine(GetAdImages());
       HorizontalScroll= AdImageHolder.GetComponentInParent<MagneticScrollRect>();
    }


    private void Update()
    {
        ScrollTimer -= Time.deltaTime;
        if (ScrollTimer < 0) {

            MovetoNextSlide();
            ScrollTimer = AutoScrollTimeGap;
        }
        
    }


    void MovetoNextSlide() {
        HorizontalScroll.ScrollForward();
    }


    public IEnumerator GetAdImages()
    {
        var form = new WWWForm();
        form.AddField(Constants.KEYS.display_in, "Mobile App");

        using (UnityWebRequest www = UnityWebRequest.Post(Constants.API_METHODS.ADS_IMAGES,form))
        {
#if UNITY_ANDROID
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_android);
#elif UNITY_IPHONE
            www.SetRequestHeader(Constants.KEYS.requesting_source, Constants.DEVICE_TYPE.Unity_Ios);

#endif
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var response = (Response<List<AvatarModel>>)JsonUtility.FromJson(www.downloadHandler.text, typeof(Response<List<AvatarModel>>));
                if (response.status == Constants.KEYS.valid)
                {


                    foreach (var model in response.data)
                    {
                        AdImagespaths.Add(model.image);
                    }

                    SetAdimages();
                }
                else
                {

                }
            }
        }
    }


    void SetAdimages() {


        if (AdImagespaths.Count > 0) {

            StartCoroutine(SetAdBannerImage());
        
        }
    
    }


    IEnumerator SetAdBannerImage() {

        foreach (string path in AdImagespaths)
        {
            WWW www = new WWW(path);
            yield return www;
            var adImage = Instantiate(AdImageprefab,AdImageHolder);
            Image avatarImg = adImage.GetComponent<Image>();
           
            avatarImg.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
            yield return new WaitForSeconds(0.2f);
        }
      //  HorizontalScroll.numberOfSteps = AdImagespaths.Count;
    }
}
