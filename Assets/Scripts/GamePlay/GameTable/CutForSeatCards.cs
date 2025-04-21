using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CutForSeatCards : MonoBehaviour
{
    public Image card;
    public Image cardGlow;
    public GameObject seatCard;
   
    public Transform initialPosition;


   private void OnEnable()
   {
       seatCard.transform.position = initialPosition.position;
   }

   public void CutforSeatAnim()
   {
       seatCard.transform.DOLocalMove(Vector3.zero, 0.5f);
   }
}
