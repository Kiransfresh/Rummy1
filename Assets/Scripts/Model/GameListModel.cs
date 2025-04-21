using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class GameListModel
{
    public string id;
    public string deals;
    public string entry_fee;
    public string game_type;
    public string game_sub_type;
    public float point_value;
    public float pool_deal_prize;
    public string pool_game_type;
    public int number_of_players_per_table;
    public int drop;
    public int middle_drop;
    public int full_count;
    public int prize_after_deduction;
    public string lobby_seating;
    public string coins_value_for_private_table;
    public int no_of_rounds;

    public decimal getEntryFeesInNumericFormat()
    {
        return decimal.Round(decimal.Parse(entry_fee), 2);
    }
}

[Serializable]
public class GameListFilter
{
    public static List<GameListModel> GetGames(string GameType, string GameSubType, string PoolType)
    {
        var GameList = CacheMemory.GameList;
        var list = new List<GameListModel>();

        foreach (var gamemodel in GameList)
        {
            if (gamemodel.game_type.Equals(GameType) && gamemodel.game_sub_type.Equals(GameSubType))
            {
                if (GameSubType.Equals(Constants.GAME_SUB_TYPE.POOL))
                {
                    if (gamemodel.pool_game_type.Equals(PoolType))
                    {
                        list.Add(gamemodel);
                    }
                }
                else
                {
                    list.Add(gamemodel);
                }
            }
        }

        if (GameSubType.Equals(Constants.GAME_SUB_TYPE.POINT))
        {
            list.Sort((x, y) => x.point_value.CompareTo(y.point_value));
        }
        else
        {
            list.Sort((x, y) => x.getEntryFeesInNumericFormat().CompareTo(y.getEntryFeesInNumericFormat()));
        }

        return list;
    }

    public static String GetGameText(string GameType, string GameSubType, string PoolType) {
        var gamelist = GetGames(GameType, GameSubType, PoolType: PoolType);
        if (GameSubType.Equals(Constants.GAME_SUB_TYPE.POOL) || GameSubType.Equals(Constants.GAME_SUB_TYPE.DEALS))
        {
            return "Value " + gamelist[0].entry_fee + " To " + gamelist[gamelist.Count - 1].entry_fee;
        }
        else {
            return "Value " + gamelist[0].point_value + " To " + gamelist[gamelist.Count - 1].point_value;
        }
    }
}