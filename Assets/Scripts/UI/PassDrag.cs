using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MagneticScrollView;

//Passesdragmessagestothe parent
public class PassDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private ScrollRect scrollRect;
    public GameObject MagneticScrollerBeginDrag;
   


    void Awake()
    {
        scrollRect = GetComponentInParent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        MagneticScrollerBeginDrag.GetComponent<MagneticScrollRect>().OnBeginDrag(eventData);
        MagneticScrollerBeginDrag.GetComponent<SwipeDetection>().OnBeginDrag(eventData);
        scrollRect.OnBeginDrag(eventData);

    }

    public void OnDrag(PointerEventData eventData)
    {
       
        MagneticScrollerBeginDrag.GetComponent<MagneticScrollRect>().OnDrag(eventData);
        
        scrollRect.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);

        MagneticScrollerBeginDrag.GetComponent<MagneticScrollRect>().OnEndDrag(eventData);
        MagneticScrollerBeginDrag.GetComponent<SwipeDetection>().OnEndDrag(eventData);

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MagneticScrollerBeginDrag.GetComponent<MagneticScrollRect>().OnPointerDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MagneticScrollerBeginDrag.GetComponent<MagneticScrollRect>().OnPointerUp(eventData);
    }
}