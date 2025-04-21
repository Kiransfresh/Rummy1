using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointRummyView : BaseMonoBehaviour
{
    public Button backBtn;

    [Header("Animations")]
    public SlidingEffect[] slidingEffect;
    public FadingColorEffect[] fadingColorEffect;

    [Header("Entry Fees holder")]
    public GameObject entryFeePrefab;
    public GameObject entryFeeHolder;

    [Header("Number Of Players Buttons")]
    [SerializeField] private Button twoPlayerBtn;
    [SerializeField] private Button fourPlayerBtn;
    [SerializeField] private Button sixPlayerBtn;

    [Header("Message Text")]
    [SerializeField] private TextMeshProUGUI messageText;


    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    private int numberOfPlayer;

    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        PlayerSelectionToggle(Constants.SEAT.PLAYER_2);
        StartCoroutine(PointRummyEntryEffect());
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() =>
        {
           // UIManager.instance.ClearEntryFeeList(pointGameEntryFee);
            DisablePointRummy();
        });
    }




    private void PlayStartEffects()
    {
        for (int i = 0; i < fadingColorEffect.Length; i++)
        {
            StartCoroutine(fadingColorEffect[i].ChangeColor());
        }

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

        for (int i = 0; i < fadingColorEffect.Length; i++)
        {
            StartCoroutine(fadingColorEffect[i].SetInitialColor());
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
        FetchPointGameList(players);
        Debug.Log("Fecthed Players : " + players);
    }

    List<GameListModel> gameListModel = new List<GameListModel>();

    private void FetchPointGameList(int numberOfPlayers)
    {
        ClearEntryFeeHolder();
        if (CacheMemory.GameType.Equals(Constants.GAME_TYPE.PRACTICE)
            || CacheMemory.GameType.Equals(Constants.GAME_TYPE.CASH))
        {
            var gamelist = GameListFilter.GetGames(CacheMemory.GameType, Constants.GAME_SUB_TYPE.POINT, null);
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

            if (gameListModel.Count <= 0) {
                messageText.gameObject.SetActive(true);
            } else {
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

    private void DisablePointRummy()
    {
        StartCoroutine(PointRummyExitEffect());
    }

    private IEnumerator PointRummyEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    public IEnumerator PointRummyExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    public void EnableVerificationPanel()
    {
        UIManager.instance.verificationPopUpView.gameObject.SetActive(true);
    }
}
