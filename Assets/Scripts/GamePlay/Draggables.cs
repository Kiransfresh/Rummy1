using UnityEngine;
using UnityEngine.EventSystems;
public class Draggables : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{

    public bool Draggable = true;
    [HideInInspector]
    public Card card;
    public GameObject DuplicatePrefab;
    Transform DuplicateCard;

    private void Start()
    {
        Vector2 CanvasBounds = Camera.main.WorldToScreenPoint(new Vector2(Screen.width, Screen.height));
        card = GetComponentInChildren<Card>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
#if UNITY_EDITOR || UNITY_WEBGL

        if (Input.GetMouseButton(0))
        {
            if (card != null && card.CardImage != null) {
                card.CardImage.enabled = false;
            }
            CreateDuplicate();
            if (card.IsCardSelected)
            {
                card.IsCardSelected = false;
                card.StopAllCoroutines();
                card.SlideAnim = null;
                card.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            }
            GamePlayManager.instance.OnDragStart(this.transform, DuplicateCard.gameObject);
            DuplicateCard.position = this.transform.position;
        }
#else

        if (Input.touchCount==1)
        {
            if (card != null && card.CardImage != null) {
                card.CardImage.enabled = false;
            }
            CreateDuplicate();
            if (card.IsCardSelected)
            {
                card.IsCardSelected = false;
                card.StopAllCoroutines();
                card.SlideAnim = null;
                card.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            }
            GamePlayManager.instance.OnDragStart(this.transform, DuplicateCard.gameObject);
            DuplicateCard.position = this.transform.position;
        }


#endif

    }

    public void OnDrag(PointerEventData eventData)
    {
        // DuplicateCard.position = Vector3.Lerp(DuplicateCard.position, eventData.position, Time.deltaTime * 16);
       if(DuplicateCard!=null)
            DuplicateCard.position = eventData.position;
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (DuplicateCard != null)
        {
            if (card != null && card.CardImage != null) {
                card.CardImage.enabled = true;
            }
            transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
            GamePlayManager.instance.OnCardDrop();
            DeleteDuplicate();
        }
    }
    private void OnDisable()
    {
        if (DuplicateCard != null)
        DeleteDuplicate();
    }
    void CreateDuplicate()
    {
        var Card = Instantiate(DuplicatePrefab, FindObjectOfType<Canvas>().transform);
        Sprite temp = card.GetCardSprite();
        DuplicateCard = Card.transform;
        DuplicateCard.GetComponentInChildren<DummyCard>().SetCardSprite(temp);
    }

    void DeleteDuplicate()
    {
        Destroy(DuplicateCard.gameObject);
    }

}
