using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelSlice : MonoBehaviour
{
  public Image SliceImage;
    

   

    public void UpdateSlice(Color SliceColor,float FillAmount,float Zangle,float Spacing ) {

        if (SliceImage != null)
        {
            SliceImage.color = SliceColor;
            SliceImage.fillAmount = FillAmount;
            transform.eulerAngles = new Vector3(0, 0, Zangle);
            GetComponent<Outline>().effectDistance = new Vector2(Spacing, Spacing);
        }
    
    
    }
}
