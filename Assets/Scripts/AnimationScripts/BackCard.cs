using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BackCard : MonoBehaviour
{
    public float animationTime = 0.15f;
    [SerializeField]

    public void OnCardSpawn(BackCardAnim anim, Ease animEase)
    {

        transform.localPosition = Vector3.zero;
        transform.eulerAngles = Vector3.zero;

        transform.DOMove(anim.CardEndTarget.position, animationTime).SetEase(animEase);
        transform.DORotate(anim.CardEndRotation, animationTime).SetEase(animEase);
        transform.DOScale(anim.CardEndSize, animationTime).SetEase(animEase);

        // transform.SetParent(anim.CardEndTarget);



    }
}
