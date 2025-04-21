using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CacheMemory
{
    public static PlayerModel playerModel;
    public static UserModel userModel = null;
    public static string GameType = Constants.GAME_TYPE.PRACTICE;
    public static List<GameListModel> GameList;
    public static string RunningTableId = null;
    public static string IsGuestUser = null;
    public static string NumberOfPlayers = null;
    public static string ReferAFriend = null;
    public static string MobileNumber = null;
    public static string PlayerStatus = null;

    public static string privateTableCode = null;
    public static string privateTableHostAuthToken = null;
    public static string coinprice = null;

    public static List<TournamentListModel> TournamentList;

}
