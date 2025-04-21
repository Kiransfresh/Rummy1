using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{

    public Image AnimImage;
    public Sprite[] sprites;
    public bool IsLooping;
    public float fps;

    public void StartAnimation()
    {
        StartCoroutine(UISpriteAnim(AnimImage, sprites));
    }



    IEnumerator UISpriteAnim(Image UIImage, Sprite[] sprites)
    {
        yield return new WaitForEndOfFrame();
        if (UIImage.gameObject.activeInHierarchy)
        {
            while (true)
            {
                for (int i = 2; i < sprites.Length; i++)
                {
                    UIImage.sprite = sprites[i];

                    yield return new WaitForSeconds(2 / fps);
                }
                if (!IsLooping)
                    break;
            }
        }
    }
}
