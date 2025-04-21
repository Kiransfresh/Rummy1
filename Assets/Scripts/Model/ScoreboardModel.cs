
using System;
using System.Collections.Generic;

[Serializable]
public class ScoreboardModel
{
    public bool isPointsRummy;
    public List<string> userNameList;
    public List<string> userTotalScoreList;
    public string userTotalScore;
    public List<ScoreRow> scoreRows;


    [Serializable]
    public  class ScoreRow 
    {
        public List<string> scoreList;
    }
}


