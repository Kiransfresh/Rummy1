using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class GamePlayManager : TableBaseMono
{
    public static GamePlayManager instance;

    public GameObject CardPrefab;
    public CardsHolder CardImages;
    public GameObject DropBackcardprefab;

    public GameObject CardsContainerPrefab, SlotsParent;

    Card LastestHitCard, LastFrameCard, CurrentCard, RightLastCard, PrevRightLast;

    DummyCard dummyCard;
    Transform cardTransform;
    [HideInInspector] public List<Card> inHandCardList = new List<Card>();
    [HideInInspector] public List<GameObject> inHandGroups = new List<GameObject>();
    [HideInInspector] public List<Card> selectedCards = new List<Card>();


    public Button SortBtn, GroupBtn, DiscardBtn, DropBtn, DropCardAnimBtn, leaveBtn;
    public Button OpenDeckBtn, ClosedDeckBtn, FinishBtn;
    public bool isGameFinish;
    public CardSlideAnim cardForAnimation, FinishCardAnimation;
    public Image openDeckImage;
    public Button privateTableCodeSharingBtn;
    GameObject LastHitObj, DropBackcard;
    RayCastUI rayCastUI;

    public GameObject openDeckCards, CloseDeckCards, inHandCards, finishSlot;
    public bool isCardPicked;
    public bool isDropClicked;
    public Image CutJoker;
    public GameRoomAnimationController AnimationController;
    Image SlotHolderBg;

    public bool isDeckInteracted = false;

    private void Start()
    {
#if UNITY_WEBGL
        SlotsParent.GetComponent<HorizontalLayoutGroup>().spacing = Constants.CARD_SPACE.WEBGL_SLOT;
        SlotsParent.transform.GetChild(0).GetComponent<HorizontalLayoutGroup>().spacing =
            -Constants.CARD_SPACE.WEBGL_CARD;
#endif

        PlayerPrefs.DeleteKey(Constants.KEYS.Selection_Counter);
        Application.runInBackground = true;

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance);
        }

        isGameFinish = false;
        OpenDeckBtn.onClick.AddListener(() =>
        {

            if (closeCardBGAnim.activeInHierarchy) closeCardBGAnim.SetActive(false);
            if (openCardBGAnim.activeInHierarchy) openCardBGAnim.SetActive(false);
            isDeckInteracted = true;
            PickOpenDeckCard();
        });

        ClosedDeckBtn.onClick.AddListener(() =>
        {
            if (closeCardBGAnim.activeInHierarchy) closeCardBGAnim.SetActive(false);
            if (openCardBGAnim.activeInHierarchy) openCardBGAnim.SetActive(false);
            isDeckInteracted = true;
            SelfPlayerCardPick(true);
            gameTableEventHandler.tableHeader.gameSettingMenu.gameObject.SetActive(false);
        });
         SortBtn.onClick.AddListener(() => {  SortCardsBySquencesAI(); });
        //SortBtn.onClick.AddListener(SortCards);
        GroupBtn.onClick.AddListener(GroupCards);
        DropBtn.onClick.AddListener(DropRequest);
        DiscardBtn.onClick.AddListener(() => { selfPlayerDiscardCard(false, false); });
        SlotHolderBg = SlotsParent.GetComponent<Image>();
        SlotHolderBg.enabled = false;
        DropCardAnimBtn.onClick.AddListener(OnDropBackCardBtn);
        rayCastUI = GetComponent<RayCastUI>();

        FinishBtn.onClick.AddListener(() =>
        {
            if (inHandCardList.Count > 0)
            {
                if (gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken()).turn)
                {

                    gameTableEventHandler.tableHeader.gameSettingMenu.gameObject.SetActive(false);
                    ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.SHOW_CONFIRMATION, () =>
                    {
                        MeldCardRequest();
                        DeselectAllCards();
                        isGameFinish = true;
                        gameTableEventHandler.buttonStats.isGameSarted = false;

                        //Need to know more about this...
                        // DeclareGameBeforeTimeOut();

                    }, "Yes", () =>
                    {
                        gameTableEventHandler.messageInfo.DeclareButton(false);
                        DeselectAllCards();
                    }, "No");
                }
                else
                {
                    StartCoroutine(gameTableEventHandler.messageInfo
                            .ShowMessage(Constants.MESSAGE.INVALID_TURN, 3f));
                }
            }
        });
    }


    IEnumerator WaitAndSort()
    {

        SortCards();
        yield return new WaitForSeconds(0.5f);
        SortCardsBySquencesAI();
    }



    private void Update()
    {
        if (UIManager.instance != null && UIManager.instance.gameRoom != null && UIManager.instance.gameRoom.activeInHierarchy == true)
        {
            if (gameTableEventHandler.buttonStats.isGameSarted && GameRoomAnimationController.isAnimDone)
            {
                ButtonStats();
            }
            else
            {
                gameTableEventHandler.buttonStats.DeactivateAllButton(false);
            }

            if (isCardPicked && ServerManager.instance.alertPopUp.gameObject.activeInHierarchy
                             && isDropClicked)
            {
                isDropClicked = false;
                ServerManager.instance.alertPopUp.DisableVerificationPanel();
                ServerManager.instance.alertPopUp.gameObject.SetActive(false);
            }
            CheckForInHandCardLable();
            GameStartTimer();
        }


        if (isCurrentPlayerTurn && isDeckInteracted == false && isDeckAnimDone == true)
        {
            PlayerModel model = gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken());

            if (model != null)
            {

                if (model.turn && openCardBGAnim.activeInHierarchy == false && closeCardBGAnim.activeInHierarchy == false)
                {
                    openCardBGAnim.SetActive(true);
                    closeCardBGAnim.SetActive(true);
                }
                else if (!model.turn && openCardBGAnim.activeInHierarchy == true && closeCardBGAnim.activeInHierarchy == true)
                {
                    openCardBGAnim.SetActive(false);
                    closeCardBGAnim.SetActive(false);
                }
            }
        }


    }


    public GameObject openCardBGAnim;
    public GameObject closeCardBGAnim;
    public bool isCurrentPlayerTurn = false;
    public bool isDeckAnimDone = false;



    private void GameStartTimer()
    {
        if (gameTableEventHandler.gameResult.isShowResult ||
            gameTableEventHandler.gameResult.gameObject.activeInHierarchy ||
            gameTableEventHandler.gameResult.timeAfterCloseResultPanel <= 0
            || !gameTableEventHandler.CurrentEvent.Equals(Constants.GAME_TABLE_EVENT.GAME_PLAY_RESULT)) return;
        StartCoroutine(gameTableEventHandler.messageInfo.ShowWithMessage(Constants.MESSAGE.WAITING_FOR_GAME,
               gameTableEventHandler.gameResult.timeAfterCloseResultPanel));
        gameTableEventHandler.gameResult.isShowResult = true;
    }

    public void ButtonStats()
    {
        gameTableEventHandler.buttonStats.SortBtnStats(SlotsParent);
        gameTableEventHandler.buttonStats.DropBtnStats(inHandCardList);
        gameTableEventHandler.buttonStats.AutoDropStats();
        gameTableEventHandler.buttonStats.FinishAndDiscardBtnStats(inHandCardList);
    }

    public void OnDragStart(Transform _cardTransform, GameObject _DummyCard)
    {
        DeselectAllCards();
        _cardTransform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        cardTransform = _cardTransform;
        CurrentCard = _cardTransform.GetComponentInChildren<Card>();
        dummyCard = _DummyCard.GetComponentInChildren<DummyCard>();
        rayCastUI.enableRayCast(true, OnHit, dummyCard.LeftCollider);
    }

    public void OnHit(RaycastResult results)
    {
        Card HitCard = results.gameObject.GetComponent<Card>();

        if (HitCard != null && results.gameObject != SlotsParent)
        {
            LastestHitCard = HitCard;
        }
        else
        {
            LastestHitCard = null;
        }
        LastHitObj = results.gameObject;
    }

    public void OnLastFrameHit(RaycastResult results)
    {
        Card HitCard = results.gameObject.GetComponent<Card>();

        if (HitCard != null && results.gameObject != SlotsParent)
        {
            LastFrameCard = HitCard;
        }
        LastHitObj = results.gameObject;
    }

    public void OnCardDrop()
    {
        // gameTableEventHandler.meldCards.isMeldedCardGroupChange = true;

        if (LastestHitCard == null)
        {


            if (LastHitObj != null)
            {
                if (CheckForFinishOrDisCardDrop())
                {
                    return;
                }
                rayCastUI.RaycastOneFrame(OnLastFrameHit, dummyCard.CenterCollider);
                if (CheckForFinishOrDisCardDrop())
                {
                    return;
                }
                if (LastFrameCard != null)
                {
                    DeselectAllCards();
                    cardTransform.SetParent(LastFrameCard.GetParent());
                    cardTransform.SetSiblingIndex(LastFrameCard.GetIndex());
                    LastFrameCard = null;
                }
                else
                {
                    rayCastUI.RaycastOneFrame(OnLastFrameHit, dummyCard.RightCollider);
                    if (CheckForFinishOrDisCardDrop())
                    {
                        return;
                    }
                    if (LastFrameCard != null)
                    {
                        DeselectAllCards();
                        cardTransform.SetParent(LastFrameCard.GetParent());
                        cardTransform.SetSiblingIndex(LastFrameCard.GetIndex() - 1);
                        LastFrameCard = null;
                    }
                }

            }
        }
        else if (LastestHitCard != cardTransform.GetComponent<Card>())
        {

            if (LastestHitCard.GetParent() == CurrentCard.GetParent())
            {
                DeselectAllCards();
                cardTransform.SetParent(LastestHitCard.GetParent());
                cardTransform.SetSiblingIndex(LastestHitCard.GetIndex() + 1);
            }
            else
            {
                DeselectAllCards();
                cardTransform.SetParent(LastestHitCard.GetParent());
                cardTransform.SetSiblingIndex(LastestHitCard.GetIndex() + 1);
            }
        }
        PlayerPrefs.DeleteKey(Constants.KEYS.Selection_Counter);
        CheckForEmptySLots();
        CurrentCard.UpdateCurrentParent();
        rayCastUI.disableRayCast();
        CheckLastCard();
        SendGroup();
        FinishCardAnimation.gameObject.SetActive(false);
    }

    bool CheckForFinishOrDisCardDrop()
    {


        if (LastHitObj == finishSlot.transform.GetChild(0).gameObject || LastHitObj == OpenDeckBtn.gameObject)
        {

            if (inHandCardList.Count == 14)
            {

                if (LastHitObj == finishSlot.transform.GetChild(0).gameObject)
                {
                    ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.SHOW_CONFIRMATION, () =>
                    {
                        DeselectAllCards();
                        CurrentCard.IsCardSelected = true;
                        FinishCardAnimation.gameObject.SetActive(true);
                        FinishCardAnimation.SetOpenCardSprite(CurrentCard.GetCardSprite());
                        MeldCardRequest();
                        isGameFinish = true;
                    }, "Yes", () => { DeselectAllCards(); }, "No");

                }
                else
                {

                    DeselectAllCards();
                    CurrentCard.IsCardSelected = true;
                    selfPlayerDiscardCard(false, true);
                }
            }
            return true;
        }


        return false;

    }

    private void CheckForEmptySLots()
    {
        foreach (Transform child in SlotsParent.transform)
        {
            var cardsCount = child.GetChild(0).transform.childCount;
            if (cardsCount <= 0)
            {
                inHandGroups.Remove(child.gameObject);
                Destroy(child.gameObject);
            }
        }
    }

    public void DestroySlot(GameObject Slot)
    {
        inHandGroups.Remove(Slot);
        Slot.transform.SetParent(null);

        Destroy(Slot);
    }

    public void OnCardCLick(Card _currentCard)
    {
        selectedCards.Add(_currentCard);
        CurrentCard = _currentCard;
        CurrentCard.UpdateCardState();
    }

     private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }

    private void SortCardsBySquencesAI()
    {
        List<Card> deckCards = new List<Card>(inHandCardList);

        var cardsListOfList = CardValidator.getCardsForSystem(deckCards);
        //List<Card> cardList = CardValidator.getCardsForSystem(deckCards);
        ClearList();
        ResetSlots();
        
        var groupIds = new List<int>();
        foreach (var cardModel in cardsListOfList.Item1.Where(cardModel => !groupIds.Contains(cardModel.cardModel.groupId)))
        {
            groupIds.Add(cardModel.cardModel.groupId);
        }

        foreach (var groupId in groupIds)
        {
            var tempParent = Instantiate(CardsContainerPrefab, SlotsParent.transform);
            var tempParent2 = tempParent.transform.GetChild(0).gameObject;
            tempParent2.SetActive(true);
            //Debug.Log("CardList Count After reset Deck - " + cardList.Count);
            foreach (var cardModel in cardsListOfList.Item1.Where(cm => cm.cardModel.groupId == groupId))
            {
                var name = cardModel.cardModel.suit + "_" + cardModel.cardModel.rank;
                var CardParent = Instantiate(CardPrefab, tempParent2.transform);
                var _currentCard = CardParent.GetComponentInChildren<Card>();
                _currentCard.SetCardData(cardModel.cardModel);
                CardParent.name = name;
                _currentCard.UpdateCurrentParent();
                _currentCard.SetSlotParent(tempParent2); // Set slot parent similar to SortCards method
                inHandCardList.Add(_currentCard);
                DeselectAllCards();
            }
            inHandGroups.Add(tempParent);
        }

        CheckLastCard();
        SendGroup();
        CheckForEmptySLots();
    }



    void SortCards()
    {
        foreach (var card in inHandCardList)
        {
            card.cardModel.SuitValueForSorting();
        }
        var sorter = new SortBySuitName();
        DeselectAllCards();
        var ExistingSuits = new List<string>();
        gameTableEventHandler.tableHeader.gameSettingMenu.gameObject.SetActive(false);
        CheckForSelectedCards();
        if (selectedCards.Count > 1)
        {
            foreach (var card in selectedCards)
            {
                card.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                card.IsCardSelected = false;
            }
            selectedCards.Clear();
        }
        inHandGroups.Clear();
        inHandCardList.Sort();
        inHandCardList.Sort(sorter);
       

        foreach (var card in inHandCardList)
        {
            if (!ExistingSuits.Contains(card.cardModel.suit))
            {
                if (ExistingSuits.Count < 1 && inHandGroups.Count > 0)
                {
                    card.SetSlotParent(inHandGroups[0].transform.GetChild(0).gameObject);
                    card.UpdateCurrentParent();
                }
                else
                {
                    var NewSlot = Instantiate(CardsContainerPrefab, SlotsParent.transform);
#if UNITY_WEBGL
                    NewSlot.GetComponent<HorizontalLayoutGroup>().spacing = -Constants.CARD_SPACE.WEBGL_CARD;
#endif

                    inHandGroups.Add(NewSlot);
                    card.SetSlotParent(NewSlot.transform.GetChild(0).gameObject);
                    card.UpdateCurrentParent();
                }
                ExistingSuits.Add(card.cardModel.suit);
            }
            else
            {
                var index = ExistingSuits.IndexOf(card.cardModel.suit);
                var lastCardindex = inHandGroups[index].transform.GetChild(0).childCount - 1;
                card.SetSlotParent(inHandGroups[index].transform.GetChild(0).gameObject);
                card.UpdateCurrentParent();

                var LastCard = inHandGroups[index].transform.GetChild(0).GetChild(lastCardindex).GetComponentInChildren<Card>();
                if (LastCard.cardModel.rank > card.cardModel.rank)
                {
                    card.transform.SetAsLastSibling();
                }
            }
        }
        SendGroup();
        CheckForEmptySLots();
        Invoke("CheckLastCard", 0.2f);
    }



    void SortCards(List<Card> remainingDeck, int groupStarting = 0)
    {
        foreach (var card in remainingDeck)
        {
            card.cardModel.SuitValueForSorting();
        }
        var sorter = new SortBySuitName();
        DeselectAllCards();
        var ExistingSuits = new List<string>();
        gameTableEventHandler.tableHeader.gameSettingMenu.gameObject.SetActive(false);
        CheckForSelectedCards();
        if (selectedCards.Count > 1)
        {
            foreach (var card in selectedCards)
            {
                card.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                card.IsCardSelected = false;
            }
            selectedCards.Clear();
        }
        inHandGroups.Clear();
        remainingDeck.Sort();
        remainingDeck.Sort(sorter);


        foreach (var card in remainingDeck)
        {
            if (!ExistingSuits.Contains(card.cardModel.suit))
            {
                if (ExistingSuits.Count < 1 && inHandGroups.Count > 0)
                {
                    card.SetSlotParent(inHandGroups[groupStarting].transform.GetChild(0).gameObject);
                    card.UpdateCurrentParent();
                }
                else
                {
                    var NewSlot = Instantiate(CardsContainerPrefab, SlotsParent.transform);
#if UNITY_WEBGL
                    NewSlot.GetComponent<HorizontalLayoutGroup>().spacing = -Constants.CARD_SPACE.WEBGL_CARD;
#endif

                    inHandGroups.Add(NewSlot);
                    card.SetSlotParent(NewSlot.transform.GetChild(0).gameObject);
                    card.UpdateCurrentParent();
                }
                ExistingSuits.Add(card.cardModel.suit);
            }
            else
            {
                var index = ExistingSuits.IndexOf(card.cardModel.suit);
                var lastCardindex = inHandGroups[index].transform.GetChild(0).childCount - 1;
                card.SetSlotParent(inHandGroups[index].transform.GetChild(0).gameObject);
                card.UpdateCurrentParent();

                var LastCard = inHandGroups[index].transform.GetChild(0).GetChild(lastCardindex).GetComponentInChildren<Card>();
                if (LastCard.cardModel.rank > card.cardModel.rank)
                {
                    card.transform.SetAsLastSibling();
                }
            }
        }
        SendGroup();
        CheckForEmptySLots();
        Invoke("CheckLastCard", 0.2f);
    }




    void CheckForSelectedCards()
    {
        selectedCards.Clear();
        foreach (var card in inHandCardList.Where(card => card.IsCardSelected))
        {
            selectedCards.Add(card);
        }
    }
    //SAI
    public void DeselectAllCards()
    {
        //if(isGameFinish) return;
        foreach (var card in inHandCardList.Where(card => card.IsCardSelected))
        {
            card.UpdateCardState();
            card.SlideUpOrDown(false);
            card.IsCardSelected = false;
            card.count = 0;
            PlayerPrefs.DeleteKey(Constants.KEYS.Selection_Counter);
        }
    }

    void GroupCards()
    {
        gameTableEventHandler.tableHeader.gameSettingMenu.gameObject.SetActive(false);
        CheckForSelectedCards();
        bool GroupPosibility = true;
        if (selectedCards.Count > 0)
        {
            if (inHandGroups.Count == 6)
            {
                GroupPosibility = false;
                foreach (var card in selectedCards)
                {
                    var Holder = card.GetParent();
                    card.transform.parent.SetParent(null);
                    if (Holder.childCount == 0)
                    {
                        GroupPosibility = true;
                        break;
                    }
                }
            }

            if (GroupPosibility)
            {
                var NewSlot = Instantiate(CardsContainerPrefab, SlotsParent.transform);
#if UNITY_WEBGL
                NewSlot.GetComponent<HorizontalLayoutGroup>().spacing = -Constants.CARD_SPACE.WEBGL_CARD;
#endif
                 NewSlot.transform.SetSiblingIndex(0);
                inHandGroups.Add(NewSlot);
                selectedCards.Sort();

           

                foreach (var card in selectedCards)
                {
                    card.SetSlotParent(NewSlot.transform.GetChild(0).gameObject);
                    card.UpdateCurrentParent();
                    DeselectAllCards();
                }
                CheckForEmptySLots();
            }
            else
            {
                foreach (var card in selectedCards)
                {
                    card.UndoParent();
                    DeselectAllCards();
                }
            }
        }
        SendGroup();
        CheckLastCard();
        CheckForEmptySLots();
    }

    public void StartDealing(List<GameTableResponse.CardModel> cardList)
    {
        Debug.Log("CardList Count Before Reset Deck - " + cardList.Count);
        ResetGameplay();
        ResetSlots();
#if UNITY_WEBGL
        SlotsParent.GetComponent<HorizontalLayoutGroup>().spacing = -Constants.CARD_SPACE.WEBGL_SLOT;
#else
        SlotsParent.GetComponent<HorizontalLayoutGroup>().spacing = -Constants.CARD_SPACE.OTHER_SLOT;
#endif
        AnimationController.IsInShrinkState = true;
        DropCardAnimBtn.gameObject.SetActive(false);
        SlotHolderBg.enabled = false;
        isCardPicked = false;
        if (DropBackcard != null)
        {
            Destroy(DropBackcard);
        }
        var groupIds = new List<int>();
        foreach (var cardModel in cardList.Where(cardModel => !groupIds.Contains(cardModel.groupId)))
        {
            groupIds.Add(cardModel.groupId);
        }

        for (var i = 0; i < groupIds.Count; i++)
        {
            var tempParent = Instantiate(CardsContainerPrefab, SlotsParent.transform);
            var tempParent2 = tempParent.transform.GetChild(0).gameObject;
            tempParent2.SetActive(true);
#if UNITY_WEBGL
            tempParent2.GetComponent<HorizontalLayoutGroup>().spacing = -Constants.CARD_SPACE.WEBGL_SLOT;
#else
            tempParent2.GetComponent<HorizontalLayoutGroup>().spacing = -Constants.CARD_SPACE.OTHER_SLOT;
#endif
            Debug.Log("CardList Count After reset Deck - " + cardList.Count);
            foreach (var cardModel in cardList)
            {

                if (groupIds[i].Equals(cardModel.groupId))
                {
                    var name = cardModel.suit + "_" + cardModel.rank;
                    var CardParent = Instantiate(CardPrefab, tempParent2.transform);
                    var _currentCard = CardParent.GetComponentInChildren<Card>();
                    _currentCard.SetCardData(cardModel);
                    CardParent.name = name;
                    _currentCard.UpdateCurrentParent();
                    inHandCardList.Add(_currentCard);
                    inHandGroups.Add(tempParent);
                    DeselectAllCards();
                }
            }
        }
        isAnimationFinish = true;
        cardForAnimation.gameObject.SetActive(false);
        StartCoroutine(SetOpenDeckCards());
        SetCutJoker(CutJoker);
        SetAllDeck(true);
        CheckLastCard();
        ButtonStats();
        SortCards();
        SendGroup();
        CheckForEmptySLots();
    }

    public void ResetSlots()
    {
        if (SlotsParent.transform.childCount <= 0) return;

        foreach (Transform child in SlotsParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetCutJoker(Image img, string cardName = null)
    {
        if (cardName == null)
        {
            var cardModel = gameTableEventHandler.gameTableResponse.liveGameModel
           .gamePlayCardModel.cutJoker;
            var name = cardModel.suit + "_" + cardModel.rank;
            img.sprite = GetSprite(name);
        }
        else
        {
            img.sprite = GetSprite(cardName);
        }
    }

    public bool isAnimationFinish = false;
    public IEnumerator SetOpenDeckCards()
    {
        var cardModels = gameTableEventHandler.gameTableResponse.liveGameModel.gamePlayCardModel.openDeckCardList;
        if (cardModels.Count > 0)
        {
            var model = cardModels[cardModels.Count - 1];
            var name = model.suit + "_" + model.rank;
            while (!isAnimationFinish)
            {
                yield return null;
            }
            openDeckImage.sprite = GetSprite(name);

            if (gameTableEventHandler.gameTableResponse.gameModel.game_type.Equals(Constants.GAME_TYPE.PRACTICE))
            {
                openDeckImage.transform.GetChild(0).gameObject.SetActive(model.cutJoker);
            }

            if (gameTableEventHandler.gameTableResponse.gameModel.game_type.Equals(Constants.GAME_TYPE.CASH))
            {
                openDeckImage.transform.GetChild(0).gameObject.SetActive(model.cutJoker);
            }
        }
        else
        {
            openDeckImage.sprite = GetSprite("EMPTY_CARD");
            openDeckImage.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private GameTableResponse.CardModel pickCardModel = null;
    private void SelfPlayerCardPick(bool isCloseDeck)
    {
        var playerModel = gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken());
        if (playerModel.turn)
        {
            if (inHandCardList.Count == 13 && !isCardPicked)//SAI
            {
                var request = new GameTableRequest();
                request.table_id = gameTableEventHandler.gameTableResponse.id;
                request.table_event_type = Constants.GAME_TABLE_EVENT.CARD_PICKED;
                var sfsObject = request.ToISfsObject();
                sfsObject.PutUtfString(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
                sfsObject.PutBool(Constants.KEYS.is_close_deck, isCloseDeck);
                SendGameTableRequest(sfsObject);

                var closedDeckanim = ClosedDeckBtn.GetComponentInChildren<CardSlideAnim>();


                if (isCloseDeck)
                {
                    AudioController.instance.PlayCardPick();
                    var cardModel = gameTableEventHandler.gameTableResponse.liveGameModel.gamePlayCardModel.closeDeck;
                    if (cardModel != null)
                    {
                        pickCardModel = cardModel;
                    }
                    isCardPicked = true;

                    closedDeckanim.EndingScale = new Vector3(1f, 1f, 1f);
                    OnDeckCard(closedDeckanim);
                    DeselectAllCards();
                }
                else
                {

                    AudioController.instance.PlayCardPick();
                    var cardModels = gameTableEventHandler.gameTableResponse.liveGameModel.gamePlayCardModel
                        .openDeckCardList;
                    isCardPicked = true;
                    if (cardModels.Count > 0)
                    {
                        if (cardModels.Count > 1)
                        {
                            var model = cardModels[cardModels.Count - 2];
                            var name1 = model.suit + "_" + model.rank;
                            openDeckImage.sprite = GetSprite(name1);
                        }
                        else
                        {
                            openDeckImage.sprite = GetSprite("EMPTY_CARD");
                        }
                        cardForAnimation.EndingScale = new Vector3(1.4f, 1.4f, 1.4f);
                        var name = pickCardModel.suit + "_" + pickCardModel.rank;
                        cardForAnimation.GetComponent<Image>().sprite = GetSprite(name);
                        cardForAnimation.gameObject.SetActive(true);
                        OnDeckCard(cardForAnimation);
                        DeselectAllCards();
                    }
                }
            }
            else
            {

                StartCoroutine(gameTableEventHandler.messageInfo.ShowMessage("You have already picked card", 3));
            }
        }
        else
        {
            StartCoroutine(gameTableEventHandler.messageInfo.ShowMessage(Constants.MESSAGE.INVALID_TURN, 3));
        }
        CheckLastCard();
    }

    private void PickOpenDeckCard()
    {
        gameTableEventHandler.tableHeader.gameSettingMenu.gameObject.SetActive(false);
        var playerModel = gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken());
        if (!cardForAnimation.gameObject.activeInHierarchy && cardForAnimation.DiscardAnim)
        {

            cardForAnimation.CheckForDiscardAnimStatus();
        }



        if (playerModel.turn && cardForAnimation.DiscardAnim == false)
        {
            if (inHandCardList.Count == 13 && !isCardPicked)
            {
                var liveGameModel = gameTableEventHandler.gameTableResponse.liveGameModel;
                var cardModels = liveGameModel.gamePlayCardModel.openDeckCardList;
                pickCardModel = cardModels[cardModels.Count - 1];

                if (pickCardModel.IsJoker() && !liveGameModel.firstTurn)
                {
                    StartCoroutine(gameTableEventHandler.messageInfo.ShowMessage("You can not pick joker", 3));
                }
                else
                {
                    SelfPlayerCardPick(false);
                    isCardPicked = true;
                }
            }
            else
            {
                StartCoroutine(gameTableEventHandler.messageInfo.ShowMessage("You have already picked card", 3));
            }
        }
        else
        {
            StartCoroutine(gameTableEventHandler.messageInfo.ShowMessage(Constants.MESSAGE.INVALID_TURN, 3));
        }
    }

    public void OtherPlayersCardPick()
    {
        var userCard = gameTableEventHandler.gameTableResponse.liveGameModel.gamePlayCardModel.picked;
        if (userCard != null
            && !userCard.auth_token.Equals(PlayerPrefsManager.GetAuthToken()))
        {
            foreach (var player in gameTableEventHandler.playerHolder.players
                .Where(player => player.gameObject.activeInHierarchy)
                .Where(player => player.authToken.Equals(userCard.auth_token)))
            {
                if (gameTableEventHandler.gameTableResponse.liveGameModel.gamePlayCardModel.picked.closeDeck)
                {
                    var closedDeckanim = ClosedDeckBtn.GetComponentInChildren<CardSlideAnim>();
                    closedDeckanim.EndingScale = new Vector3(0.05f, 0.05f, 0.05f);
                    closedDeckanim.TakeOpenCard(player.transform.position);
                }
                else
                {
                    cardForAnimation.EndingScale = new Vector3(0.05f, 0.05f, 0.05f);
                    var cardModel = gameTableEventHandler.gameTableResponse.liveGameModel.gamePlayCardModel.picked
                        .cardModel;
                    var name = cardModel.suit + "_" + cardModel.rank;
                    cardForAnimation.GetComponent<Image>().sprite = GetSprite(name);
                    cardForAnimation.gameObject.SetActive(true);
                    cardForAnimation.TakeOpenCard(player.transform.position);
                }
                break;
            }
        }
    }

    private void selfPlayerDiscardCard(bool isDeclare, bool IsDrag)
    {
        gameTableEventHandler.tableHeader.gameSettingMenu.gameObject.SetActive(false);
        CheckForSelectedCards();
        if (selectedCards.Count == 1 && inHandCardList.Count == 14
                                     && gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken()).turn)
        {
            var request = new GameTableRequest
            {
                table_id = gameTableEventHandler.gameTableResponse.id,
                table_event_type = Constants.GAME_TABLE_EVENT.PLAYER_TURN_FINISH
            };
            var sfsObject = request.ToISfsObject();
            sfsObject.PutUtfString(Constants.KEYS.discarded_card, JsonUtility.ToJson(selectedCards[0].cardModel));
            sfsObject.PutBool(Constants.KEYS.is_declare, isDeclare);
            sfsObject.PutUtfString(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
            SendGameTableRequest(sfsObject);

            if (!isDeclare)
            {
                var Pos = Vector3.zero;
                Pos = !IsDrag ? selectedCards[0].transform.position : Input.mousePosition;
                cardForAnimation.DiscardAnimation(Pos);
                DeselectAllCards();
                cardForAnimation.SetOpenCardSprite(selectedCards[0].GetCardSprite());
                cardForAnimation.gameObject.SetActive(true);
                FinishCardAnimation.gameObject.SetActive(false);
            }
            else
            {
                GameTableResponse.CardModel cardModel = selectedCards[0].cardModel;
                var spriteName = cardModel.suit + "_" + cardModel.rank;

                var Pos = selectedCards[0].transform.position;
                FinishCardAnimation.SetOpenCardSprite(GetSprite(spriteName));
                FinishCardAnimation.gameObject.SetActive(true);
                FinishCardAnimation.DiscardAnimation(Pos);
                cardForAnimation.gameObject.SetActive(false);
                DeselectAllCards();
            }

            AudioController.instance.StopSounds();
            AudioController.instance.PlayCardPick();
            inHandCardList.Remove(selectedCards[0]);
            selectedCards[0].transform.parent.SetParent(null);
            Destroy(selectedCards[0].transform.parent.gameObject);
        }
        else
        {
            gameTableEventHandler.messageInfo.ShowWithMessage("Please pick a card", 3);
        }
        SendGroup();
        CheckForEmptySLots();
        CheckLastCard();
    }

    public void OtherPlayersDiscardCard()
    {
        var userCard = gameTableEventHandler.gameTableResponse?.liveGameModel?.gamePlayCardModel?.discarded;
        if (userCard?.cardModel != null
            && !userCard.auth_token.Equals(PlayerPrefsManager.GetAuthToken()))
        {
            foreach (var player in gameTableEventHandler.playerHolder.players
                .Where(player => player.gameObject.activeInHierarchy)
                .Where(player => player.authToken.Equals(userCard.auth_token)))
            {
                var pos = player.transform.position;
                if (userCard.declare)
                {
                    var spriteName = userCard.cardModel.suit + "_" + userCard.cardModel.rank;
                    FinishCardAnimation.gameObject.SetActive(true);
                    FinishCardAnimation.SetOpenCardSprite(GetSprite(spriteName));
                    FinishCardAnimation.DiscardAnimation(pos);
                }
                else
                {
                    var name = userCard.cardModel.suit + "_" + userCard.cardModel.rank;
                    cardForAnimation.gameObject.SetActive(true);
                    cardForAnimation.SetOpenCardSprite(GetSprite(name));
                    cardForAnimation.DiscardAnimation(pos);
                }

            }
        }
        else if (userCard?.cardModel != null
                && userCard.auth_token.Equals(PlayerPrefsManager.GetAuthToken()))
        {
            if (inHandCardList.Count == 14)
            {
                var index = inHandCardList.Count - 1;

                for (int i = 0; i < inHandCardList.Count; i++)
                {
                    if (inHandCardList[i].cardModel.id == userCard.cardModel.id)
                    {
                        index = i;
                        break;
                    }
                }

                var Pos = inHandCardList[index].transform.position;

                cardForAnimation.DiscardAnimation(Pos);
                var spriteName = userCard.cardModel.suit + "_" + userCard.cardModel.rank;
                cardForAnimation.SetOpenCardSprite(GetSprite(spriteName));
                cardForAnimation.gameObject.SetActive(true);
                inHandCardList[index].transform.parent.SetParent(null);
                Destroy(inHandCardList[index].transform.parent.gameObject);
                inHandCardList.Remove(inHandCardList[index]);
                PlayerPrefs.DeleteKey(Constants.KEYS.Selection_Counter);
            }
        }
    }


    void OnDeckCard(CardSlideAnim cardAnim)
    {

        CheckLastCard();
        cardAnim.TakeOpenCard(RightLastCard.transform.position);
    }
    private void DragDiscard()
    {
        Vector3 Pos = CurrentCard.transform.position;
        cardForAnimation.SetOpenCardSprite(CurrentCard.GetCardSprite());
        inHandCardList.Remove(CurrentCard);

        CurrentCard.transform.parent.SetParent(null);
        Destroy(CurrentCard.transform.parent.gameObject);
        cardForAnimation.DiscardAnimation(Pos);
        CheckForEmptySLots();
    }

    public void OnCardAnimationEnd()
    {
        if (gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken()).turn
        && pickCardModel != null)
        {
            GenerateDrawCard(pickCardModel);

        }
    }

    void GenerateDrawCard(GameTableResponse.CardModel cardModel)
    {
        var name = cardModel.suit + "_" + cardModel.rank;
        var NewPlayerCard = Instantiate(CardPrefab, RightLastCard.GetParent());
        var PlayCard = NewPlayerCard.GetComponentInChildren<Card>();
        PlayCard.SetCardData(cardModel);
        NewPlayerCard.name = name;
        PlayCard.UpdateCurrentParent();
        inHandCardList.Add(PlayCard);
        pickCardModel = null;
    }

    //MergeBtn
    void CheckLastCard()
    {
        if (SlotsParent.transform.childCount > 0)
        {
            var LastSlot = SlotsParent.transform.GetChild(SlotsParent.transform.childCount - 1).transform;
            if (LastSlot.GetChild(0).childCount > 0)
            {
                RightLastCard = LastSlot.GetChild(0).GetChild(LastSlot.GetChild(0).childCount - 1).GetComponentInChildren<Card>();

                if (!isGameFinish)
                    RightLastCard.DeactivateBlocker();

                if (PrevRightLast != null)
                {
                    if (RightLastCard == PrevRightLast) return;
                    PrevRightLast.ActivateBlocker();
                    PrevRightLast = RightLastCard;
                }
                else
                {
                    PrevRightLast = RightLastCard;
                }
            }

        }
    }

    public Sprite GetSprite(string name)
    {
        return CardImages.CardSprite.FirstOrDefault(card => card.name.Equals(name));
    }

    public void SendGroup()
    {
        var cardModels = new List<GameTableResponse.CardModel>();
        for (var i = 0; i < SlotsParent.transform.childCount; i++)
        {
            var cardGroup = SlotsParent.transform.GetChild(i);
            Card[] cards = cardGroup.transform.GetComponentsInChildren<Card>();
            for (var j = 0; j < cards.Length; j++)
            {
                if (cards[j] == null) continue;
                cards[j].cardModel.groupId = i;
                cardModels.Add(cards[j].cardModel);
            }
        }


        if (cardModels.Count == 13)
        {
            var request = new GameTableRequest();
            request.table_id = gameTableEventHandler.gameTableResponse.id;
            request.table_event_type = Constants.GAME_TABLE_EVENT.GROUP_HAND_CARDS;
            var sfsObject = request.ToISfsObject();
            var cardListJson = Utils.JsonHelper.ToJson(cardModels);
            Debug.Log("CARD_cardListJson = " + cardListJson);
            sfsObject.PutUtfString(Constants.KEYS.card_group, cardListJson);
            SendGameTableRequest(sfsObject);
        }
    }

    public void DropRequest()
    {
        if (inHandCardList.Count > 0)
        {
            gameTableEventHandler.tableHeader.gameSettingMenu.gameObject.SetActive(false);
            gameTableEventHandler.isDropOnTimeOut = true;

            var playerModel = gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken());
            if (playerModel.turn)
            {
                isDropClicked = true;

                var dropMessage = Constants.MESSAGE.DROP_CONFIRMATION;
                if (playerModel.turnCounter <= 1 && playerModel.discardedCardList.Count == 0)
                {
                    float dropValue = gameTableEventHandler.gameTableResponse.gameModel.drop;
                    if (gameTableEventHandler.gameTableResponse.gameModel.game_sub_type == Constants.GAME_SUB_TYPE.POINT)
                    {
                        dropValue = dropValue * gameTableEventHandler.gameTableResponse.gameModel.point_value;
                        dropMessage = dropMessage + "\nYou will loose " + Constants.Country.currency_symbol + dropValue;
                    }
                    else
                    {
                        dropMessage = dropMessage + "\nYou will loose " + dropValue + " points";
                    }
                }
                else
                {
                    float dropValue = gameTableEventHandler.gameTableResponse.gameModel.middle_drop;
                    if (gameTableEventHandler.gameTableResponse.gameModel.game_sub_type == Constants.GAME_SUB_TYPE.POINT)
                    {
                        dropValue = dropValue * gameTableEventHandler.gameTableResponse.gameModel.point_value;
                        dropMessage = dropMessage + "\nYou will loose " + Constants.Country.currency_symbol + dropValue;
                    }
                    else
                    {
                        dropMessage = dropMessage + "\nYou will loose " + dropValue + " points";
                    }
                }

                ServerManager.instance.alertPopUp.ShowView(dropMessage, () =>
                {
                    playerModel = gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken());
                    SendGroup();
                    instance.gameTableEventHandler.buttonStats.isGameSarted = false;
                    if (playerModel.turn)
                    {
                        ShrinkOrStrechForDrop(false);
                        var request = new GameTableRequest();
                        request.table_id = gameTableEventHandler.gameTableResponse.id;
                        request.table_event_type = Constants.GAME_TABLE_EVENT.PLAYER_DROP;
                        var sfsObject = request.ToISfsObject();
                        sfsObject.PutUtfString(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
                        SendGameTableRequest(sfsObject);
                        isDropClicked = false;
                    }
                    else
                    {
                        StartCoroutine(gameTableEventHandler.messageInfo
                            .ShowMessage(Constants.MESSAGE.INVALID_TURN, 3f));
                    }
                    AudioController.instance.StopSounds();
                    DeselectAllCards();
                }, "Yes", () =>
                {
                    DeselectAllCards();
                    gameTableEventHandler.isDropOnTimeOut = false;
                }, "No");
            }
            else
            {
                StartCoroutine(gameTableEventHandler.messageInfo
                    .ShowMessage(Constants.MESSAGE.INVALID_TURN, 3f));
            }
        }
    }

    public void SetAllDeck(bool isSet)
    {
        inHandCards.SetActive(isSet);
        CloseDeckCards.SetActive(isSet);
        openDeckCards.SetActive(isSet);
        finishSlot.SetActive(isSet);
    }

    public void ResetGameplay()
    {
        ClearList();
        SetAllDeck(false);
    }

    public void ClearList()
    {
        foreach (var card in inHandCardList)
        {
            Destroy(card);
        }

        foreach (var group in inHandGroups)
        {
            Destroy(@group);
        }

        foreach (var card in selectedCards)
        {
            Destroy(card);
        }

        inHandCardList.Clear();
        inHandGroups.Clear();
        selectedCards.Clear();
    }

    public void MeldCardRequest()
    {
        var playerModel = gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken());
        if (playerModel.turn)
        {
            selfPlayerDiscardCard(true, false);
            //  gameTableEventHandler.meldCards.gameObject.SetActive(true);
            //  gameTableEventHandler.meldCards.UpdateTimer();
            UpdateTimer();
            // gameTableEventHandler.buttonStats.GroupBtnStats(true);
            // gameTableEventHandler.buttonStats.dropBtn.gameObject.SetActive(false);
            //   gameTableEventHandler.meldCards.ArrangeCardHolder();
        }
        else
        {
            StartCoroutine(gameTableEventHandler.messageInfo
                .ShowMessage(Constants.MESSAGE.INVALID_TURN, 3));
        }
    }

    private void CreateDropBackcard(bool intercatable = true)
    {
        if (DropBackcard == null)
        {
            DropBackcard = Instantiate(DropBackcardprefab, SlotsParent.transform);
        }

        Button DropbackBtn = DropBackcard.GetComponentInChildren<Button>();
        DropbackBtn.onClick.AddListener(OnDropBackCardBtn);
        DropbackBtn.interactable = intercatable;
        DropCardAnimBtn.gameObject.SetActive(false);
        SlotHolderBg.enabled = true;
    }

    private void OnDropBackCardBtn()
    {

        AnimationController.ShrinkOrStrechCards();
        if (AnimationController.IsInShrinkState)
        {
            foreach (Card card in inHandCardList)
            {
                card.DeactivateDropBlocker();
            }
            DropCardAnimBtn.gameObject.SetActive(false);
        }
        else
        {
            foreach (Card card in inHandCardList)
            {
                card.ActivateDropBlocker();
            }
            DropCardAnimBtn.gameObject.SetActive(true);
        }
    }

    public void ShrinkOrStrechForDrop(bool WrongShow)
    {
        if (DropBackcard == null)
        {
            AnimationController.ShrinkOrStrechCards();
            if (WrongShow)
            {
                CreateDropBackcard(intercatable: false);
            }
            else
            {
                CreateDropBackcard(intercatable: true);
                SetCardstoDropState();
            }
        }
    }

    void SetCardstoDropState()
    {
        foreach (var card in inHandCardList)
        {
            card.SetToDropState();
        }

        for (int i = 0; i < inHandGroups.Count; i++)
        {
            var group = inHandGroups[i];
            var cardModels = new List<GameTableResponse.CardModel>();
            ArrayList tempCards = new ArrayList();
            Card[] cards = group.transform.GetComponentsInChildren<Card>();

            foreach (var card in cards)
            {
                card.SetToDropState();
            }
        }
    }

    public void AutoDrop()
    {
        if (!gameTableEventHandler.buttonStats.isAutoDrop) return;
        SendGroup();
        gameTableEventHandler.buttonStats.isGameSarted = false;
        if (gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken()).turn)
        {
            var request = new GameTableRequest();
            request.table_id = gameTableEventHandler.gameTableResponse.id;
            request.table_event_type = Constants.GAME_TABLE_EVENT.PLAYER_DROP;
            var sfsObject = request.ToISfsObject();
            sfsObject.PutUtfString(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
            SendGameTableRequest(sfsObject);
            isDropClicked = false;
            gameTableEventHandler.buttonStats.autoDrop.isOn = false;
            gameTableEventHandler.buttonStats.isAutoDrop = false;
        }
        AudioController.instance.StopSounds();
    }





    private Coroutine updateTimerCoroutine = null;
    public bool meldCardRequest;
    public void UpdateTimer()
    {
        StopAllCoroutines();
        var gameModel = gameTableEventHandler.gameTableResponse.liveGameModel;
        var time = gameModel.gameDeclareTime - gameModel.baseStateModel.timer.elapsedCounter;
        if (updateTimerCoroutine != null)
        {
            StopCoroutine(updateTimerCoroutine);
        }
        updateTimerCoroutine = StartCoroutine(ResultCounter(time));
    }

    public IEnumerator ResultCounter(int timer)
    {
        var counter = 0;
        meldCardRequest = false;
        while (!meldCardRequest)
        {
            var timerDiff = (timer - counter);
            var timerInfo = "You have " + timerDiff.ToString() + " seconds left";
            gameTableEventHandler.messageInfo.declareAction = DeclareGameBeforeTimeOut;
            gameTableEventHandler.messageInfo.ShowWithMessage(timerInfo);
            gameTableEventHandler.messageInfo.DeclareButton(true);
            yield return new WaitForSeconds(1f);
            if (timerDiff <= 1)
            {
                SendMeldCard();
                meldCardRequest = true;
                gameTableEventHandler.messageInfo.gameObject.SetActive(false);
                if (updateTimerCoroutine != null)
                {
                    StopCoroutine(updateTimerCoroutine);
                }
            }

            counter++;
        }
    }

    public void DeclareGameBeforeTimeOut()
    {
        if (updateTimerCoroutine != null)
        {
            StopCoroutine(updateTimerCoroutine);
            gameTableEventHandler.messageInfo.gameObject.SetActive(false);
        }

        meldCardRequest = true;
        SendMeldCard();
    }

    [HideInInspector] public List<GameTableResponse.CardModel> cardModels = new List<GameTableResponse.CardModel>();
    private void SendMeldCard()
    {
        cardModels.Clear();
        var groupIds = new List<int>();


        for (var i = 0; i < SlotsParent.transform.childCount; i++)
        {
            var cardGroup = SlotsParent.transform.GetChild(i);
            if (cardGroup == null) continue;

            Card[] cards = cardGroup.transform.GetComponentsInChildren<Card>();
            for (var j = 0; j < cards.Length; j++)
            {
                if (cards[j] == null) continue;
                if (groupIds.Count > 0)
                {
                    cards[j].cardModel.groupId = groupIds.Max() + i;
                }
                else
                {
                    cards[j].cardModel.groupId = i;
                }

                if (cards[j].cardModel.groupId >= 6)
                    cards[j].cardModel.groupId = 5;
                cardModels.Add(cards[j].cardModel);
            }
        }

        if (cardModels.Count != 13)
        {
            StartCoroutine(gameTableEventHandler.messageInfo.ShowMessage("Wait for your show", 3));
            return;
        }

        var request = new GameTableRequest();
        request.table_id = gameTableEventHandler.gameTableResponse.id;
        request.table_event_type = Constants.GAME_TABLE_EVENT.REQUEST_MELD_CARD;
        var sfsObject = request.ToISfsObject();
        var cardListJson = Utils.JsonHelper.ToJson(cardModels);
        sfsObject.PutUtfString(Constants.KEYS.card_group, cardListJson);
        sfsObject.PutUtfString(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        SendGameTableRequest(sfsObject);
    }

    private int nextUpdate = 2;
    public int totalInvalidCount = 0;
    private void CheckForInHandCardLable()
    {
        if (Time.time >= nextUpdate)
        {
            nextUpdate = Mathf.FloorToInt(Time.time) + 1;
            if (inHandGroups.Count > 0)
            {
                totalInvalidCount = 0;
                var needToShowCount = NeedToShowCount();
                for (int i = 0; i < inHandGroups.Count; i++)
                {
                    var group = inHandGroups[i];
                    var cardModels = new List<GameTableResponse.CardModel>();
                    ArrayList tempCards = new ArrayList();
                    Card[] cards = group.transform.GetComponentsInChildren<Card>();
                    GameObject mainPanel = group.transform.GetChild(1).transform.gameObject;


                    Image validatorImage = group.transform.GetChild(1).GetChild(0).GetComponent<Image>();
                    TextMeshProUGUI validatorText = group.transform.GetComponentInChildren<TextMeshProUGUI>();
                    tempCards.AddRange(cards);


                    bool isPureValidSequence = CardValidator.isPureValidSequence(tempCards);
                    bool isValidSequence = CardValidator.isValidSequence(tempCards);
                    bool validSet = CardValidator.isValidSet(tempCards);


                    if (!isPureValidSequence && !isValidSequence && !validSet)
                    {
                        int invalidCount = InvalidSetCount(tempCards);
                        totalInvalidCount = totalInvalidCount + invalidCount;
                        validatorText.text = "Invalid(" + invalidCount + ")";
                        validatorText.color = Color.white;
                        validatorImage.color = Color.red;
                    }
                    else if (isPureValidSequence == true)
                    {
                        validatorText.text = "Pure Sequence";
                        validatorText.color = Color.white;
                        validatorImage.color = new Color32(60, 107, 41, 255);
                    }
                    else if (isValidSequence == true)
                    {
                        validatorText.text = "Sequence";
                        if (needToShowCount == true)
                        {
                            int invalidCount = InvalidSetCount(tempCards);
                            totalInvalidCount = totalInvalidCount + invalidCount;
                            validatorText.text = "Sequence(" + invalidCount + ")";
                        }
                        validatorText.color = Color.white;
                        validatorImage.color = new Color32(60, 107, 41, 255);
                    }
                    else if (validSet == true)
                    {
                        validatorText.text = "Set";
                        if (needToShowCount == true)
                        {
                            int invalidCount = InvalidSetCount(tempCards);
                            totalInvalidCount = totalInvalidCount + invalidCount;
                            validatorText.text = "Set(" + invalidCount + ")";
                        }
                        validatorImage.color = new Color32(255, 165, 0, 255);
                        validatorText.color = Color.white;
                    }
                    else
                    {
                        int invalidCount = InvalidSetCount(tempCards);
                        totalInvalidCount = totalInvalidCount + invalidCount;
                        validatorText.text = "Invalid(" + invalidCount + ")";
                        validatorText.color = Color.white;
                        validatorImage.color = new Color32(207, 85, 85, 255);
                    }
                }
                gameTableEventHandler.playerHolder.players.Find(model => model.authToken == PlayerPrefsManager.GetAuthToken()).SetPlayerScoreForPointGame();
            }
        }
    }

    private bool NeedToShowCount()
    {
        var isPureSequence = true;
        List<int> indexForCounter = new List<int>();
        for (int i = 0; i < inHandGroups.Count; i++)
        {
            var group = inHandGroups[i];
            ArrayList tempCards = new ArrayList();
            Card[] cards = group.transform.GetComponentsInChildren<Card>();
          //  Debug.Log("your group transform is" + group.transform);
            tempCards.AddRange(cards);
            bool isPureValidSequence = CardValidator.isPureValidSequence(tempCards);
            if (isPureValidSequence == true)
            {
                indexForCounter.Add(i);
            }
        }

        if (indexForCounter.Count >= 2)
        {
            return false;
        }
        else if (indexForCounter.Count == 1)
        {
            for (int i = 0; i < inHandGroups.Count; i++)
            {
                var group = inHandGroups[i];
                ArrayList tempCards = new ArrayList();
                Card[] cards = group.transform.GetComponentsInChildren<Card>();
                tempCards.AddRange(cards);
                bool isValidSequence = CardValidator.isValidSequence(tempCards);
                if (isValidSequence == true)
                {
                    if (indexForCounter.Contains(i) == false)
                    {
                        return false;
                    }
                }
            }
        }
        return isPureSequence;
    }

    private int InvalidSetCount(ArrayList paramArrCards)
    {
        int flag = 0;
        foreach (Card cardModel in paramArrCards)
        {
            if (cardModel.cardModel.IsJoker() == false)
            {
                if (cardModel.cardModel.rank >= 10 || cardModel.cardModel.rank == 1)
                {
                    flag += 10;
                }
                else
                {
                    flag += cardModel.cardModel.rank;
                }
            }
        }
        return flag;
    }
}
