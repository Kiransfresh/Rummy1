using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ServerManager : MonoBehaviour
{
    [Header("Dialogs")]
    public AlerPopUp alertPopUp;
    public CommonWebView commonWebView;
    public WinnerPopup winAlertPopUp;

    public TextMeshProUGUI livePlayers;

    public static ServerManager instance;

    private bool useEncryption = false;

    public SmartFox sfs;

   

    public Loader loader;
    public bool isTurboPlay;

    public Action<Response<SplitModel>> splitCallback;
    public Action<Response<RejoinModel>> rejoinCallback;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InvokeRepeating("ClientPingingRequest", 10f, 1f);
    }

    private int nextInternetCheck = 2;
    private void Update()
    {
        if (sfs != null)
        {
            sfs.ProcessEvents();
        }

        if (Time.time >= nextInternetCheck)
        {
            nextInternetCheck = Mathf.FloorToInt(Time.time) + 1;
            CheckInternetReachability();
        }

    }

    public void Initialise()
    {
        if (sfs != null)
        {
            sfs.Disconnect();
            sfs.RemoveAllEventListeners();
            sfs = null;
        }

        if (sfs == null)
        {
#if UNITY_WEBGL
        sfs = new SmartFox(useEncryption ? UseWebSocket.WSS_BIN : UseWebSocket.WS_BIN);
#else
            sfs = new SmartFox();
#endif
        }
        sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.AddEventListener(SFSEvent.CONNECTION_RETRY, OnConnectionRetry);
        sfs.AddEventListener(SFSEvent.CONNECTION_RESUME, OnConnectionResume);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.SOCKET_ERROR, OnSocketError);
        sfs.AddEventListener(SFSEvent.LOGIN, OnSFSLogin);
        sfs.AddEventListener(SFSEvent.LOGOUT, OnSFSLogout);
        sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnSFSLoginError);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponseHandler);
        sfs.AddEventListener(SFSEvent.CRYPTO_INIT, OnCryptoInit);

        var cfg = new ConfigData();
        cfg.Host = Constants.SERVER_DETAILS.Host;
#if !UNITY_WEBGL
        cfg.Port = Constants.SERVER_DETAILS.TcpPort;
#else
if (!useEncryption)
        cfg.Port = Constants.SERVER_DETAILS.WsPort;
else
        cfg.Port = Constants.SERVER_DETAILS.WssPort;
#endif
        cfg.Zone = Constants.SERVER_DETAILS.Zone;
        cfg.Debug = true;
        sfs.Connect(cfg);
    }

    private void OnConnection(BaseEvent evt)
    {
        Debug.Log("On Connection");
        if ((bool)evt.Params["success"])
        {
#if !UNITY_WEBGL
            if (useEncryption)
            {
                sfs.InitCrypto();
            }
            else
            {
                LoginRequest(null);
            }
#else
        
       LoginRequest(null);
#endif
        }
        else
        {
            Debug.Log("Fail");
            Initialise();
        }
    }


    private void OnConnectionRetry(BaseEvent evt)
    {
        Debug.Log("SFS connection retry");
        Initialise();
    }

    private void OnConnectionResume(BaseEvent evt)
    {
        Debug.Log("SFS connection resume");
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        if (evt.Params.ContainsKey("reason"))
        {
            var reason = evt.Params["reason"].ToString();
            if (reason != null && reason.ToLower().Equals("kick"))
            {
                instance.commonWebView.CloseWebView();
                PlayerPrefs.DeleteKey(Constants.KEYS.auth_token);
                PlayerPrefs.DeleteKey(Constants.KEYS.bank_name);
                PlayerPrefs.DeleteKey(Constants.KEYS.account_number);

                if (UIManager.instance.loginView.gameObject.activeInHierarchy == true) {
                    #if !UNITY_EDITOR
                               UIManager.instance.loginView.googleLogin.OnSignOut();
                    #endif
                }

                
                if (UIManager.instance.lobbyView.gameObject.activeInHierarchy)
                {
                    UIManager.instance.lobbyView.gameObject.SetActive(false);
                }

                if (UIManager.instance.gameRoom.activeInHierarchy)
                {
                    #if UNITY_WEBGL
                        Application.ExternalEval("Quit()");
                    #else
                        Debug.Log("KICK - UNLOAD GAME");
                        UIManager.instance.gameRoom.SetActive(false);
                    #endif
                }

                #if !UNITY_WEBGL
                    Debug.Log("KICK - LOAD LOGIN");
                    UIManager.instance.loginView.gameObject.SetActive(true);
                #endif

                Debug.Log("KICK DONE");
            }
            else
            {
                Initialise();
            }
        }
        else
        {
            Debug.Log("Connection on connection lost2");
            Initialise();
        }
    }

    private void OnSocketError(BaseEvent evt)
    {

    }

    private void OnSFSLogin(BaseEvent evt)
    {
        var sFSObject = (ISFSObject)evt.Params["data"];
        if (sFSObject != null)
        {

#if UNITY_WEBGL
            string url = "https://moonrummy.com/playtool/p/index.html?ecffgiHUgcH7jOy3C3DV4oWp02DDMoYr&116";
            if (url != "")
            //if (Application.absoluteURL != "")
            {
                string[] UrlParams = null;
                //int flag = Application.absoluteURL.IndexOf("?", StringComparison.Ordinal);
                int flag = url.IndexOf("?", StringComparison.Ordinal);
                if (flag != -1)
                {
                    //UrlParams = Application.absoluteURL.Split('?')[1].Split('&');
                    UrlParams = url.Split('?')[1].Split('&');
                }
                string authToken = UrlParams[0];
                string gameID = UrlParams[1];
                string tableID = null;
                if (UrlParams.Length > 2)
                {
                    tableID = UrlParams[2];
                    CacheMemory.RunningTableId = tableID;
                }
                UIManager.instance.JoinTable(null, table_id: tableID, gameID);
            }  
#else
            var json = sFSObject.GetText(Constants.KEYS.packet);
            Debug.Log(json);
            var model = (Response<UserModel>)JsonUtility.FromJson(json, typeof(Response<UserModel>));
            if (model.status == Constants.KEYS.valid)
            {
                OnSFSLoginCallback?.Invoke(true);
                UIManager.instance.lobbyView.gameObject.SetActive(true);
                UIManager.instance.FetchTournamentList();
                StartCoroutine(APIManager.instance.GetProfileDetails());
                // StartCoroutine(APIManager.instance.FetchBankDetails());
                UIManager.instance.FetchGameList();
                UIManager.instance.GetCoinValue();
            }
            else
            {
                OnSFSLoginCallback?.Invoke(false);
                UIManager.instance.loginView.gameObject.SetActive(true);
            }
#endif
        }
        else
        {
            OnSFSLoginCallback?.Invoke(false);
            UIManager.instance.loginView.gameObject.SetActive(true);
        }
    }

    private void OnSFSLogout(BaseEvent evt)
    {
        Debug.Log("Logout executed!");
        UIManager.instance.LogoutView();
        if (PlayerPrefs.HasKey("LastFortuneWheelSpin"))
            PlayerPrefs.DeleteKey("LastFortuneWheelSpin");
    }

    private void OnSFSLoginError(BaseEvent evt)
    {

    }

    private void OnExtensionResponseHandler(BaseEvent evt)
    {
        var cmd = (string)evt.Params[Constants.KEYS.cmd];
        // Debug.Log(cmd);
        if (cmd.Equals(Constants.REQUEST.GAMES))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<List<GameListModel>>)JsonUtility.FromJson(json, typeof(Response<List<GameListModel>>));
            GameListCallback?.Invoke(model);
            UIManager.instance.RefreshAllGameList();
        }
        else if (cmd.Equals(Constants.REQUEST.JOIN_TABLE))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<string>)JsonUtility.FromJson(json, typeof(Response<string>));
            JoinTableCallback?.Invoke(model);
        }
        else if (cmd.Equals(Constants.REQUEST.CREATE_PRIVATE_TABLE))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<PrivateTableModel>)JsonUtility.FromJson(json, typeof(Response<PrivateTableModel>));
            CreatePrivateTableCallback?.Invoke(model);
        }
        else if (cmd.Equals(Constants.REQUEST.JOIN_PRIVATE_TABLE))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<string>)JsonUtility.FromJson(json, typeof(Response<string>));
            JoinPrivateTableCallback?.Invoke(model);
        }
        else if (cmd.Equals(Constants.REQUEST.GAME_PLAY_EVENT))
        {
            if (!UIManager.instance.gameRoom.activeInHierarchy) return;
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<GameTableResponse>)JsonUtility.FromJson(json, typeof(Response<GameTableResponse>));
            GameTableCallback?.Invoke(model);
        }
        else if (cmd.Equals(Constants.REQUEST.USER_INFORMATION))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<UserModel>)JsonUtility.FromJson(json, typeof(Response<UserModel>));
            UserProfileCallback?.Invoke(model);
        }
        else if (cmd.Equals(Constants.REQUEST.UPDATE_CHIPS))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<object>)JsonUtility.FromJson(json, typeof(Response<object>));
            RefreshChipsCallback?.Invoke(model);
        }
        else if (cmd.Equals(Constants.REQUEST.UPDATE_COINS))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<object>)JsonUtility.FromJson(json, typeof(Response<object>));
            RefreshCoinsCallback?.Invoke(model);
        }
        else if (cmd.Equals(Constants.REQUEST.SPLIT))
        {
            if (!GamePlayManager.instance.gameTableEventHandler.gameResult.gameObject.activeInHierarchy) return;
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            Debug.Log("SPLIT - " + json);
            var model = (Response<SplitModel>)JsonUtility.FromJson(json, typeof(Response<SplitModel>));
            splitCallback?.Invoke(model);
        }
        else if (cmd.Equals(Constants.REQUEST.REJOIN_GAME))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<RejoinModel>)JsonUtility.FromJson(json, typeof(Response<RejoinModel>));
            rejoinCallback?.Invoke(model);
        }
        else if (cmd.Equals(Constants.REQUEST.ON_GOING_GAME_LIST))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<List<OnGoingGameListModel>>)JsonUtility.FromJson(json, typeof(Response<List<OnGoingGameListModel>>));
            onGoingGameListCallback?.Invoke(model);
        }
        else if (cmd.Equals(Constants.REQUEST.LAST_ROUND_RESULT))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<GameTableResponse.LiveGameModel>)JsonUtility.FromJson(json, typeof(Response<GameTableResponse.LiveGameModel>));
            lastRoundResultCallback?.Invoke(model);
        }
        else if (cmd.Equals(Constants.REQUEST.SCORE_BOARD))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<ScoreboardModel>)JsonUtility.FromJson(json, typeof(Response<ScoreboardModel>));
            scoreboardCallback?.Invoke(model);
        }
        else if (cmd.Equals(Constants.REQUEST.LIVE_PLAYERS))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<int>)JsonUtility.FromJson(json, typeof(Response<int>));
            livePlayers.text = "Online players- " + model.data;
        }
        else if (cmd.Equals(Constants.REQUEST.REGISTERED_PLAYERS))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<List<string>>)JsonUtility.FromJson(json, typeof(Response<List<string>>));
        }
        else if (cmd.Equals(Constants.REQUEST.TOURNAMENT_LIST))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<List<TournamentListModel>>)JsonUtility.FromJson(json, typeof(Response<List<TournamentListModel>>));
            TournamentListCallback?.Invoke(model);
            StartCoroutine(UIManager.instance.tournamentView.RefreshTournaments());
        }
        else if (cmd.Equals(Constants.REQUEST.TOURNAMENT_JOINED_PLAYERS_COUNT))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<List<TournamentListModel>>)JsonUtility.FromJson(json, typeof(Response<List<TournamentListModel>>));
            TournamentListCallback?.Invoke(model);
            UIManager.instance.tournamentView.UpdateTournamentJoinedPlayersCount();
        }
        else if (cmd.Equals(Constants.REQUEST.TOURNAMENT_DATA))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            var model = (Response<List<TournamentListModel>>)JsonUtility.FromJson(json, typeof(Response<List<TournamentListModel>>));
            UIManager.instance.tournamentView.ChangeRegisterButtonVisibility(model.data);
        }
        else if (cmd.Equals(Constants.REQUEST.CONFIG))
        {
            var ISFSObj = (ISFSObject)evt.Params[Constants.KEYS.parameter];
            var json = ISFSObj.GetText(Constants.KEYS.packet);
            Debug.Log(json);
            var configModel = (Response<string>)JsonUtility.FromJson(json, typeof(Response<string>));
            if (configModel.data == "YES")
            {
                UIManager.instance.lobbyView.Toggle_Changed(true);
            }
            else {
                UIManager.instance.lobbyView.Toggle_Changed(false);
            }
            
        }
    }

    private void OnCryptoInit(BaseEvent evt)
    {
        if ((bool)evt.Params["success"])
        {
            Debug.Log("success oncryptoinit");
            LoginRequest(null);
            loader.HideLoader();
        }
        else
        {

            //LoginRequest(null);
            Debug.Log("fail oncryptoinit" + string.Join(Environment.NewLine,evt.Params.Select(a => $"{a.Key} :{a.Value} ")));
        }
    }


    public Action<bool> OnSFSLoginCallback;
    public void LoginRequest(Action<bool> callback)
    {
        Debug.Log("Login with sfs");
        if (sfs != null && sfs.IsConnected)
        {
            if (callback != null)
            {
                OnSFSLoginCallback = callback;
            }
#if UNITY_WEBGL
            string url = "https://moonrummy.com/playtool/p/index.html?ecffgiHUgcH7jOy3C3DV4oWp02DDMoYr&116";
            if (url != "")
            //if (Application.absoluteURL != "")
            {
                string[] UrlParams = null;
                //int flag = Application.absoluteURL.IndexOf("?", StringComparison.Ordinal);
                int flag = url.IndexOf("?", StringComparison.Ordinal);
                if (flag != -1)
                {
                    //UrlParams = Application.absoluteURL.Split('?')[1].Split('&');
                    UrlParams = url.Split('?')[1].Split('&');
                }
                string authToken = UrlParams[0];
                PlayerPrefsManager.SetAuthToken(authToken);
            }
            
#endif

            if (PlayerPrefsManager.GetAuthToken() != null)
            {
                Debug.Log($"{PlayerPrefsManager.GetAuthToken()}   LOGIN");
                var Request = new LoginRequest(PlayerPrefsManager.GetAuthToken(), "123456", Constants.SERVER_DETAILS.Zone);
                sfs.Send(Request);
            }
        }
        else
        {
            callback?.Invoke(false);
        }
    }


    private Action<Response<List<GameListModel>>> GameListCallback;
    public void GameListRequest(Action<Response<List<GameListModel>>> callback)
    {
        GameListCallback = callback;
        var tempobj = new SFSObject();
        if (string.IsNullOrEmpty(PlayerPrefsManager.GetAuthToken()))
        {
            tempobj.PutUtfString(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        }
        var request = new ExtensionRequest(Constants.REQUEST.GAMES, tempobj);
        sfs.Send(request);
    }

    public void GetConfig()
    {
        var tempobj = new SFSObject();
        if (string.IsNullOrEmpty(PlayerPrefsManager.GetAuthToken()))
        {
            tempobj.PutUtfString(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        }
        var request = new ExtensionRequest(Constants.REQUEST.CONFIG, tempobj);
        sfs.Send(request);
    }

    private Action<Response<List<TournamentListModel>>> TournamentListCallback;
    public void TournamentListRequest(Action<Response<List<TournamentListModel>>> callback)
    {
        TournamentListCallback = callback;
        var tempobj = new SFSObject();
        if (string.IsNullOrEmpty(PlayerPrefsManager.GetAuthToken()))
        {
            tempobj.PutUtfString(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
        }
        var request = new ExtensionRequest(Constants.REQUEST.TOURNAMENT_LIST, tempobj);
        sfs.Send(request);
    }

    private Action<Response<string>> JoinTableCallback;
    public void JoinTableRequest(string game_Id, string table_Id, Action<Response<string>> callback)
    {
        JoinTableCallback = callback;
        var tempobj = new SFSObject();
        if (game_Id != null)
            tempobj.PutUtfString(Constants.KEYS.game_id, game_Id);
        if (table_Id != null)
            tempobj.PutUtfString(Constants.KEYS.table_id, table_Id);

        tempobj.PutBool(Constants.KEYS.is_turbo, isTurboPlay);
        tempobj.PutBool(Constants.KEYS.is_private_table, false);
        var request = new ExtensionRequest(Constants.REQUEST.JOIN_TABLE, tempobj);
        sfs.Send(request);
    }

    private Action<Response<PrivateTableModel>> CreatePrivateTableCallback;
    public void CreatePrivateTableRequest(string game_Id, Action<Response<PrivateTableModel>> callback)
    {
        CreatePrivateTableCallback = callback;
        var tempobj = new SFSObject();
        if (game_Id != null)
            tempobj.PutUtfString(Constants.KEYS.game_id, game_Id);

        tempobj.PutBool(Constants.KEYS.is_turbo, isTurboPlay);
        tempobj.PutBool(Constants.KEYS.is_private_table, true);
        var request = new ExtensionRequest(Constants.REQUEST.CREATE_PRIVATE_TABLE, tempobj);
        sfs.Send(request);
    }

    private Action<Response<PrivateTableModel>> goldCoinValueCallback;
    public void GoldCoinValueRequestRequest(Action<Response<PrivateTableModel>> callback)
    {
        goldCoinValueCallback = callback;
        var tempobj = new SFSObject();
        var request = new ExtensionRequest(Constants.REQUEST.COINS_VALUE, tempobj);
        sfs.Send(request);
    }

    private Action<Response<string>> JoinPrivateTableCallback;
    public void JoinPrivateTableRequest(string table_Id, Action<Response<string>> callback)
    {
        JoinPrivateTableCallback = callback;
        var tempobj = new SFSObject();
        if (table_Id != null)
            tempobj.PutUtfString(Constants.KEYS.table_id, table_Id);

        tempobj.PutBool(Constants.KEYS.is_turbo, isTurboPlay);
        tempobj.PutBool(Constants.KEYS.is_private_table, true);
        var request = new ExtensionRequest(Constants.REQUEST.JOIN_PRIVATE_TABLE, tempobj);
        sfs.Send(request);
    }

    public Action<Response<GameTableResponse>> GameTableCallback;
    public void GameTableRequest(GameTableRequest gameTableRequest)
    {
        var request = new ExtensionRequest(Constants.REQUEST.GAME_PLAY_EVENT, gameTableRequest.ToISfsObject());
        if (sfs != null && sfs.IsConnected)
        {
            sfs.Send(request);
        }

    }
    public void GameTableRequest(ISFSObject sfsObject)
    {
        var request = new ExtensionRequest(Constants.REQUEST.GAME_PLAY_EVENT, sfsObject);
        sfs.Send(request);
    }


    private Action<Response<UserModel>> UserProfileCallback;
    public void UserProfileRequest(Action<Response<UserModel>> callback)
    {
        UserProfileCallback = callback;
        var request = new ExtensionRequest(Constants.REQUEST.USER_INFORMATION, new SFSObject());
        if (sfs != null && sfs.IsConnected)
        {
            sfs.Send(request);
        }
    }

    private Action<Response<object>> RefreshChipsCallback;
    public void RefreshFunChips(Action<Response<object>> callback)
    {
        RefreshChipsCallback = callback;
        var tempobj = new SFSObject();
        //var chipsValue = Convert.ToInt32(GamePlayManager.instance.gameTableEventHandler.fortuneWheel.Prizetext.text);
        //tempobj.PutInt(Constants.KEYS.chips, 8000);
        var request = new ExtensionRequest(Constants.REQUEST.UPDATE_CHIPS, tempobj);
        sfs.Send(request);
    }

    private Action<Response<object>> RefreshCoinsCallback;
    public void RefreshGoldCoins(Action<Response<object>> callback)
    {
        RefreshCoinsCallback = callback;
        var tempobj = new SFSObject();
        var coinsValue = Convert.ToInt32(GamePlayManager.instance.gameTableEventHandler.fortuneWheel.Prizetext.text);
        tempobj.PutInt(Constants.KEYS.coins, coinsValue);
        var request = new ExtensionRequest(Constants.REQUEST.UPDATE_COINS, tempobj);
        sfs.Send(request);
    }

    // call in invoke repeating
    private void ClientPingingRequest()
    {
        if ((UIManager.instance.gameRoom.activeInHierarchy
             || UIManager.instance.lobbyView.gameObject.activeInHierarchy)
            && sfs != null && sfs.IsConnected
            && Application.internetReachability != NetworkReachability.NotReachable)
        {
            var request = new ExtensionRequest(Constants.REQUEST.CLIENT_PINGING, new SFSObject());
            sfs.Send(request);
        }
    }

    private Action<Response<List<OnGoingGameListModel>>> onGoingGameListCallback;
    public void OnGoingGameListRequest(Action<Response<List<OnGoingGameListModel>>> callback)
    {
        onGoingGameListCallback = callback;
        var request = new ExtensionRequest(Constants.REQUEST.ON_GOING_GAME_LIST, new SFSObject());
        sfs.Send(request);
    }

    private Action<Response<GameTableResponse.LiveGameModel>> lastRoundResultCallback;
    public void LAST_ROUND_RESULT(string tableid, Action<Response<GameTableResponse.LiveGameModel>> callback)
    {
        lastRoundResultCallback = callback;
        var sfsObj = new SFSObject();
        sfsObj.PutUtfString(Constants.KEYS.table_id, tableid);
        var request = new ExtensionRequest(Constants.REQUEST.LAST_ROUND_RESULT, sfsObj);
        sfs.Send(request);

    }

    private Action<Response<ScoreboardModel>> scoreboardCallback;
    public void SCORE_BOARD(string tableid, Action<Response<ScoreboardModel>> callback)
    {
        scoreboardCallback = callback;
        var sfsObj = new SFSObject();
        sfsObj.PutUtfString(Constants.KEYS.table_id, tableid);
        var request = new ExtensionRequest(Constants.REQUEST.SCORE_BOARD, sfsObj);
        sfs.Send(request);

    }

    public void SendRejoinAcceptance(bool isAccept)
    {
        var tempobj = new SFSObject();
        if (CacheMemory.RunningTableId != null)
            tempobj.PutUtfString(Constants.KEYS.table_id, CacheMemory.RunningTableId);

        tempobj.PutBool(Constants.KEYS.is_accept, isAccept);
        var request = new ExtensionRequest(Constants.REQUEST.REJOIN_GAME, tempobj);
        sfs.Send(request);
    }

    public void SendSplitAcceptance(bool isAccept)
    {
        var tempobj = new SFSObject();
        if (CacheMemory.RunningTableId != null)
            tempobj.PutUtfString(Constants.KEYS.table_id, CacheMemory.RunningTableId);

        tempobj.PutBool(Constants.KEYS.is_accept, isAccept);
        var request = new ExtensionRequest(Constants.REQUEST.SPLIT, tempobj);
        sfs.Send(request);
    }



    // call in invoke repeating
    private void CheckInternetReachability()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (sfs == null || !sfs.IsConnected)
            {
                if (!UIManager.instance.splashView.gameObject.activeInHierarchy)
                {
                    Initialise();
                }
            }
            if (sfs != null && sfs.IsConnected)
            {
                if (loader.gameObject.activeInHierarchy == true)
                {
                    StartCoroutine(DelayOnReconnect());
                }
            }

        }
        else if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (loader.gameObject.activeInHierarchy == false)
            {
                loader.ShowLoader("Please check your internet connection");
                GamePlayManager.instance.gameTableEventHandler.buttonStats.isGameSarted = false;
            }
        }
    }

    private IEnumerator DelayOnReconnect()
    {
        yield return new WaitForSeconds(1f);
        loader.HideLoader();
    }
}


