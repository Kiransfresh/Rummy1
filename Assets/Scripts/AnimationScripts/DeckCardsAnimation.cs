using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DeckCardsAnimation : MonoBehaviour
{

    public static DeckCardsAnimation Instance;

    [SerializeField] private GameObject animCard;
    [SerializeField] private GameObject animCard_Open;

    [SerializeField] private Animator initAnimation;

    [SerializeField] private GameObject finishSlot;
    [SerializeField] private GameObject openDeck;
    [SerializeField] private GameObject closeDeck;

    [SerializeField] private GameObject finishSlotChild;
    [SerializeField] private GameObject openDeckChild;
    [SerializeField] private GameObject closeDeckChild;

    [SerializeField] private GameObject animCardsHolder;

    [SerializeField] private GameObject screenBlocker;


    GameObject openDeckAnimCard;
    GameObject closeDeckAnimCard;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void InitOpenDeckCard()
    {
        GamePlayManager.instance.isDeckAnimDone = false;
        openDeckAnimCard = Instantiate(animCard_Open,animCardsHolder.transform);
        openDeckAnimCard.GetComponent<PreApplyCardAnim>().SetCardImage(openDeckChild.transform.GetChild(0).GetComponent<Image>().sprite);

        openDeckAnimCard.transform.DOLocalMove(new(300f,0f,0f),0.5f);
        openDeckAnimCard.transform.DORotate(new(0f, 180f, 0f), 0.5f).OnComplete(() => {
            DestroyAnimCards();
        });



    }

    public void SetNeededCardsState()
    {
        closeDeckChild.SetActive(false);
    }

    public void InitCloseDeckCard()
    {
        closeDeckAnimCard = Instantiate(animCard, closeDeck.transform);
        closeDeckAnimCard.GetComponent<PreApplyCardAnim>().SetCardImage(closeDeckChild.GetComponent<Image>().sprite);

        closeDeckAnimCard.transform.SetSiblingIndex(0);

        closeDeckAnimCard.transform.DOLocalMove(new(-180f, 0f, 0f), 1f).OnComplete(() => {
            closeDeckAnimCard.transform.DORotate(new(0f, 180f, 0f), 1f).OnComplete(() => {
                closeDeckAnimCard.transform.DOLocalMove(new(-80f,-10f,0f),0.7f);
                closeDeckAnimCard.transform.DORotate(new(0f,180f,-15f),0.7f).OnComplete(() => { DisableCloseDeckCards(); });
            });


           // closeDeckAnimCard.transform.DOScale(1.1f,0.5f).SetEase(Ease.Linear).SetDelay(0.25f).OnComplete(()=> { closeDeckAnimCard.transform.DOScale(1f, 0.5f).SetEase(Ease.Linear); });
        });
    }


    public void DestroyAnimCards()
    {
        Destroy(openDeckAnimCard,0.1f);
        openDeckChild.SetActive(true);
        InitCloseDeckCard();
    }

    public void DisableCloseDeckCards()
    {
        Destroy(closeDeckAnimCard,0.1f);
        closeDeckChild.SetActive(true);

        SetScreenBlockerState(false);
        GameRoomAnimationController.isAnimDone = false;
        GamePlayManager.instance.isDeckAnimDone = true;
    }

    public void SetScreenBlockerState(bool _state)
    {
        screenBlocker.SetActive(_state);
    }

}
