using UnityEngine;

public class TestGallery : MonoBehaviour
{
	[SerializeField]
	SunshineNativeGalleryHandler _sunshineScript;
		public void OpenGallery()
    {
        _sunshineScript.OpenGallery((bool success, string[] paths) =>
        {
            if (success)
            {
                // paths = file location or url of all picked items
                //Showing the picked Item.. You can make your own gallery by yourself
                FindObjectOfType<Gallery>().PrepareGallery(paths);
                FindObjectOfType<GalleryManager>().ShowCrossBtn();
            }
        });
    }


}
