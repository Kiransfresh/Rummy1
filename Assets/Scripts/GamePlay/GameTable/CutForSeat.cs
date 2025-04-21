using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutForSeat : TableBaseMono
{
    public List<CutForSeatCards> cutForSeatObjs;
    public PlayerHolder playerHolder;


    public void Start()
    {
        ResetCards();
    }

    public void StartCutForSeat()
    {
        ResetCards();
        for (var i = 0; i < playerHolder.players.Count; i++)
        {
            if (playerHolder.players[i].gameObject.activeInHierarchy)
            {
                var authToken = playerHolder.players[i].authToken;
                var playerModel = gameTableEventHandler.gameTableResponse.GetPlayer(authToken);
                cutForSeatObjs[i].gameObject.SetActive(true);
                cutForSeatObjs[i].cardGlow.gameObject.SetActive(playerModel.cutForSeatWinner);
                var name = playerModel.cutForSeatCard.suit + "_" + playerModel.cutForSeatCard.rank;
                cutForSeatObjs[i].card.sprite = GamePlayManager.instance.GetSprite(name);
                cutForSeatObjs[i].CutforSeatAnim();

                
            }
        }

        var cutForSeatWinner = gameTableEventHandler.gameTableResponse.GetCutForSeatWinner();

        if (cutForSeatWinner.userModel.auth_token.Equals(PlayerPrefsManager.GetAuthToken()))
        {
            gameTableEventHandler.messageInfo.ShowWithMessage("You won the toss");
        }
        else
        {
            var message = cutForSeatWinner.userModel.unique_name + " won the toss";
            gameTableEventHandler.messageInfo.ShowWithMessage(message);
        }
        for (var i = 0; i < playerHolder.players.Count; i++)
        {
            if (playerHolder.players[i].gameObject.activeInHierarchy)
            {
                playerHolder.players[i].SetPlayerAvatar();
            }
        }
    }

    public void ResetCards()
    {
        foreach (var Cards in cutForSeatObjs)
        {
           Cards.gameObject.SetActive(false);
        }
    }


}
