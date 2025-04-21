using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class Gallery : MonoBehaviour
{

    string directoryPath;
    public GameObject galleryItem;
    public RectTransform galleryPanel;

    string currentUrl;

    int counter;
    int row;
    int previousRow;
    int column;

    VideoPlayer videoPlayer;

    float offset;
    float galleryItemWidth;
    public int itemsPerRow = 3;

    void Start()
    {
        ResizeGallery();
        //  Debug.Log("Directory Path :" + directoryPath);

    }

    private void ResetGallery()
    {
        counter = 0;
        row = 0;
        previousRow = 0;
        column = 0;
    }
    public string[] GetFilePathsFromAFolder()
    {
        directoryPath = Application.persistentDataPath.Trim() + "/";
        DirectoryInfo dir = new DirectoryInfo(directoryPath);
        //FileInfo[] info = dir.GetFiles("*.mp4");
        FileInfo[] info = dir.GetFiles("*");
        string[] paths = new string[info.Length];
        for (int i = 0; i < info.Length; i++)
        {
            paths[i] = info[i].FullName;
        }
        return paths;
    }



    void ResizeGallery()
    {
        /* float width = galleryPanel.rect.width;
         //Debug.Log("width " + width);
         //  offset = (width / (itemsPerRow + 1)) / (itemsPerRow + 1); //getting unused space for both sides
         offset = (width / (itemsPerRow + 1) / 2); //getting unused space for both sides
         galleryItem.GetComponent<RectTransform>().sizeDelta = new Vector2(width / (itemsPerRow + 1), width / (itemsPerRow + 1));

         offset = 54;
         Debug.Log("Size Delta  " + galleryItem.GetComponent<RectTransform>().sizeDelta);
         galleryItemWidth = galleryItem.GetComponent<RectTransform>().rect.width;*/
        CalculateOffset();
    }

    void CalculateOffset()
    {
        float width = galleryPanel.rect.width;
        Debug.Log("Width : " + width);

        float perItemWidth_full = width / itemsPerRow; //270
        galleryItemWidth = (width / (itemsPerRow + 1)); //216

        offset = perItemWidth_full - galleryItemWidth;

        Debug.Log("Offset " + offset);

        galleryItem.GetComponent<RectTransform>().sizeDelta = new Vector2(galleryItemWidth, galleryItemWidth);
        Debug.Log("galleryItemWidth " + galleryItemWidth);
    }

    public void PrepareGallery(string[] paths)
    {

        ResetGallery();
        StartCoroutine(GalleryMaker(paths));
    }

    IEnumerator GalleryMaker(string[] paths)
    {
        foreach (string path in paths)
        {
            // Debug.Log(path);
            row = counter / itemsPerRow;
            column = counter % itemsPerRow;

            if (previousRow != row)
            {
                gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (galleryItemWidth + offset) + row * (galleryItemWidth + offset));
                //Debug.Log(Screen.currentResolution.width);
                previousRow = row;
            }

            var item = Instantiate(galleryItem) as GameObject;

            item.transform.SetParent(gameObject.transform, false);

            RectTransform rt = item.GetComponent<RectTransform>();

            rt.anchoredPosition3D = new Vector3(offset + column * (galleryItemWidth + offset), -(offset + row * (galleryItemWidth + offset)), 0);

            var script = item.GetComponent<GalleryItemController>();
            // script.image = item.GetComponent<RawImage>();

            script.url = path;
            currentUrl = path;
            counter++;

            yield return null;
        }
    }








}
