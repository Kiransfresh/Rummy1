using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagneticScrollView;
using TMPro;
using UnityEngine.UI;

public class DealRummyView : BaseMonoBehaviour
{
    public SlidingEffect[] slidingEffect;
    public Button backBtn;
    public GameObject entryFeePrefab;
    public GameObject entryFeeHolder;


    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    private int numberOfPlayer;

    [Header("Number Of Players Buttons")]
    [SerializeField] private Button twoPlayerBtn;
    [SerializeField] private Button fourPlayerBtn;
    [SerializeField] private Button sixPlayerBtn;

    [Header("Message Text")]
    [SerializeField] private TextMeshProUGUI messageText;

    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        PlayerSelectionToggle(Constants.SEAT.PLAYER_2);
        StartCoroutine(DealRummyEntryEffect());
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() =>
        {
            //UIManager.instance.ClearEntryFeeList(dealGameEntryFee);
            DisableDealRummy();
        });

    }

    private void PlayStartEffects()
    {

        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].EntryEffect());
        }

    }

    private void PlayEndEffect()
    {
        for (int i = 0; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].ExitEffect());
        }
    }

    public void PlayerSelectionToggle(int players)
    {
        if (players == Constants.SEAT.PLAYER_2)
        {
            twoPlayerBtn.GetComponent<Image>().color = Color.green;
            fourPlayerBtn.GetComponent<Image>().color = Color.white;
            sixPlayerBtn.GetComponent<Image>().color = Color.white;
        }
        else if (players == Constants.SEAT.PLAYER_4)
        {
            twoPlayerBtn.GetComponent<Image>().color = Color.white;
            fourPlayerBtn.GetComponent<Image>().color = Color.green;
            sixPlayerBtn.GetComponent<Image>().color = Color.white;
        }
        else if (players == Constants.SEAT.PLAYER_6)
        {
            twoPlayerBtn.GetComponent<Image>().color = Color.white;
            fourPlayerBtn.GetComponent<Image>().color = Color.white;
            sixPlayerBtn.GetComponent<Image>().color = Color.green;
        }

        CacheMemory.NumberOfPlayers = players.ToString();
        FetchDealGameList(players);
        Debug.Log("Fecthed Players : " + players);
    }

    List<GameListModel> gameListModel = new List<GameListModel>();

    private void FetchDealGameList(int numberOfPlayers)
    {
        ClearEntryFeeHolder();
        if (CacheMemory.GameType.Equals(Constants.GAME_TYPE.PRACTICE)
            || CacheMemory.GameType.Equals(Constants.GAME_TYPE.CASH))
        {
            var gamelist = GameListFilter.GetGames(CacheMemory.GameType, Constants.GAME_SUB_TYPE.DEALS, null);
            gameListModel = new List<GameListModel>();
            foreach (var model in gamelist)
            {
                if (model.number_of_players_per_table == numberOfPlayers)
                {
                    var cell = Instantiate(entryFeePrefab);
                    cell.transform.SetParent(entryFeeHolder.transform);
                    cell.transform.localScale = Vector3.one;
                    cell.GetComponent<EntryFee>().SetGameData(model: model);
                    gameListModel.Add(model);
                }
            }
            // StartCoroutine(UIManager.instance.RefreshEntryFee(pointGameEntryFee, 0.2f));
            if (gameListModel.Count <= 0)
            {
                messageText.gameObject.SetActive(true);
            }
            else
            {
                messageText.gameObject.SetActive(false);
            }
        }
    }

    private void ClearEntryFeeHolder()
    {
        for (int count = 0; count < entryFeeHolder.transform.childCount; count++)
        {
            Destroy(entryFeeHolder.transform.GetChild(count).gameObject);
        }
    }



    private IEnumerator DealRummyEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    public IEnumerator DealRummyExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    public void DisableDealRummy()
    {
        StartCoroutine(DealRummyExitEffect());
    }

    public void EnableVerificationPanel()
    {
        UIManager.instance.verificationPopUpView.gameObject.SetActive(true);
    }
}