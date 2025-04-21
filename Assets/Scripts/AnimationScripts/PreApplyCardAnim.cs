using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreApplyCardAnim : MonoBehaviour
{
    public Image cardImage;


    public void SetCardImage(Sprite _sprite)
    {
        cardImage.sprite = _sprite;
    }

}
