using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Kakera
{
    public class PickerController : MonoBehaviour
    {
        [SerializeField] private Unimgpicker imagePicker1;
       /* [SerializeField] private Unimgpicker imagePicker2;
        /* [SerializeField] private Unimgpicker imagePicker3;
         [SerializeField] private Unimgpicker imagePicker4;  */

        [SerializeField]
        //public Image Pan;
        public Image bankProofImage;
        /*public Image AdhaarFront;
        public Image AdhaarBack; */

        [SerializeField] private BankStatementRequestPanelView bankStatementRequestPanelView;

        private int imageCounter = 1;

        private enum ActiveImagePicker
        {
            ImagePicker1,
            ImagePicker2,
            ImagePicker3,
            ImagePicker4
        }

        private ActiveImagePicker activePicker;


        private void Start()
        {
            imagePicker1.Completed += OnImagePickerComplete;
            /*imagePicker2.Completed += OnImagePickerComplete;
            imagePicker3.Completed += OnImagePickerComplete;
            imagePicker4.Completed += OnImagePickerComplete;*/
        }

        private void OnImagePickerComplete(string path)
        {
            switch (activePicker)
            {
                case ActiveImagePicker.ImagePicker1:
                    StartCoroutine(LoadImageAndSave(path, bankProofImage, "bankProofImage"));
                    bankStatementRequestPanelView.bankdetailsTxt.gameObject.SetActive(false);
                    break;
               /* case ActiveImagePicker.ImagePicker2:
                    StartCoroutine(LoadImageAndSave(path, AdhaarFront, "AdhaarFront"));
                    kYCPanelView.AdhaarFronttxt.gameObject.SetActive(false);
                    break;
                    /* case ActiveImagePicker.ImagePicker3:
                         StartCoroutine(LoadImageAndSave(path, AdhaarBack, "AdhaarBack"));
                         kYCPanelView.AdhaarBacktxt.gameObject.SetActive(false);
                         break;
                     case ActiveImagePicker.ImagePicker4:
                         StartCoroutine(LoadImageAndSave(path, Pan, "Pan"));
                         kYCPanelView.Pantxt.gameObject.SetActive(false);
                         break;*/
            }
        }

        public void OnPressShowPicker1()
        {
            activePicker = ActiveImagePicker.ImagePicker1;
            imagePicker1.Show("Select Image", "unimgpicker");
        }

      /*  public void OnPressShowPicker2()
        {
            activePicker = ActiveImagePicker.ImagePicker2;
            imagePicker2.Show("Select Image", "unimgpicker");
        } */

        /* public void OnPressShowPicker3()
         {
             activePicker = ActiveImagePicker.ImagePicker3;
             imagePicker3.Show("Select Image", "unimgpicker");
         }

         public void OnPressShowPicker4()
         {
             activePicker = ActiveImagePicker.ImagePicker4;
             imagePicker4.Show("Select Image", "unimgpicker");
         }  */

        private IEnumerator LoadImageAndSave(string path, Image output, string imageName)
        {
            var url = "file://" + path;
            var unityWebRequestTexture = UnityWebRequestTexture.GetTexture(url);
            yield return unityWebRequestTexture.SendWebRequest();

            var texture = ((DownloadHandlerTexture)unityWebRequestTexture.downloadHandler).texture;
            if (texture == null)
            {
                Debug.LogError("Failed to load texture url:" + url);
                yield break;
            }

            output.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            output.color = Color.white;

            SaveImage(texture, imageName);
        }

        private void SaveImage(Texture2D texture, string imageName)
        {
            byte[] bytes = texture.EncodeToPNG();
            string uniqueFileName = imageName + "_" + imageCounter + ".png";
            string path = Application.persistentDataPath + "/" + uniqueFileName;
            System.IO.File.WriteAllBytes(path, bytes);

            Debug.Log("Image saved at: " + path);
            imageCounter++;
        }
    }
}
