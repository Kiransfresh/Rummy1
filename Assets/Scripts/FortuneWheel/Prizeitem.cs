using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Prizeitem : MonoBehaviour
{
    public Image PrizeImage;
    public TextMeshProUGUI Prizetext;


    public void UpdatePrizeItem(Sprite PrizeSprite,string prizeAmount,float Zangle) {

        PrizeImage.sprite = PrizeSprite;
        Prizetext.text = prizeAmount;
        transform.eulerAngles = new Vector3(0,0,Zangle);
    }

}
