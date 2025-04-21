using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GalleryItemController : MonoBehaviour
{

    public RawImage image;
    public string url;
    public GameObject playIcon;
    //public VideoClip videoToPlay;

    private VideoPlayer videoPlayer;
    private VideoSource videoSource;
    private Button button;
    //private AudioSource audioSource;
    bool isitFirstTime = true;
    // Use this for initialization

    void Start()
    {
        //  playIcon.SetActive(false);
        isitFirstTime = true;
        InitializeComponents();
    }


    void InitializeComponents()
    {
        Application.runInBackground = true;
        button = GetComponent<Button>();
        // GalleryManager gm = GameObject.Find("Script Holder").GetComponent<GalleryManager>();
        button.onClick.AddListener(() =>
        {
            // gm.GoToVideo(GetComponent<VideoPlayer>().url);
            //  gm.GoToVideo(url);
        }

        );

        ShowOrPlayGalleryItem();

    }

    void OnDisable()
    {
        isitFirstTime = false;

    }

    void OnEnable()
    {
        if (isitFirstTime == false)
        {
            InitializeComponents();

        }

    }


    void ShowOrPlayGalleryItem()
    {
        if (IsItVideoTypeFile(url))
        {
            playIcon.SetActive(true);
            StartCoroutine(PlayVideo(url));

        }
        else
        {
            playIcon.SetActive(false);
            Texture2D tex = LoadPNG(url);
            image.texture = tex;
        }

    }
    bool IsItVideoTypeFile(string url)
    {
        if (url.Contains(".mp4") || url.Contains(".avi") || url.Contains(".flv") || url.Contains(".wmv") || url.Contains(".mov") || url.Contains(".mkv"))
        {
            return true;
        }
        else return false;
    }

    IEnumerator PlayVideo(string url)
    {
        videoPlayer = gameObject.GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake = true;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = url;
        videoPlayer.Prepare();
        videoPlayer.isLooping = true;
        WaitForSeconds waitTime = new WaitForSeconds(1);
        while (!videoPlayer.isPrepared)
        {
            yield return waitTime;
            break;
        }

        //videoPlayer.Play();

        image.texture = videoPlayer.texture;
        // videoPlayer.Pause();
    }

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
}
