using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStats : TableBaseMono
{
    public Button dropBtn;
    public Button sortBtn;
    public Button finishBtn;
    public Button discardBtn;
    public Button groupBtn;
    public Button sharePrivateTableCodeBtn;
    public Toggle autoDrop;

    public bool isGameSarted;
    public bool isAutoDrop;

    private void Start()
    {
        autoDrop.isOn = false;
        isAutoDrop = false;
        autoDrop.onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ServerManager.instance.alertPopUp.ShowView(Constants.MESSAGE.AUTO_DROP_CONFIRMATION, () =>
                {
                    isAutoDrop = true;
                    autoDrop.isOn = true;
                  //  gameTableEventHandler.isDropOnTimeOut = true;
                }, "Yes", () =>
                {
                    isAutoDrop = false;
                    autoDrop.isOn = false;
                   // gameTableEventHandler.isDropOnTimeOut = false;
                }, "No");
            }
            else
            {
                isAutoDrop = false;
                autoDrop.isOn = false;
            }
            
        });

        sharePrivateTableCodeBtn.onClick.AddListener(() =>
        {
            UIManager.instance.sharePrivateTableCode.gameObject.SetActive(true);
        });
    }


    public void SortBtnStats(GameObject SlotsParent)
    {
        if (!UIManager.instance.gameRoom.activeInHierarchy || SlotsParent.transform.childCount > 1) return;
        Card[] cards = SlotsParent.transform.GetChild(0).transform.GetChild(0).transform.GetComponentsInChildren<Card>();

        if (cards != null
            && cards.Length >= 13  
            && SlotsParent.transform.GetChild(0).gameObject.activeInHierarchy && isGameSarted
            && !gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken()).playerEnum.Equals(Constants.PLAYER_ENUM.WRONG_SHOW)
            &&!gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken()).playerEnum.Equals(Constants.PLAYER_ENUM.DROP)
            &&!gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken()).playerEnum.Equals(Constants.PLAYER_ENUM.MIDDLE_DROP)
            )
        {
            sortBtn.gameObject.SetActive(true);
        }
        else
        {
            sortBtn.gameObject.SetActive(false);
        }
    }

    public void FinishAndDiscardBtnStats(List<Card> inHandCardList)
    {
        if (inHandCardList.Count == 14 
            && PlayerPrefs.GetInt(Constants.KEYS.Selection_Counter) == 1 && isGameSarted 
            && gameTableEventHandler.gameTableResponse
                .GetPlayer(PlayerPrefsManager.GetAuthToken()).turn)
        {
            finishBtn.gameObject.SetActive(true);
            discardBtn.gameObject.SetActive(true);
        }
        else
        {
            finishBtn.gameObject.SetActive(false);
            discardBtn.gameObject.SetActive(false);
        }
    }

    public void DropBtnStats(List<Card> inHandCardList)
    {
        if (inHandCardList.Count == 13 
            && gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken()).turn
        && !gameTableEventHandler.gameTableResponse.gameModel.game_sub_type.Equals(Constants.GAME_SUB_TYPE.DEALS) 
            && isGameSarted && !GamePlayManager.instance.isCardPicked)
        {
            dropBtn.gameObject.SetActive(true);
        }
        else
        {
            dropBtn.gameObject.SetActive(false);
        }
    }

    public void AutoDropStats()
    {
        if (autoDrop == null || GamePlayManager.instance.inHandCards == null || UIManager.instance.gameRoom == null || gameTableEventHandler == null || gameTableEventHandler.gameTableResponse == null) {
            return;
        }
        PlayerModel playerModel = gameTableEventHandler.gameTableResponse.GetPlayer(PlayerPrefsManager.GetAuthToken());
        if (autoDrop.gameObject != null 
            && GamePlayManager.instance.inHandCards.activeInHierarchy && isGameSarted
            && UIManager.instance.gameRoom.activeInHierarchy
            && playerModel != null
            && !playerModel.turn
            && !playerModel.playerEnum.Equals(Constants.PLAYER_ENUM.WRONG_SHOW)
            && !gameTableEventHandler.gameTableResponse.gameModel.game_sub_type.Equals(Constants.GAME_SUB_TYPE.DEALS)
            && !playerModel.playerEnum.Equals(Constants.PLAYER_ENUM.DROP) 
            && !playerModel.playerEnum.Equals(Constants.PLAYER_ENUM.MIDDLE_DROP))
        {
            autoDrop.gameObject.SetActive(true);
        }
        else
        {
            if(autoDrop.gameObject != null)
                autoDrop.gameObject.SetActive(false);
        }
    }

    public void GroupBtnStats(bool isEnable)
    {
        if (isGameSarted)
        {
            groupBtn.gameObject.SetActive(isEnable);
        }
    }


    public void DeactivateAllButton(bool isHide)
    {
        dropBtn.gameObject.SetActive(!isHide);
        sortBtn.gameObject.SetActive(!isHide);
        finishBtn.gameObject.SetActive(!isHide);
        discardBtn.gameObject.SetActive(!isHide);
        groupBtn.gameObject.SetActive(!isHide);
        autoDrop.gameObject.SetActive(!isHide);
    }

    public void SharePrivateTableButtonStats(bool value) 
    {
        sharePrivateTableCodeBtn.gameObject.SetActive(value);
    }
}
