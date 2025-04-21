using System.Collections;
using TMPro;
using UnityEngine;

public class Loader : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private RectTransform rectComponent;
    [SerializeField] private float rotateSpeed = 200f;


    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    private void Update()
    {
        rectComponent.Rotate(0f, 0f, -rotateSpeed * Time.deltaTime);
    }

    public void ShowLoader(string message)
    {
        infoText.text = message;
        gameObject.SetActive(true);
    }

    public void HideLoader()
    {
        gameObject.SetActive(false);
    }

}
