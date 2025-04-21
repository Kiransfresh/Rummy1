using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHistoryPanel : MonoBehaviour
{
    public GameObject gameHistoryRow;
    public Transform SpawnParent;

    public SlidingEffect[] slidingEffect;
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    [SerializeField] private Button backBtn;
    private void Awake()
    {
        startDelay = new WaitForSeconds(1.1f);
        disableDelay = new WaitForSeconds(2.6f);
    }

    private void OnEnable()
    {
        PlayStartEffects();
    }

    private void Start()
    {
        backBtn.onClick.AddListener(() =>
        {
            DeactivatePanel();
            
        });
    }

    private IEnumerator GameHistoryExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    public void DisableGameHistoryPanel()
    {
        StartCoroutine(GameHistoryExitEffect());
        ResetGameInfo();
    }

    private void PlayStartEffects()
    {
        for (int i = 1; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].EntryEffect());
        }
    }

    private void PlayEndEffect()
    {
        for (int i = 1; i < slidingEffect.Length; i++)
        {
            StartCoroutine(slidingEffect[i].ExitEffect());
        }
    }

    public void DeactivatePanel()
    {
        DisableGameHistoryPanel();
    }

    public void SetGameInfo(Response<List<GameHistoryModel>> response)
    {
        foreach (var history in response.data)
        {
            var gameHistory = Instantiate(gameHistoryRow, SpawnParent);
            var gameHistoryInfo = gameHistory.GetComponent<GameHistoryInfo>();
            gameHistoryInfo.tableId.text = history.table_id;
            gameHistoryInfo.amount.text = history.amount + " Rs";
        }
    }

    public void ResetGameInfo()
    {
        for (var i = 0; i < SpawnParent.childCount; i++)
        {
            Destroy(SpawnParent.GetChild(i).gameObject);
        }
    }

}
