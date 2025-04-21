using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EntryFee : BaseMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI EntryAmount;
    [SerializeField] private TextMeshProUGUI Player;
    [SerializeField] private Button PlayBtn;

    public List<Transform> imageTransforms;

    private GameListModel selectedGameModel = null;

    public void SetGameData(GameListModel model)
    {
        if (model.game_sub_type == Constants.GAME_SUB_TYPE.POINT)
        {
            EntryAmount.text = model.point_value + "\nPoint";// float.Parse(model.entry_fee).ToString("0.00") + "\nPoint";
        }
        else if (model.game_sub_type == Constants.GAME_SUB_TYPE.POOL)
        {
            EntryAmount.text = float.Parse(model.entry_fee).ToString("0.00") + "\nPool";
        }
        else if (model.game_sub_type == Constants.GAME_SUB_TYPE.DEALS)
        {
            EntryAmount.text = float.Parse(model.entry_fee).ToString("0.00") + "\nDeal";
        }

        Player.text = "Open Table: " + model.lobby_seating;

        PlayBtn.onClick.AddListener(() => {
            selectedGameModel = model;
            UIManager.instance.lobbyView.RestrictBlockPlayer(PlayGame);
        });
    }



    private void PlayGame()
    {
        PlayBtn.interactable = false;
        if (!UIManager.instance.lobbyView.isPrivateTable)
        {
            JoinTable(selectedGameModel, null, null);
            GamePlayManager.instance.gameTableEventHandler.ResetTable();
            GamePlayManager.instance.gameTableEventHandler.ResetAllBeforeFocus();
            GamePlayManager.instance.gameTableEventHandler.gameResult.gameObject.SetActive(false);
            //  GamePlayManager.instance.gameTableEventHandler.meldCards.gameObject.SetActive(false);
            UIManager.instance.lobbyView.gameObject.SetActive(true);
        }
        else
        {
            ServerManager.instance.alertPopUp.ShowView("Are you sure, do you want to Create Private Table?", () =>
            {
                CreatePrivateTableResponseHandler(selectedGameModel.id);
            }, "Yes", () => { }, "No");
        }
        StartCoroutine(EnabledrDisabledPlayButton());
    }

    private IEnumerator EnabledrDisabledPlayButton()
    {
        yield return new WaitForSeconds(0.2f);
        PlayBtn.interactable = true;
    }
}
