using System;
using System.Collections.Generic;

[Serializable]
public class SplitModel
{
    public int time;
    public List<Splits> splits = new List<Splits>();

    [Serializable]
    public class Splits
    {
        public string auth_token;
        public string unique_name;
        public string score;
        public float amount;
        public string drop_possible;
        public bool accepted;
        public bool result_received;
    }
}


