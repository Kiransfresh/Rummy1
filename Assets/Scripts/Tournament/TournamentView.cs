using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentView : MonoBehaviour
{
    #region PUBLIC_VARS
    public SlidingEffect[] slidingEffect;
    public GameObject tournamentHolder;
    public Transform tournamentHolderParent;
    public Button backButton;
    #endregion

    #region PRIVATE_VARS
    private WaitForSeconds startDelay;
    private WaitForSeconds disableDelay;
    private List<TournamentHolder> tournamentHolders = new List<TournamentHolder>();
    #endregion

    #region UNITY_CALLBACKS
    private void Start()
    {
        backButton.onClick.AddListener(() =>
        {
            DisableTournamentPanel();
        });
    }

    private void Awake()
    {
        startDelay = new WaitForSeconds(0.15f);
        disableDelay = new WaitForSeconds(0.6f);
    }

    private void OnEnable()
    {
        StartCoroutine(TournamentPanelEntryEffect());
    }
    #endregion

    #region PRIVATE_FUNCTION

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

    #endregion

    #region CO-ROUTINES
    private IEnumerator TournamentPanelEntryEffect()
    {
        yield return startDelay;
        PlayStartEffects();
        ShowTournaments();
    }

    private IEnumerator TournamentPanelExitEffect()
    {
        PlayEndEffect();
        yield return disableDelay;
        gameObject.SetActive(false);
        ClearTournament();
    }

    private void ShowTournaments()
    {
        foreach (var tournament in CacheMemory.TournamentList)
        {
            var tournamentObj = Instantiate(tournamentHolder, tournamentHolderParent);
            tournamentObj.GetComponent<TournamentHolder>().ShowTournamentDetails(tournament);
            tournamentHolders.Add(tournamentObj.GetComponent<TournamentHolder>());
        }
    }

    private void ClearTournament()
    {
        for (var i = 0; i < tournamentHolderParent.childCount; i++)
        {
            Destroy(tournamentHolderParent.GetChild(i).gameObject);
        }
    }

    #endregion

    #region UI_CALLBACKS

    public void DisableTournamentPanel()
    {
        StartCoroutine(TournamentPanelExitEffect());
    }

    public IEnumerator RefreshTournaments()
    {
        if (!gameObject.activeInHierarchy) yield break;
        ServerManager.instance.loader.ShowLoader("Please wait tournaments are Updating...");
        ClearTournament();
        yield return new WaitForSeconds(0.2f);
        ShowTournaments();
        ServerManager.instance.loader.HideLoader();
    }

    public void UpdateTournamentJoinedPlayersCount()
    {
        if (tournamentHolders == null || tournamentHolders.Count <= 1) return;
        foreach (var tournament in CacheMemory.TournamentList)
        {
            foreach (var tournamentHolder in tournamentHolders)
            {
                tournamentHolder.UpdateTournamentJoinedPlayerCount(tournament, tournament.id);
            }
        }
    }

    public void ChangeRegisterButtonVisibility(List<TournamentListModel> tournmentListModel)
    {
        if (tournamentHolders == null || tournamentHolders.Count <= 0) return;
        foreach (var tournament in tournmentListModel)
        {
            foreach (var tournamentHolder in tournamentHolders)
            {
                tournamentHolder.ChangeRegisterButtonVisibility (tournament, tournament.id, tournament.tournament_status);
            }
        }
    }
    #endregion
}
