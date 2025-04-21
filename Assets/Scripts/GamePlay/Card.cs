using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Card :  MonoBehaviour, IPointerClickHandler, IComparable<Card> {

    [Range(1, 14)] [HideInInspector] public GameTableResponse.CardModel cardModel;
    public bool IsCardSelected;

    [HideInInspector] public int count;

    public float SlideUpAmount;
    [SerializeField]
    private float time = 0.25f;


    [SerializeField]
    public Image CardImage;
    
    [HideInInspector]
    public IEnumerator SlideAnim;
    Transform CurrentParent;
    public Image CutJokerImage;
    public GameObject Rayblocker,DropBlocker;
    bool IsDropped;
    public bool isMeldCardSlected;
    private void Start()
    {
        CardImage = GetComponent<Image>();
        UpdateCurrentParent();
    }

    public void SetCardData(GameTableResponse.CardModel cardModel)
    {
        this.cardModel = cardModel;
        var name = cardModel.suit + "_" + cardModel.rank;
        CardImage.sprite = GamePlayManager.instance.GetSprite(name);
        if (cardModel.rank != 0)
        {
            if (GamePlayManager.instance.gameTableEventHandler.gameTableResponse.gameModel.game_type.Equals(Constants.GAME_TYPE.CASH))
            {
                CutJokerImage.gameObject.SetActive(cardModel.cutJoker);
            }
            if (GamePlayManager.instance.gameTableEventHandler
                .gameTableResponse.gameModel.game_type.Equals(Constants.GAME_TYPE.PRACTICE))
            {
                CutJokerImage.gameObject.SetActive(cardModel.cutJoker);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(CardImage.raycastTarget && IsDropped==false)
            GamePlayManager.instance.OnCardCLick(this);
    }

    public void UpdateCardState()
    {
        count = PlayerPrefs.GetInt(Constants.KEYS.Selection_Counter);
        if (!IsCardSelected)
        {
           
            IsCardSelected = true;
            count++;
            PlayerPrefs.SetInt(Constants.KEYS.Selection_Counter, count);
            isMeldCardSlected = true;
        }
        else
        {
            IsCardSelected = false;
            count--;
            PlayerPrefs.SetInt(Constants.KEYS.Selection_Counter, count);
            isMeldCardSlected = true;
        }

        GamePlayManager.instance.gameTableEventHandler.tableHeader.gameSettingMenu.gameObject.SetActive(false);
        if (count < 1)
        {
            PlayerPrefs.DeleteKey(Constants.KEYS.Selection_Counter);
        }
        GamePlayManager.instance.gameTableEventHandler.buttonStats.GroupBtnStats(count > 1);
        SlideUpOrDown(IsCardSelected);
    }

    public void SlideUpOrDown(bool IsCardSelected)
    {
        if (IsCardSelected)
        {
            transform.DOLocalMoveY(40, 0.20f);
        }
        else
        {
            transform.DOLocalMoveY(0, 0.20f);
        }
    }


    public int GetIndex()
    {
        return transform.parent.GetSiblingIndex();
    }

    public Transform GetParent()
    {
        return CurrentParent;
    }

    public Sprite GetCardSprite()
    {
        return CardImage.sprite;
    }

    public void SetSlotParent(GameObject Parent)
    {
        if(Parent == null) return;
        IsCardSelected = false;
        GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        transform.parent.SetParent(Parent.transform);
    }

    public void UndoParent()
    {
        GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        transform.parent.SetParent(CurrentParent);
    }

    public void UpdateCurrentParent()
    {
        CurrentParent = transform.parent.transform.parent;
    }

    public void ActivateBlocker()
    {
            Rayblocker.SetActive(true);
    }

    public void DeactivateBlocker()
    {
        Rayblocker.SetActive(false);
    }
    public void ActivateDropBlocker()
    {
        
        DropBlocker.SetActive(true);
        GetComponentInParent<Draggables>().enabled = false;

    }
    public void DeactivateDropBlocker()
    {

        DropBlocker.SetActive(false);

    }

    public int CompareTo(Card other)
    {
        if (this.cardModel.rank == null || other.cardModel.rank == null)
        {
            return 0;
        }
        return this.cardModel.rank.CompareTo(other.cardModel.rank);
    }

    private void OnDestroy()
    {
        if (GamePlayManager.instance.inHandCardList.Contains(this)) 
        {
            GamePlayManager.instance.inHandCardList.Remove(this);
        }
    }
    public void SetToDropState() 
    {
        GetComponentInParent<Draggables>().enabled = false;
        if (IsCardSelected)
        {
            SlideUpOrDown(false);
            IsCardSelected = false;
        }

        IsDropped = true;
    }

}

public class SortBySuitName : IComparer<Card>
{
    public int Compare(Card x, Card y)
    {
        if (x.cardModel.suitReArrange == null || y.cardModel.suitReArrange == null) return 0;
        return x.cardModel.suitReArrange.CompareTo(y.cardModel.suitReArrange);
    }
}
