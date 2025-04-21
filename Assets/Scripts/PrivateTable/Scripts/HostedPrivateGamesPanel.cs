using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostedPrivateGamesPanel : MonoBehaviour
{
    public GameObject hostedGameDetails;
    public Transform SpawnParent;

    public SlidingEffect[] slidingEffect;
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;

    [SerializeField] private Button backBtn;
    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
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

    private IEnumerator HostedGameExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
    }

    public void DisableHostedGamesPanel()
    {
        StartCoroutine(HostedGameExitEffect());
        ResetGameInfo();
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

    public void DeactivatePanel()
    {
        DisableHostedGamesPanel();
    }

    public void SetGameInfo(Response<List<HostedGameDetailsModel>> response)
    {
        foreach (var hostedGame in response.data)
        {
            var hostedGameDetail = Instantiate(hostedGameDetails, SpawnParent);
            var hostGamesInfo = hostedGameDetail.GetComponent<HostedPrivateGame>();
            hostGamesInfo.TableID.text = hostedGame.table_id;
            hostGamesInfo.BetAmount.text = hostedGame.bet_amount;
            hostGamesInfo.GameType.text = hostedGame.game_sub_type;
            hostGamesInfo.CreatedDateTime.text = hostedGame.created_date;
            hostGamesInfo.InitializeGameRecord();
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
