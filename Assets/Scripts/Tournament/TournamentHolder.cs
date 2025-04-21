using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TournamentHolder : MonoBehaviour
{
    public TextMeshProUGUI tournamentTitle;
    public TextMeshProUGUI tournamentJoinedPlayersCount;
    public TextMeshProUGUI numberOfWinnersValue;
    public TextMeshProUGUI entryFeeValue;
    public TextMeshProUGUI prizeValue;
    public Slider playersFilledDetailsIntoTournament;
    public Button registerButton;
    private string tournamentId;

    public void ShowTournamentDetails(TournamentListModel tournament)
    {
        tournamentId = tournament.id;
        tournamentTitle.text = tournament.tournament_title ;
        tournamentJoinedPlayersCount.text = tournament.joined_players_count + "/" + tournament.max_players.ToString();
        entryFeeValue.text = "Rs. " + tournament.entry_value.ToString();
       
        playersFilledDetailsIntoTournament.value = Convert.ToSingle(tournament.joined_players_count) / tournament.max_players;
      //  prizeValue.text = "Rs. " + tournament.   // it is pending from the backend need to implement it

        registerButton.onClick.AddListener(() => 
        {
            StartCoroutine(APIManager.instance.RegisterTournament(tournament.id));
        });

        if (tournament.tournament_status.Equals(Constants.TOURNAMENT_STATUS.REGISTRATION_START))
        {
            registerButton.interactable = true;
        }
        else 
        {
            registerButton.interactable = false;
        }
        
        /* public TextMeshProUGUI numberOfWinnersValue; */
    }

    public void UpdateTournamentJoinedPlayerCount(TournamentListModel tournament, string tournamentId)
    {
        if (this.tournamentId != tournamentId) return;
        tournamentJoinedPlayersCount.text = tournament.joined_players_count + "/" + tournament.max_players.ToString();
        playersFilledDetailsIntoTournament.value = Convert.ToSingle(tournament.joined_players_count) / tournament.max_players;
    }

    public void ChangeRegisterButtonVisibility(TournamentListModel tournament, string tournamentId, string tournamentStatus) 
    {
        if (this.tournamentId != tournamentId) return;
        if (tournament.tournament_status.Equals(Constants.TOURNAMENT_STATUS.REGISTRATION_START))
        {
            registerButton.interactable = true;
        }
        else
        {
            registerButton.interactable = false;
        }
    }


}
