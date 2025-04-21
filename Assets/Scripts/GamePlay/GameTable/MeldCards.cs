using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MeldCards : TableBaseMono
{
    public GameObject meldCardParent;
    public GameObject meldCardSlot;
    public Button submitBtn;
    public GameObject mergeBtnPrefab;
    public TextMeshProUGUI timerInfo;
    public bool meldCardRequest;

    List<GameObject> meldCardsParent = new List<GameObject>();
    List<Button> closeBtns = new List<Button>();
    List<Button> mergeBtns = new List<Button>();
    GameObject buttonOnSelection;
    public bool isMeldedCardGroupChange;
    private bool isSelectedCardCountZero;



    private void OnEnable()
    {
        ResetMeldCards();
    }

    private void OnDisable()
    {
        meldCardRequest = true;
        if(updateTimerCoroutine != null)
            StopCoroutine(updateTimerCoroutine);
    }

    private void Start()
    {
        isSelectedCardCountZero = true;
        isMeldedCardGroupChange = false;
        submitBtn.onClick.AddListener(SubmitMeldedCards);
    }

    private void Update()
    {
      //  RecreateMergeButtonOnRuntime();
    }

    private void RecreateMergeButtonOnRuntime()
    {
        var selectedCards = GamePlayManager.instance.selectedCards;
        
        if (selectedCards.Count > 0 && selectedCards != null)
        {
            isSelectedCardCountZero = false;
            RemoveUnselectedCardFromList(selectedCards);
            
            for (var i = 0; i < selectedCards.Count; i++)
            {
                if (selectedCards.Count > 1 && selectedCards[i].count > 0
                                            && selectedCards[i].isMeldCardSlected)
                {
                    selectedCards[i].isMeldCardSlected = false;
                    if (buttonOnSelection != null)
                        Destroy(buttonOnSelection);
                    
                    buttonOnSelection = Instantiate(mergeBtnPrefab,
                        selectedCards[selectedCards.Count - 1].gameObject.transform);

                    buttonOnSelection.gameObject.name = "MergeBtn";
                    MeldSelectedCardsOnButtonClick(selectedCards);

                    if (selectedCards[i].count == 0)
                    {
                        Destroy(buttonOnSelection);
                    } 
                }
            }
        }

        if (selectedCards.Count == 0 && !isSelectedCardCountZero)
        {
            isSelectedCardCountZero = true;
            Destroy(buttonOnSelection);
            //GenerateMergeBtn(false);
        }

        if (mergeBtns.Count <= 0) return;
        if (GamePlayManager.instance.selectedCards.Count > 0)
        {
            foreach (var selectedCard in GamePlayManager.instance.selectedCards
                .Where(selectedCard => selectedCard.count <= 1))
            {
                foreach (var mergeBtn in mergeBtns.Where(Btns => Btns != null))
                {
                    Destroy(mergeBtn.gameObject);
                }

                mergeBtns.Clear();
            }
        }

        if (!isMeldedCardGroupChange || mergeBtns.Count <= 0) return;
        {
            foreach (var mergeBtn in mergeBtns.Where(Btns => Btns != null))
            {
                Destroy(mergeBtn.gameObject);
            }

            mergeBtns.Clear();
            //GenerateMergeBtn(false);
            isMeldedCardGroupChange = false;
        }
    }

    private static void RemoveUnselectedCardFromList(List<Card> selectedCards)
    {
        if (selectedCards.Count > 0 && selectedCards != null)
        {
            for (var j = selectedCards.Count - 1; j >= 0; j--)
            {
                if (!selectedCards[j].IsCardSelected)
                {
                    selectedCards.RemoveAt(j);
                }
            }
        }
    }

    private void MeldSelectedCardsOnButtonClick(List<Card> selectedCards)
    {
        buttonOnSelection.GetComponent<Button>().onClick.AddListener(() =>
        {
            foreach (var meldedCardParent in meldCardsParent
                .Select(parent => parent.transform.GetChild(2).GetComponent<ScrollRect>().content)
                .Where(meldedCardParent => meldedCardParent.childCount < 2))
            {
                foreach (var selectedCard in selectedCards)
                {
                    var cardImage = Instantiate(GamePlayManager.instance.CardPrefab,
                        meldedCardParent.transform);
                    cardImage.GetComponent<Draggables>().enabled = false;
                    var card = cardImage.GetComponentInChildren<Card>();
                    card.GetComponent<Image>().raycastTarget = false; //SAI
                    card.SetCardData(selectedCard.cardModel);

                    cardImage.gameObject.SetActive(true);

                    DestroyImmediate(selectedCard.gameObject.transform.parent.gameObject);
                    isMeldedCardGroupChange = true;
                }

                GamePlayManager.instance.selectedCards.Clear();
                GamePlayManager.instance.DeselectAllCards();
                Destroy(buttonOnSelection);
                StartCoroutine(RegenarateButtonOnTimeDelay());
                break;
            }
        });
    }

    private IEnumerator RegenarateButtonOnTimeDelay()
    {
        yield return new WaitForSeconds(0.1f);
        //GenerateMergeBtn(false);
    }

    public void ArrangeCardHolder()
    {
        if (meldCardsParent.Count > 0)
        {
            for (var i = 1; i < meldCardsParent.Count; i++)
            {
                Destroy(meldCardsParent[i]);
            }
        }

        for (var i = 0; i < 4; i++)
        {
            var cardSlots = Instantiate(meldCardSlot, meldCardParent.transform);
            cardSlots.SetActive(true);
            meldCardsParent.Add(cardSlots);
            closeBtns.Add(cardSlots.transform.GetChild(1).GetComponent<Button>());
        }

        if (closeBtns.Count <= 0) return;
        for (var i = 0; i < closeBtns.Count; i++)
        {
            CloseOfMeldCard(closeBtns, i);
        }

        //GenerateMergeBtn(false);

        if (gameTableEventHandler.playerHolder.IsSelfPlayer())
        {
            foreach (var player in gameTableEventHandler.playerHolder.players)
            {
                if (player.authToken.Equals(PlayerPrefsManager.GetAuthToken()))
                {
                    player.ResetTime();
                    Debug.Log("Timer is Reset");
                }
            }
        }
        else
        {
            gameTableEventHandler.playerHolder.ResetPlayersTurn();
        }
    }

    public void GenerateMergeBtn(bool isGen)
    {
        if (isGen) return;

        for (var i = 1; i < GamePlayManager.instance.SlotsParent.transform.childCount; i++)
        {
            
            if (GamePlayManager.instance.SlotsParent.transform.GetChild(i).childCount > 1)
            {
                var mergeBtn = Instantiate(mergeBtnPrefab, GamePlayManager.instance.SlotsParent.transform.GetChild(i));
                mergeBtn.gameObject.name = "MergeBtn";
                mergeBtns.Add(mergeBtn.GetComponent<Button>());
            }
            else if(GamePlayManager.instance.SlotsParent.transform.GetChild(i).childCount <= 0)
            {
                Destroy(GamePlayManager.instance.SlotsParent.transform.GetChild(i).gameObject);
            }
        }

        for (var j = 0; j < mergeBtns.Count; j++)
        {
            GetMeldedCards(mergeBtns, j);
        }

        isGen = true;
    }

    private void GetMeldedCards(List<Button> mergeBtns, int index)
    {
        GamePlayManager.instance.selectedCards.Clear();
        PlayerPrefs.DeleteKey(Constants.KEYS.Selection_Counter);
        mergeBtns[index].onClick.AddListener(() =>
        {
            for (int i = 0; i < meldCardsParent.Count; i++)
            {
                var meldedCardParent = meldCardsParent[i].transform.GetChild(2).GetComponent<ScrollRect>().content.gameObject;
                if (meldedCardParent.transform.childCount < 2)
                {
                    
                    for (var j = 0; j < mergeBtns[index].transform.parent.childCount; j++)
                    {
                        if (!mergeBtns[index].transform.parent.GetChild(j).gameObject.activeInHierarchy) continue;
                        var name = mergeBtns[index].transform.parent.GetChild(j).gameObject.name;

                        if (name != "MergeBtn")
                        {
                            var cardImage = Instantiate(GamePlayManager.instance.CardPrefab, meldedCardParent.transform);
                            cardImage.transform.GetComponent<Draggables>().enabled = false;
                            var card = cardImage.GetComponentInChildren<Card>();
                            card.GetComponent<Image>().raycastTarget = false;//SAI
                            card.SetCardData(mergeBtns[index].transform.parent.GetChild(j).GetChild(0).GetComponent<Card>().cardModel);
                            cardImage.gameObject.SetActive(true);
                        }

                    }
                    Destroy(mergeBtns[index].transform.parent.gameObject);
                    break;
                }
            }
        });
    }

    private void CloseOfMeldCard(List<Button> closeBtns, int index)
    {
        if (index >= closeBtns.Count) return;
        if (meldCardsParent[index].transform.GetChild(2).GetComponent<ScrollRect>().content.childCount > 0)
        {
           
                closeBtns[index].onClick.AddListener(() =>
                {
                    if (GamePlayManager.instance.SlotsParent.transform.childCount < 7)
                    {
                        var tempParent = Instantiate(GamePlayManager.instance.CardsContainerPrefab,
                            GamePlayManager.instance.SlotsParent.transform);
                        tempParent.SetActive(true);
                        
#if UNITY_WEBGL
                        tempParent.GetComponent<HorizontalLayoutGroup>().spacing = -Constants.CARD_SPACE.WEBGL_CARD;
#else
                        tempParent.GetComponent<HorizontalLayoutGroup>().spacing = -Constants.CARD_SPACE.OTHER_CARD;
#endif


                        for (var i = 1;
                            i < meldCardsParent[index].transform.GetChild(2).GetComponent<ScrollRect>().content
                                .childCount;
                            i++)
                        {
                            var card = meldCardsParent[index].transform.GetChild(2).GetComponent<ScrollRect>().content
                                .GetChild(i)
                                .GetChild(0)
                                .GetComponent<Card>();

                            var name = card.cardModel.suit + "_" + card.cardModel.rank;
                            var CardParent = Instantiate(GamePlayManager.instance.CardPrefab, tempParent.transform);
                            var _currentCard = CardParent.GetComponentInChildren<Card>();
                            _currentCard.SetCardData(card.cardModel);
                            GamePlayManager.instance.inHandCardList.Add(_currentCard); //SAI
                            CardParent.name = name;
                        }

                        for (var j = 1;
                            j < meldCardsParent[index].transform.GetChild(2).GetComponent<ScrollRect>().content
                                .childCount;
                            j++)
                        {
                            Destroy(meldCardsParent[index].transform.GetChild(2).GetComponent<ScrollRect>().content
                                .GetChild(j)
                                .gameObject);
                        }


                        if (mergeBtns.Count <= 0) return;
                        {
                            foreach (var Btns in mergeBtns)
                            {
                                if (Btns != null)
                                    Destroy(Btns.gameObject);
                            }

                            mergeBtns.Clear();
                            //GenerateMergeBtn(false);
                        }
                    }
                });
        }
    }

   [HideInInspector]public List<GameTableResponse.CardModel> cardModels = new List<GameTableResponse.CardModel>();
    private void SendMeldCard()
    {
        cardModels.Clear();
        var groupIds = new List<int>();

        for (var i = 1; i < meldCardParent.transform.childCount; i++)
        {
            var cardGroup = meldCardParent.transform.GetChild(i).GetChild(2).GetComponent<ScrollRect>().content;
            if (cardGroup.childCount <= 0) continue;
            for (var j = 1; j < cardGroup.childCount; j++)
            {
                if (cardGroup.GetChild(j).gameObject.activeInHierarchy)
                {
                    var cardGameObj = cardGroup.transform.GetChild(j);
                    var card = cardGameObj.transform.GetChild(0).GetComponent<Card>();
                    card.cardModel.groupId = i - 1;
                    if (!groupIds.Contains(card.cardModel.groupId))
                    {
                        groupIds.Add(card.cardModel.groupId);
                    }
                    cardModels.Add(card.cardModel);
                }
            }
        }

        for (var i = 1; i < GamePlayManager.instance.SlotsParent.transform.childCount; i++)
        {
            var cardGroup = GamePlayManager.instance.SlotsParent.transform.GetChild(i);

            if (cardGroup.childCount <= 0) continue;
            for (var j = 0; j < cardGroup.childCount; j++)
            {
                if (!cardGroup.GetChild(j).gameObject.activeInHierarchy) continue;
                var cardGameObj = cardGroup.transform.GetChild(j);
                if (cardGameObj == null) continue;
                if (cardGameObj.gameObject.name == "MergeBtn" || cardGameObj.gameObject.name == "Dropcard") continue;

                if (cardGameObj.transform.GetChild(0) == null) continue;
                var card = cardGameObj.transform.GetChild(0).GetComponent<Card>();



                if (card == null) continue;
                if (groupIds.Count > 0)
                {
                    card.cardModel.groupId = groupIds.Max() + i;
                }
                else
                {
                    card.cardModel.groupId = i - 1;
                }

                if (card.cardModel.groupId >= 6)
                    card.cardModel.groupId = 5;

                cardModels.Add(card.cardModel);
            }
        }

        if (cardModels.Count != 13) return;
        var request = new GameTableRequest();
        request.table_id = gameTableEventHandler.gameTableResponse.id;
        request.table_event_type = Constants.GAME_TABLE_EVENT.REQUEST_MELD_CARD;
        var sfsObject = request.ToISfsObject();
        var cardListJson = Utils.JsonHelper.ToJson(cardModels);
        sfsObject.PutUtfString(Constants.KEYS.card_group, cardListJson);
        sfsObject.PutUtfString(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        SendGameTableRequest(sfsObject);
    }

    public IEnumerator ResultCounter(int timer)
    {
        var counter = 0;
        meldCardRequest = false;
        gameObject.SetActive(true);
        while (!meldCardRequest)
        {
            timerInfo.text = "You have " + (timer - counter).ToString() + " seconds left";
            yield return new WaitForSeconds(1f);
            if (timer <= counter)
            {
                meldCardRequest  = true;
                if (updateTimerCoroutine != null)
                {
                    StopCoroutine(updateTimerCoroutine);
                }
            }

            if ((timer - counter) <= 2f)
            {
                SendMeldCard();
                ResetMeldCards();
            }
            
            counter++;
        }

       
    }

    private void SendAllCardToSlot()
    {
        var index = 0;
        foreach (var cardParent in meldCardsParent)
        {
            var meldedCardParent = cardParent.transform.GetChild(2).GetComponent<ScrollRect>().content.gameObject;
            if (meldedCardParent.transform.childCount >= 2) continue;
            if (GamePlayManager.instance.SlotsParent.transform.childCount <= 1
                && !GamePlayManager.instance.SlotsParent.transform.GetChild(index).gameObject
                    .activeInHierarchy) continue;
            while (meldedCardParent.transform.childCount <= 1 && index < GamePlayManager.instance.SlotsParent.transform.childCount - 1)
            {
                if (GamePlayManager.instance.SlotsParent.transform.GetChild(index).childCount <= 1 &&
                    GamePlayManager.instance.SlotsParent.transform.GetChild(index).gameObject != null 
                    && index < GamePlayManager.instance.SlotsParent.transform.childCount)
                {
                    index++;
                }

                if (GamePlayManager.instance.SlotsParent.transform.GetChild(index).gameObject != null 
                    && GamePlayManager.instance.SlotsParent.transform.GetChild(index).childCount > 1)
                {
                    for (var k = 0; k < GamePlayManager.instance.SlotsParent.transform.GetChild(index).childCount; k++)
                    {
                        var cardName = GamePlayManager.instance.SlotsParent.transform.GetChild(index).GetChild(k)
                            .gameObject.name;
                        if (cardName == "MergeBtn") continue;
                        var cardImage = Instantiate(GamePlayManager.instance.CardPrefab, meldedCardParent.transform);
                        cardImage.transform.GetComponent<Draggables>().enabled = false;
                        var card = cardImage.GetComponentInChildren<Card>();
                        card.GetComponent<Image>().raycastTarget = false;
                        card.SetCardData(GamePlayManager.instance.SlotsParent.transform.GetChild(index).GetChild(k)
                            .GetChild(0).GetComponent<Card>().cardModel);
                        cardImage.gameObject.SetActive(true);
                    }

                    DestroyImmediate(GamePlayManager.instance.SlotsParent.transform.GetChild(index).gameObject);
                   
                }
            }
            index = 0;
        }
    }

    private void SubmitMeldedCards()
    {
        SendAllCardToSlot();
        SendMeldCard();
        ResetMeldCards();
      /*  ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.MELD_CONFIRMATION, () =>
        {
            
        }, "Yes", () => { }, "No"); */
    }

    public void ResetMeldCards()
    {
        if(meldCardsParent.Count <= 0) return;
        foreach (var cardParent in meldCardsParent)
        {
            if(cardParent != null)
                Destroy(cardParent);
        }
        meldCardsParent.Clear();

        if (closeBtns.Count <= 0) return;
        foreach (var closeButtons in closeBtns)
        {
            if(closeButtons != null)
                Destroy(closeButtons);
        }
        closeBtns.Clear();

        if (mergeBtns.Count <= 0) return;
        foreach (var mergeButtons in mergeBtns)
        {
            if(mergeButtons != null)
                Destroy(mergeButtons.gameObject);
        }
        mergeBtns.Clear();

        cardModels.Clear();
        isMeldedCardGroupChange = false;
        meldCardRequest = false;
        gameObject.SetActive(false);
    }

    public bool IsExist(GameTableResponse.CardModel cardModel) {
        for (var i = 1; i < meldCardParent.transform.childCount; i++)
        {
            var cardGroup = meldCardParent.transform.GetChild(i).GetChild(2).GetComponent<ScrollRect>().content;
            if (cardGroup.childCount <= 0) continue;
            for (var j = 1; j < cardGroup.childCount; j++)
            {
                if (cardGroup.GetChild(j).gameObject.activeInHierarchy)
                {
                    var cardGameObj = cardGroup.transform.GetChild(j);
                    var card = cardGameObj.transform.GetChild(0).GetComponent<Card>();

                    if (card.cardModel.id.Equals(cardModel.id)) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private Coroutine updateTimerCoroutine = null;
    public void UpdateTimer()
    {
        var gameModel = gameTableEventHandler.gameTableResponse.liveGameModel;

        var time = gameModel.gameDeclareTime - gameModel.baseStateModel.timer.elapsedCounter;
        if (updateTimerCoroutine != null)
        {
            StopCoroutine(updateTimerCoroutine);
        }
        updateTimerCoroutine = StartCoroutine(ResultCounter(time));
    }
}
