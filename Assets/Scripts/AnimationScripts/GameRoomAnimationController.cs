using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

public class GameRoomAnimationController : MonoBehaviour
{


    public Transform SpawingPoint;

    public Queue<GameObject> OpeningCards;
    public GameObject SlotsHolder;
    public GameObject animPooler;
    public GameObject animCard;
    public Animator deckCardAnimController;
    public List<BackCardAnim> users;
    public DeckCard PlayCards;
    public float offset;

    public Ease AnimEase = Ease.Linear;
    IEnumerator SpawnAnim;
    [HideInInspector]
    public UnityEvent DistributionEnd;
    public Transform ClosedDeck, OpenDeck, OpenJoker;
    public List<BackCardAnim> OpenAndClosedDeck;
    public bool IsInShrinkState;

    private void Start()
    {
        DistributionEnd = new UnityEvent();
        InstatiateFakeCards();
        //  DistributionEnd.AddListener(SceneController.instance.GenerateGame);
    }

    void InstatiateFakeCards()
    {
        OpeningCards = new Queue<GameObject>();

        for (int i = 0; i < users.Count * 2; i++)
        {
            GameObject NewCardm = Instantiate(PlayCards.DeckcardPrefab, SpawingPoint);
            NewCardm.SetActive(false);
            OpeningCards.Enqueue(NewCardm);
        }
    }

    public void StartDistribution()
    {
#if UNITY_WEBGL
        SlotsHolder.GetComponent<HorizontalLayoutGroup>().spacing = -Constants.CARD_SPACE.WEBGL_SLOT;
#else
        SlotsHolder.GetComponent<HorizontalLayoutGroup>().spacing = -Constants.CARD_SPACE.OTHER_SLOT;
#endif

        IsInShrinkState = true;
        AudioController.instance.PlayCardsDealing();
        StartCoroutine(SpawnCards());
    }

    public static bool isAnimDone = false;
    IEnumerator SpawnCards()
    {
        isAnimDone = false;
        GamePlayManager.instance.isDeckAnimDone = false;
        animPooler.SetActive(true);
        //for (int i = 0; i < 5; i++)
        //{

        //    for (int j = 0; j < users.Count; j++)
        //    {
        //        if (users[j].CardEndTarget.gameObject.activeInHierarchy)
        //        {
        //            SpawnEachCard(users[j]);
        //            yield return new WaitForSeconds(0.05f);
        //        }
        //        else
        //        {
        //            continue;
        //        }
        //    }

        //}


        SpawnAnim = null;
       // yield return new WaitForSeconds(1f);


        foreach (var card in OpeningCards)
        {
            card.transform.localPosition = new Vector3(0f,200f,0f);
            card.transform.eulerAngles = Vector3.zero;
            card.transform.localScale = Vector3.one;
            card.SetActive(true);
        }
        yield return new WaitForSeconds(0.2f);
        GameObject obj = animCard;
        float x = -600;
        List<GameObject> animCards = new List<GameObject>();
        for (int i = 0; i < 13; i++)
        {
            GameObject rocketClone = (GameObject)Instantiate(obj, new Vector3(0, 0f, 0), animPooler.transform.rotation,animPooler.transform);
            animCards.Add(rocketClone);
            rocketClone.transform.localPosition = new Vector3(0f,200f,0f);
            rocketClone.transform.DOLocalMove(new Vector3(x, -90f,0f), 0.7f);

            yield return new WaitForSeconds(0.1f);

            x += 100;
        }

        var player = GameTableEventHandler.Instance.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken());
        if (player != null)
        {
            var groupIds = new List<int>();
            foreach (var cardModel in player.inHandCardList.Where(cardModel => !groupIds.Contains(cardModel.groupId)))
            {
                groupIds.Add(cardModel.groupId);
            }
            int index = 0;
            for (var i = 0; i < groupIds.Count; i++)
            {
                foreach (var cardModel in player.inHandCardList)
                {
                    var name = cardModel.suit + "_" + cardModel.rank;
                    animCards[index].GetComponent<PreApplyCardAnim>().SetCardImage(GamePlayManager.instance.GetSprite(name));
                    index++;
                }
            }
        }


        foreach (var item in animCards)
        {
            item.transform.DORotate(new(0f, 180f, 0f), 0.2f);
            yield return new WaitForSeconds(0.1f);
        }



        yield return new WaitForSeconds(0.5f);
        animPooler.SetActive(false);
        foreach (var item in animCards)
        {
            Destroy(item);
        }
        animCards.Clear();

        DistributionEnd.Invoke();
        ResetCards();
        ShrinkOrStrechCards();
        isAnimDone = true;
        deckCardAnimController.enabled = true;
        deckCardAnimController.Play(0);
        DeckCardsAnimation.Instance.SetNeededCardsState();
        DeckCardsAnimation.Instance.SetScreenBlockerState(true);
       // GamePlayManager.instance.SortBtn.gameObject.SetActive(true);
    }


    void SpawnEachCard(BackCardAnim animDetails)
    {
        GameObject CardToSpawn = OpeningCards.Dequeue();
        CardToSpawn.SetActive(true);

        CardToSpawn.GetComponent<BackCard>().OnCardSpawn(animDetails, AnimEase);

        OpeningCards.Enqueue(CardToSpawn);
    }


    void ResetCards()
    {

        foreach (var card in OpeningCards)
        {
            card.transform.localPosition = Vector3.zero;
            card.transform.eulerAngles = Vector3.zero;
            card.SetActive(false);
        }
    }


    IEnumerator OpenJokerAnim()
    {
        SpawingPoint.gameObject.SetActive(false);
        OpenDeck.transform.DOMove(OpenAndClosedDeck[0].CardEndTarget.position, 0.6f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.45f);
        OpenJoker.transform.DOMove(OpenAndClosedDeck[1].CardEndTarget.position, 0.6f).SetEase(Ease.OutBack);
        OpenJoker.transform.DORotate(OpenAndClosedDeck[1].CardEndRotation, 0.6f).SetEase(Ease.OutBack);
        ClosedDeck.transform.DOMove(OpenAndClosedDeck[2].CardEndTarget.position, 0.6f).SetEase(Ease.OutBack);
    }


    public void ShrinkOrStrechCards()
    {

        HorizontalLayoutGroup[] layouts = SlotsHolder.GetComponentsInChildren<HorizontalLayoutGroup>();
        float CurrentSpacing = 0;
        if (IsInShrinkState)
        {
#if UNITY_WEBGL
            CurrentSpacing = -Constants.CARD_SPACE.WEBGL_CARD;
#else
            CurrentSpacing = -Constants.CARD_SPACE.OTHER_CARD;
#endif


            IsInShrinkState = false;
        }
        else
        {

#if UNITY_WEBGL
            CurrentSpacing = -Constants.CARD_SPACE.OTHER_SLOT;
#else
            CurrentSpacing = -Constants.CARD_SPACE.OTHER_SLOT;
#endif

            IsInShrinkState = true;
        }

        foreach (var layout in layouts)
        {
            if (layout == SlotsHolder.GetComponent<HorizontalLayoutGroup>() && IsInShrinkState == false)
            {
#if UNITY_WEBGL
                DOTween.To(() => layout.spacing, x => layout.spacing = x, Constants.CARD_SPACE.WEBGL_SLOT, 0.5f);
#else
                DOTween.To(() => layout.spacing, x => layout.spacing = x, Constants.CARD_SPACE.OTHER_SLOT, 0.5f);
#endif

            }
            else
            {
                DOTween.To(() => layout.spacing, x => layout.spacing = x, CurrentSpacing, 0.5f);
            }
        }
    }
}

[System.Serializable]
public class DeckCard
{
    public GameObject DeckcardPrefab;
    public int NumOfCards;

}
[System.Serializable]
public class BackCardAnim
{
    public Vector3 CardEndSize;
    public Transform CardEndTarget;
    public Vector3 CardEndRotation;
    public float offset;

}
