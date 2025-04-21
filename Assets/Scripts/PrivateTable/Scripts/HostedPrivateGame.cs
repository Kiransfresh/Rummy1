using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class HostedPrivateGame : MonoBehaviour
{
    public TextMeshProUGUI TableID, BetAmount, GameType, CreatedDateTime;
    public Image GameTypeIcon;
    [SerializeField]
    private Sprite PointsGameIcon, PoolGameIcon, DealsGameIcon;
    public void InitializeGameRecord() 
    {
        if (GameType.text.Equals(Constants.GAME_SUB_TYPE.POINT))
        {

            GameTypeIcon.sprite = PointsGameIcon;
        }
        else if (GameType.text.Equals(Constants.GAME_SUB_TYPE.DEALS))
        {
            GameTypeIcon.sprite = DealsGameIcon;
        }
        else
        {
            GameTypeIcon.sprite = PoolGameIcon;
        }
    }


}
