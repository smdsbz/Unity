// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerMenu.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using UnityEngine;
using Random = UnityEngine.Random;
using ExitGames.Client.Photon;

public class NetworkMenu : Photon.PunBehaviour
{
    public GUISkin Skin;
    public Vector2 WidthAndHeight = new Vector2(600, 400);
    private string roomName = "myRoom";

    private Vector2 scrollPos = Vector2.zero;

    private bool connectFailed = false;

    public static readonly string SceneNameMenu = "Login";

    public static readonly string SceneNameGame = "GameLoadingSplash";

    private string errorDialog;
    private double timeToClearDialog;

    public string ErrorDialog
    {
        get { return this.errorDialog; }
        private set
        {
            this.errorDialog = value;
            if (!string.IsNullOrEmpty(value))
            {
                this.timeToClearDialog = Time.time + 4.0f;
            }
        }
    }

    public void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated)
        {
            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings("1.0");
        }

        // generate a name for this player, if none is assigned yet
        if (String.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
        }

        // if you wanted more debug out, turn this on:
        // PhotonNetwork.logLevel = NetworkLogLevel.Full;
    }

    public void OnGUI()
    {
        if (this.Skin != null)
        {
            GUI.skin = this.Skin;
        }

        if (!PhotonNetwork.connected)
        {
            if (PhotonNetwork.connecting)
            {
                GUILayout.Label("Connecting to: " + PhotonNetwork.ServerAddress);
            }
            else
            {
                GUILayout.Label("Not connected. Check console output. Detailed connection state: " + PhotonNetwork.connectionStateDetailed + " Server: " + PhotonNetwork.ServerAddress);
            }

            if (this.connectFailed)
            {
                GUILayout.Label("Connection failed. Check setup and use Setup Wizard to fix configuration.");
                GUILayout.Label(String.Format("Server: {0}", new object[] {PhotonNetwork.ServerAddress}));
                GUILayout.Label("AppId: " + PhotonNetwork.PhotonServerSettings.AppID.Substring(0, 8) + "****"); // only show/log first 8 characters. never log the full AppId.

                if (GUILayout.Button("Try Again", GUILayout.Width(100)))
                {
                    this.connectFailed = false;
                    PhotonNetwork.ConnectUsingSettings("1.0");
                }
            }

            return;
        }

        Rect content = new Rect((Screen.width - this.WidthAndHeight.x)/2, (Screen.height - this.WidthAndHeight.y)/2, this.WidthAndHeight.x, this.WidthAndHeight.y);
        GUI.Box(content, "加入或创建房间");
        GUILayout.BeginArea(content);

        GUILayout.Space(40);

        // Player name
        GUILayout.BeginHorizontal();
        GUILayout.Label("玩家名称：", GUILayout.Width(150));
        PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName);
        GUILayout.Space(158);
        if (GUI.changed)
        {
            // Save name
            PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        // Join room by title
        GUILayout.BeginHorizontal();
        GUILayout.Label("房间名称：", GUILayout.Width(150));
        this.roomName = GUILayout.TextField(this.roomName);

        if (GUILayout.Button("创建房间", GUILayout.Width(150)))
        {
            PhotonNetwork.CreateRoom(this.roomName, new RoomOptions() { MaxPlayers = 10 }, null);
        }

        GUILayout.EndHorizontal();

        // Create a room (fails if exist!)
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        //this.roomName = GUILayout.TextField(this.roomName);
        if (GUILayout.Button("加入房间", GUILayout.Width(150)))
        {
            PhotonNetwork.JoinRoom(this.roomName);
        }

        GUILayout.EndHorizontal();


        if (!string.IsNullOrEmpty(ErrorDialog))
        {
            GUILayout.Label(ErrorDialog);

            if (this.timeToClearDialog < Time.time)
            {
                this.timeToClearDialog = 0;
                ErrorDialog = "";
            }
        }

        GUILayout.Space(15);

        // Join random room
        GUILayout.BeginHorizontal();

        GUILayout.Label("当前在线玩家数：" + PhotonNetwork.countOfPlayers + "当前房间数：" + PhotonNetwork.countOfRooms);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("随机加入", GUILayout.Width(150)))
        {
            PhotonNetwork.JoinRandomRoom();
        }


        GUILayout.EndHorizontal();

        GUILayout.Space(15);
        if (PhotonNetwork.GetRoomList().Length == 0)
        {
            GUILayout.Label("当前没有可加入的房间");
            GUILayout.Label("当有可加入的房间时，此处会显示");
        }
        else
        {
            GUILayout.Label(PhotonNetwork.GetRoomList().Length + "当前可用房间：");

            // Room listing: simply call GetRoomList: no need to fetch/poll whatever!
            this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);
            foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(roomInfo.Name + " " + roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers);
                if (GUILayout.Button("加入", GUILayout.Width(150)))
                {
                    PhotonNetwork.JoinRoom(roomInfo.Name);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        GUILayout.EndArea();
    }

    // We have two options here: we either joined(by title, list or random) or created a room.
    override public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }

    public void OnPhotonCreateRoomFailed()
    {
        ErrorDialog = "Error: Can't create room (room name maybe already used).";
        Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
    }

	override public void OnPhotonJoinRoomFailed(object[] cause)
    {
        ErrorDialog = "Error: Can't join room (full or unknown room name). " + cause[1];
        Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
    }

    public void OnPhotonRandomJoinFailed()
    {
        ErrorDialog = "Error: Can't join random room (none found).";
        Debug.Log("OnPhotonRandomJoinFailed got called. Happens if no room is available (or all full or invisible or closed). JoinrRandom filter-options can limit available rooms.");
    }

	override public void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        //PhotonNetwork.LoadLevel(SceneNameGame);
    }

	override public void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon.");
    }

    public void OnFailedToConnectToPhoton(object parameters)
    {
        this.connectFailed = true;
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.ServerAddress);
    }

	override public void OnConnectedToMaster()
    {
        Debug.Log("As OnConnectedToMaster() got called, the PhotonServerSetting.AutoJoinLobby must be off. Joining lobby by calling PhotonNetwork.JoinLobby().");
        PhotonNetwork.JoinLobby();
    }

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		Debug.Log("有人连接成功" + newPlayer.NickName);
		Debug.Log("是不是主机？" + PhotonNetwork.isMasterClient);	}

}
