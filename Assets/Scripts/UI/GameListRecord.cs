using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using MagneticScrollView;

public class GameListRecord : BaseMonoBehaviour
{
    public TextMeshProUGUI EntryFee, PlayingNumber, Players, StatusOfGame;
    public Image GameTypeIcon;
    [HideInInspector]
    public int MaxPlayers;
    public Button JoinTableBtn;
    public Action JoinCallBack;
    GameListModel gamemodel;

    private void Start()
    {
        JoinTableBtn.onClick.AddListener(() =>
        {
            UIManager.instance.lobbyView.RestrictBlockPlayer(OnJoinBtnClick);
        });
    }

    public void SetGameRecordData(GameListModel game) {
        gamemodel = game;
        Decimal fee = Decimal.Round(Decimal.Parse(game.entry_fee),2);
        
        EntryFee.text = fee.ToString();
        Players.text = game.lobby_seating;
        StatusOfGame.text = "Play Now";

        MaxPlayers = game.number_of_players_per_table;
    }

    void OnJoinBtnClick()
    {
        JoinTable(gamemodel, null, null);
    }

    public void SetRegisteredPlayers(string registeredPlayers)
    {
        Players.text = registeredPlayers;
    }
}
