using Sfs2X.Entities.Data;
using System;
using UnityEngine;
using UnityEngine.Android;

public class BaseMonoBehaviour : MonoBehaviour
{
    public void JoinTable(GameListModel model, string table_id, string game_id)
    {
        if (model != null)
        {
            string latitude = LocationManager.instance.latitude;
            string longitude = LocationManager.instance.longitude;

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude))
                {
                    latitude = null;
                    longitude = null;
                }
                else if (!Input.location.isEnabledByUser || !Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    latitude = null;
                    longitude = null;
                }
            }

            if (latitude != null && longitude != null)
            {
                var form = new WWWForm();
                form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
                form.AddField("latitude", latitude);
                form.AddField("longitude", longitude);
                form.AddField("game_type", model.game_type);
                Debug.Log(Constants.API_METHODS.CHECK_LOCATION + "?auth_token=" + PlayerPrefsManager.GetAuthToken() + "&latitude=" + latitude + "&longitude=" + longitude + "&game_type=" + model.game_type);
                StartCoroutine(APIManager.instance.CheckLocation(form, response =>
                {
                    if (response.status == "valid") {
                        PlayGame(model: model, table_id: table_id, game_id: game_id);
                    } else {
                        ServerManager.instance.alertPopUp.ShowView(response.message);
                    }
                }));
            }
            else
            {
                LocationManager.instance.CheckForLocation();
            }
            /* if (model.game_type.Equals(Constants.GAME_TYPE.PRACTICE))
             {
                 PlayGame(model: model, table_id: table_id, game_id: game_id);
             }
             else
             {
                 string latitude = LocationManager.instance.latitude;
                 string longitude = LocationManager.instance.longitude;
                 if (latitude != null && longitude != null)
                 {
                     var form = new WWWForm();
                     form.AddField(Constants.KEYS.auth_token, PlayerPrefsManager.GetAuthToken());
                     form.AddField("latitude", latitude);
                     form.AddField("longitude", longitude);
                     Debug.Log(Constants.API_METHODS.CHECK_LOCATION + "?auth_token=" + PlayerPrefsManager.GetAuthToken() + "&latitude=" + longitude + "&longitude=" + longitude);
                     StartCoroutine(APIManager.instance.CheckLocation(form, response =>
                     {
                         PlayGame(model: model, table_id: table_id, game_id: game_id);
                     }));
                 }
                 else
                 {
                     LocationManager.instance.CheckForLocation();
                 }
             }*/
        }
        else
        {

            if (table_id != null)
            {
                TableResponseHandler(table_id, null);
            }
            else if (game_id != null)
            {
                TableResponseHandler(table_id, game_id);
            }
            else
            {
                ServerManager.instance.alertPopUp.ShowView("Invalid join request, please rejoin new game");
            }
        }
    }


    private void PlayGame(GameListModel model, string table_id, string game_id)
    {
        if (model != null)
        {
            string id = null;
            if (model != null)
            {
                id = model.id;
            }
            TableResponseHandler(table_id, id);
            GamePlayManager.instance.gameTableEventHandler.buttonStats.isGameSarted = false;
            GamePlayManager.instance.gameTableEventHandler.buttonStats.autoDrop.isOn = false;
        }
        else
        {
            if (table_id != null)
            {
                TableResponseHandler(table_id, null);
            }
            else if (game_id != null)
            {
                TableResponseHandler(table_id, game_id);
            }
            else
            {
                ServerManager.instance.alertPopUp.ShowView("Invalid join request, please rejoin new game");
            }
        }
    }

    public void DeactivateSubGameView()
    {
        if (UIManager.instance.poolRummyView.gameObject.activeInHierarchy)
        {
            StartCoroutine(UIManager.instance.poolRummyView.PoolRummyExitEffect());
        }

        if (UIManager.instance.pointRummyView.gameObject.activeInHierarchy)
        {
            StartCoroutine(UIManager.instance.pointRummyView.PointRummyExitEffect());
        }

        if (UIManager.instance.dealRummyView.gameObject.activeInHierarchy)
        {
            StartCoroutine(UIManager.instance.dealRummyView.DealRummyExitEffect());
        }

        if (UIManager.instance.privateTableView.privateTableGameSelectionView
            .gameObject.activeInHierarchy)
        {
            StartCoroutine(UIManager.instance.privateTableView.
                privateTableGameSelectionView.PrivateTableSelectionExitEffect());
        }

        if (UIManager.instance.privateTableView.gameObject.activeInHierarchy)
        {
            StartCoroutine(UIManager.instance.privateTableView.PrivateTableExitEffect());
        }
    }

    private void TableResponseHandler(string table_id, string id)
    {
        ServerManager.instance.JoinTableRequest(id, table_id, (response) =>
        {
            if (response.status.Equals(Constants.KEYS.valid))
            {
                ServerManager.instance.loader.HideLoader();

                UIManager.instance.lobbyView.gameObject.SetActive(false);
                UIManager.instance.gameRoom.SetActive(true);

                CacheMemory.RunningTableId = response.data;
                SendGameTableRequest(Constants.GAME_TABLE_EVENT.PLAYER_TAKE_SEAT, response.data);
            }
            else
            {
#if UNITY_WEBGL
                ServerManager.instance.loader.HideLoader();
#endif
                ServerManager.instance.alertPopUp.ShowView(response.message, () =>
                {
#if UNITY_WEBGL
                    Application.ExternalEval("Quit()");
#else
                    if (!UIManager.instance.gameRoom.gameObject.activeInHierarchy) return;
                    GamePlayManager.instance.ResetSlots();
                    var orientationController = new OrientationController();

                    UIManager.instance.gameRoom.SetActive(false);
                    UIManager.instance.lobbyView.gameObject.SetActive(true);
#endif
                }, "Ok", null, null);
            }
        });
    }

    public void CreatePrivateTableResponseHandler(string game_Id)
    {
        ServerManager.instance.CreatePrivateTableRequest(game_Id, (response) =>
        {
            if (response.status.Equals(Constants.KEYS.valid))
            {
                CacheMemory.privateTableCode = response.data.privateTableId;
                ServerManager.instance.loader.HideLoader();

                UIManager.instance.lobbyView.gameObject.SetActive(false);
                UIManager.instance.gameRoom.gameObject.SetActive(true);

                CacheMemory.RunningTableId = response.data.privateTableId;
                CacheMemory.privateTableHostAuthToken = response.data.userAuthToken;
                SendGameTableRequest(Constants.GAME_TABLE_EVENT.PLAYER_TAKE_SEAT, response.data.privateTableId);
            }
            else
            {
                ServerManager.instance.alertPopUp.ShowView(response.message);
            }
        });
    }

    public void JoinPrivateTableResponseHandler(string table_Id)
    {
        ServerManager.instance.JoinPrivateTableRequest(table_Id, (response) =>
        {
            if (response.status.Equals(Constants.KEYS.valid))
            {
                ServerManager.instance.loader.HideLoader();

                UIManager.instance.lobbyView.gameObject.SetActive(false);
                UIManager.instance.gameRoom.SetActive(true);

                CacheMemory.RunningTableId = response.data;
                SendGameTableRequest(Constants.GAME_TABLE_EVENT.PLAYER_TAKE_SEAT, response.data);
            }
            else
            {
                ServerManager.instance.alertPopUp.ShowView(response.message);
            }
        });
    }

    public void SendGameTableRequest(string tableEventType, string tableId)
    {
        var model = new GameTableRequest();
        model.table_id = tableId;
        model.table_event_type = tableEventType;
        ServerManager.instance.GameTableRequest(model);
    }

    public void SendGameTableRequest(ISFSObject sfsObject)
    {
        ServerManager.instance.GameTableRequest(sfsObject);
    }

    public void SendProfileRequest(Action<Response<UserModel>> callback)
    {
        ServerManager.instance.UserProfileRequest(callback);
    }
}
