using TMPro;
using UnityEngine;

public class GameScoreboard : TableBaseMono
{
    [SerializeField] private GameObject HeaderRow;
    [SerializeField] private GameObject FooterRow;
    [SerializeField] private GameObject DataRow;
    [SerializeField] private GameObject DataRowParent;


    public void SetScoreboard(ScoreboardModel scoreboardModel)
    {
        ResetScoreRows();
        if (gameTableEventHandler.gameTableResponse.gameModel.game_sub_type.Equals(Constants.GAME_SUB_TYPE.POOL)
            || gameTableEventHandler.gameTableResponse.gameModel.game_sub_type.Equals(Constants.GAME_SUB_TYPE.DEALS))
        {
            for (var i = 1; i < HeaderRow.transform.childCount; i++)
            {
                HeaderRow.transform.GetChild(i).gameObject.SetActive(false);
            }

            for (var j = 0; j < scoreboardModel.userNameList.Count; j++)
            {
                 var usernameText =  HeaderRow.transform.GetChild(j + 1).GetChild(0).GetComponent<TextMeshProUGUI>();
                 usernameText.text = scoreboardModel.userNameList[j];
                 usernameText.transform.parent.gameObject.SetActive(true);
            }

            for (var k = 0; k < scoreboardModel.scoreRows.Count; k++)
            {
                var playersScoreCard = Instantiate(DataRow, DataRowParent.transform);
                playersScoreCard.SetActive(true);
                var scorecard = playersScoreCard.transform.GetChild(0);
                scorecard.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (k + 1).ToString();
                for (var l = 0; l < scorecard.childCount; l++)
                {
                    scorecard.GetChild(l).gameObject.SetActive(false);
                }
                
                for (var l = 0; l < scoreboardModel.scoreRows[k].scoreList.Count; l++)
                {
                    scorecard.GetChild(l + 1).GetChild(0).GetComponent<TextMeshProUGUI>().text
                        = scoreboardModel.scoreRows[k].scoreList[l];
                    scorecard.GetChild(l + 1).gameObject.SetActive(true);
                    scorecard.GetChild(0).gameObject.SetActive(true);
                }
            }

            for (var i = 1; i < FooterRow.transform.childCount; i++)
            {
                FooterRow.transform.GetChild(i).gameObject.SetActive(false);
                FooterRow.gameObject.SetActive(true);
            }

            for (var i = 0; i < scoreboardModel.userTotalScoreList.Count; i++)
            {
                FooterRow.transform.GetChild(i + 1).GetChild(0).GetComponent<TextMeshProUGUI>().text
                    = scoreboardModel.userTotalScoreList[i].ToString();
                FooterRow.transform.GetChild(i + 1).gameObject.SetActive(true);
            }

        }
        else
        {
            for (var i = 1; i < HeaderRow.transform.childCount; i++)
            {
                HeaderRow.transform.GetChild(i).gameObject.SetActive(false);
            }

            for (var j = 0; j < 4; j++)
            {
                var gameIdText = HeaderRow.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
                var resultText = HeaderRow.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
                var countText = HeaderRow.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
                var totalCountText = HeaderRow.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
                gameIdText.text = "GameId";
                resultText.text = "Result";
                countText.text = "Count";
                totalCountText.text = "Total Chips";
                HeaderRow.transform.GetChild(j + 1).gameObject.SetActive(true);
            }

            for (var k = 0; k < scoreboardModel.scoreRows.Count; k++)
            {
                var playersScoreCard = Instantiate(DataRow, DataRowParent.transform);
                playersScoreCard.SetActive(true);
                var scorecard = playersScoreCard.transform.GetChild(0);
                scorecard.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (k + 1).ToString();
                for (var l = 0; l < scorecard.childCount; l++)
                {
                    scorecard.GetChild(l).gameObject.SetActive(false);
                }

                for (var l = 0; l < scoreboardModel.scoreRows[k].scoreList.Count; l++)
                {
                    scorecard.GetChild(l + 2).GetChild(0).GetComponent<TextMeshProUGUI>().text
                        = scoreboardModel.scoreRows[k].scoreList[l];
                    var tableId = scorecard.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
                       tableId.text = gameTableEventHandler.gameTableResponse.id;

                    if (tableId.text.Length > 10)
                    {
                        tableId.text = "#" + tableId.text.Substring(0, 2) + ".."
                                       + tableId.text.Substring(tableId.text.Length - 6);
                    }
                    else
                    {
                        tableId.text = "#" + tableId.text;
                    }

                    scorecard.GetChild(l + 2).gameObject.SetActive(true);
                    scorecard.GetChild(1).gameObject.SetActive(true);
                    scorecard.GetChild(0).gameObject.SetActive(true);
                }
            }
            FooterRow.gameObject.SetActive(false);
        }
    }

    private void ResetScoreRows()
    {
        for (var i = 0; i < DataRowParent.transform.childCount; i++)
        {
            Destroy(DataRowParent.transform.GetChild(i).gameObject);
        }
    }
}
