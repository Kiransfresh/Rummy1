using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    public GameObject galleryPanel;
    public GameObject galleryCloseBtn;

    bool isCrossBtnShown;



    void Start()
    {
        isCrossBtnShown = false;
        SetCrossBtnVisibility();
    }



    public void ShowCrossBtn()
    {
        isCrossBtnShown = true;
        SetCrossBtnVisibility();
    }

    void SetCrossBtnVisibility()
    {
        galleryCloseBtn.SetActive(isCrossBtnShown);
        galleryPanel.gameObject.GetComponent<Image>().enabled = isCrossBtnShown;
    }

    public void CrossbtnClicked()
    {
        isCrossBtnShown = false;
        SetCrossBtnVisibility();

        GameObject[] galleryItems = GameObject.FindGameObjectsWithTag("gallery_item");
        foreach (GameObject gobj in galleryItems)
        {
            Destroy(gobj);
        }

    }
}
