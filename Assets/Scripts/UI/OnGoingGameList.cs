using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnGoingGameList : BaseMonoBehaviour
{
    
    public SlidingEffect[] slidingEffect;
    [SerializeField] private Button closeBtn;
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    [SerializeField] private GameObject slotParent;
    [SerializeField] private GameObject gameListSlot;
    

    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        PlayStartEffects();
        Debug.Log("On going game list on Enable is Trigger");
    }

    private void Start()
    {
        closeBtn.onClick.AddListener(DisableActiveGamesPanel);
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


    private IEnumerator OnGoingGameListEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
    }

    private IEnumerator OnGoingGameListExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    public void DisableActiveGamesPanel()
    {
        StartCoroutine(OnGoingGameListExitEffect());
    }

    public void ShowOnGoingGames(Response<List<OnGoingGameListModel>> response)
    {
        gameObject.SetActive(true);
        ClearGameList();
        foreach (var onGoingGame in response.data)
        {
            var game = Instantiate(gameListSlot, slotParent.transform);
            var tableId = game.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var gameType = game.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
            var joinBtn = game.transform.GetChild(2).GetComponent<Button>();

            tableId.text = "Table ID - #" + onGoingGame.id;
            gameType.text = onGoingGame.title;
            game.SetActive(true);
            joinBtn.onClick.AddListener(() =>
            {
                StartCoroutine(JoinOnGoingGameList(onGoingGame));
            });
        }
    }

    private IEnumerator JoinOnGoingGameList(OnGoingGameListModel onGoingGame)
    {
        DisableActiveGamesPanel();
        yield return new WaitForSeconds(0.2f);
        JoinTable(null, onGoingGame.id, null);
        GamePlayManager.instance.gameTableEventHandler.buttonStats.isGameSarted = true;
    }

    private void ClearGameList()
    {
        if(slotParent.transform.childCount <= 0) return;
        for (var i = 0; i < slotParent.transform.childCount; i++)
        {
            Destroy(slotParent.transform.GetChild(i).gameObject);
        }
    }
}
