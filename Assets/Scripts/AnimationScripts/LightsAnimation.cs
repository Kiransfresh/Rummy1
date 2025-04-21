using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightsAnimation : MonoBehaviour
{

    public bool IsLooping;
    public float fps, Multipler;
    float FillAmount=1;
    Image LightImg;



    public void OnEnable()
    {
        StartCoroutine(UISpriteAnim());
        LightImg = GetComponent<Image>();
    }

    IEnumerator UISpriteAnim()
    {
        bool increment = false;
        yield return new WaitForEndOfFrame();
        if (gameObject.activeInHierarchy)
        {
            while (true)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    LightImg.fillAmount = FillAmount;

                    if(increment)
                    FillAmount += Time.deltaTime * Multipler;
                    else
                    FillAmount -= Time.deltaTime * Multipler;


                    if (FillAmount < 0)
                    {
                        increment = true;
                        LightImg.fillClockwise = !LightImg.fillClockwise;

                    }
                    else if(FillAmount > 1)
                    {
                        increment = false;

                    }
                   

                    yield return new WaitForSeconds(1 / fps);

                }
               
                if (!IsLooping)
                    break;

            }
        }
    }
}
