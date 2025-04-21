using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour 
{
    public bool isOn;

    public Color onColorBg;
    public Color offColorBg;

    public Image toggleBgImage;
    public RectTransform toggle;

    public GameObject handle;
    private RectTransform handleTransform;

    private float handleSize;
    private float onPosY;
    private float offPosY;

    public float handleOffset;

    public GameObject onIcon;
    public GameObject offIcon;


    public float speed;
    static float t = 0.0f;

    private bool switching = false;


    void Awake()
    {
        handleTransform = handle.GetComponent<RectTransform>();
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleSize = handleRect.sizeDelta.y;
        float toggleSizeY = toggle.sizeDelta.y;
        onPosY = (toggleSizeY / 8) - (handleSize / 8) - handleOffset;
        offPosY = onPosY * -1;

    }


    void Start()
    {
        isOn = true;
        Switching();
        if (isOn)
        {
            toggleBgImage.color = onColorBg;
            handleTransform.localPosition = new Vector3(0f, onPosY, 0f);
            onIcon.gameObject.SetActive(true);
            offIcon.gameObject.SetActive(false);
        }
        else
        {
            toggleBgImage.color = offColorBg;
            handleTransform.localPosition = new Vector3(0f, offPosY, 0f);
            onIcon.gameObject.SetActive(false);
            offIcon.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (switching)
        {
            Toggle(isOn);
        }
    }

    public void Switching()
    {
        switching = true;
    }



    public void Toggle(bool toggleStatus)
    {
        if (!onIcon.activeInHierarchy || !offIcon.activeInHierarchy)
        {
            onIcon.SetActive(true);
            offIcon.SetActive(true);
        }

        if (toggleStatus)
        {
            toggleBgImage.color = SmoothColor(onColorBg, offColorBg);
            Transparency(onIcon, 1f, 0f);
            Transparency(offIcon, 0f, 1f);
            handleTransform.localPosition = SmoothMove(handle, onPosY, offPosY);
        }
        else
        {
            toggleBgImage.color = SmoothColor(offColorBg, onColorBg);
            Transparency(onIcon, 0f, 1f);
            Transparency(offIcon, 1f, 0f);
            handleTransform.localPosition = SmoothMove(handle, offPosY, onPosY);
        }

    }


    Vector3 SmoothMove(GameObject toggleHandle, float startPosY, float endPosY)
    {

        Vector3 position = new Vector3(0f, Mathf.Lerp(startPosY, endPosY, t += speed * Time.deltaTime), 0f);
        StopSwitching();
        return position;
    }

    Color SmoothColor(Color startCol, Color endCol)
    {
        Color resultCol;
        resultCol = Color.Lerp(startCol, endCol, t += speed * Time.deltaTime);
        return resultCol;
    }

    CanvasGroup Transparency(GameObject alphaObj, float startAlpha, float endAlpha)
    {
        CanvasGroup alphaVal;
        alphaVal = alphaObj.gameObject.GetComponent<CanvasGroup>();
        alphaVal.alpha = Mathf.Lerp(startAlpha, endAlpha, t += speed * Time.deltaTime);
        return alphaVal;
    }

    void StopSwitching()
    {
        if (t > 1.0f)
        {
            switching = false;

            t = 0.0f;
            switch (isOn)
            {
                case true:
                    isOn = false;
                    break;

                case false:
                    isOn = true;
                    break;
            }

        }
    }

}