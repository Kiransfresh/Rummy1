using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Utils 
{
    public static class JsonHelper
    {
        public static string ToJson<T>(List<T> array)
        {
            var jsonString = "";

            for (var i = 0; i < array.Count; i++)
            {
                if (i < array.Count - 1)
                {
                    jsonString = jsonString + JsonUtility.ToJson(array[i]) + ",";
                }
                else
                {
                    jsonString += JsonUtility.ToJson(array[i]);
                }
            }

            jsonString = "[" + jsonString + "]";
            return jsonString;
        }
    }

}

