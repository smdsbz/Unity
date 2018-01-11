// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerMenu.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class NetworkManager : Photon.PunBehaviour
{
	public InputField playerName;
	public InputField roomName;
	//public GameObject roomList;
	public Text serverInfoText;
	public Text infoText;
	public Button joinRoomButton;
	public Button randomJoinRoomButton;
	public Button createRoomButton;
	public GameObject roomObject;
	public ScrollRect scrollView;
	public Vector2 scrollPos = new Vector2(600, 400);


	public Canvas dialogCanvas;
	public Text dialogContent;
	public Button dialogButton;

	private Dictionary<string, GameObject> roomList = new Dictionary<string, GameObject>();
	private bool connectFailed = false;


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
		PhotonNetwork.autoJoinLobby = false;

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
		PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
		playerName.text = PhotonNetwork.playerName;
		roomName.text = "myRoom";

		// if you wanted more debug out, turn this on:
		// PhotonNetwork.logLevel = NetworkLogLevel.Full;
	}


	public void Start()
	{
		createRoomButton.onClick.AddListener(delegate () {
			PhotonNetwork.CreateRoom(roomName.text, new RoomOptions() { MaxPlayers = 0 }, null);
		});

		joinRoomButton.onClick.AddListener(delegate () {
			PhotonNetwork.JoinRoom(roomName.text);
		});

		randomJoinRoomButton.onClick.AddListener(delegate () {
			PhotonNetwork.JoinRandomRoom();
		});



	}


	public void OnGUI()
	{
		
		if (!PhotonNetwork.connected)
		{
			if (PhotonNetwork.connecting)
			{
				GUILayout.Label("Connecting to: " + PhotonNetwork.ServerAddress);
			}
			else
			{
				GUILayout.Label("Not connected. Check console output. Detailed connection state: " + 
				                PhotonNetwork.connectionStateDetailed + " Server: " + 
				                PhotonNetwork.ServerAddress);
			}

			if (this.connectFailed)
			{
				GUILayout.Label("Connection failed. Check setup and use Setup Wizard to fix configuration.");
				GUILayout.Label(String.Format("Server: {0}", new object[] { PhotonNetwork.ServerAddress }));
				GUILayout.Label("AppId: " + PhotonNetwork.PhotonServerSettings.AppID.Substring(0, 8) + "****"); 
				// only show/log first 8 characters. never log the full AppId.

				if (GUILayout.Button("Try Again", GUILayout.Width(100)))
				{
					this.connectFailed = false;
					PhotonNetwork.ConnectUsingSettings("1.0");
				}
			}

			return;
		}

		PhotonNetwork.playerName = playerName.text;
		if (true)
		{
			// Save name
			PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
		}






		if (!string.IsNullOrEmpty(ErrorDialog))
		{
			GUILayout.Label(ErrorDialog);

			if (this.timeToClearDialog < Time.time)
			{
				this.timeToClearDialog = 0;
				ErrorDialog = "";
			}
		}


		// Join random room

		serverInfoText.text = "当前在线玩家数：" + PhotonNetwork.countOfPlayers + "当前房间数：" + PhotonNetwork.countOfRooms;



		if (PhotonNetwork.GetRoomList().Length == 0)
		{
			infoText.text = "当前没有可加入的房间\n" + "当有可加入的房间时，此处会显示";
		}
		else
		{
			infoText.text = "当前可用房间：" + PhotonNetwork.GetRoomList().Length;
			// Room listing: simply call GetRoomList: no need to fetch/poll whatever!
			foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
			{
				if (!roomList.ContainsKey(roomInfo.Name))
				{
					GameObject newRoom = Instantiate(roomObject) as GameObject;
					newRoom.GetComponentInChildren<Text>().text = roomInfo.Name;
					newRoom.GetComponentInChildren<Button>().onClick.AddListener(() =>
					{
						PhotonNetwork.JoinRoom(roomInfo.Name);
					});
					newRoom.transform.SetParent(scrollView.viewport.transform, false);
					roomList.Add(roomInfo.Name, newRoom);
				}
				// TODO: 显示房间名
				//GUILayout.Label(roomInfo.Name + " " + roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers);
				//if (GUILayout.Button("加入", GUILayout.Width(150)))
				//{
					//PhotonNetwork.JoinRoom(roomInfo.Name);
			//	}

			}
			//GUILayout.BeginVertical();  
			//GUILayout.Space(Screen.height / 6 * 5);
			//this.scrollPos = GUILayout.BeginScrollView(scrollPos);
   //         foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())

			//{
			//	GUILayout.BeginHorizontal();
			//	GUILayout.Space(Screen.width / 12);
			//	GUILayout.Label(roomInfo.Name + " " + roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers);
			//	if (GUILayout.Button("加入", GUILayout.Width(150)))
			//	{
			//		PhotonNetwork.JoinRoom(roomInfo.Name);
			//	}

			//	GUILayout.EndHorizontal();
			//}

			//GUILayout.EndScrollView();
			//GUILayout.EndVertical();
		}

	}

	// We have two options here: we either joined(by title, list or random) or created a room.
	override public void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom");
	}

	public void OnPhotonCreateRoomFailed()
	{
		showDialog("Error: Can't create room (room name maybe already used).");
		Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
	}

	override public void OnPhotonJoinRoomFailed(object[] cause)
	{
		showDialog("Error: Can't join room (full or unknown room name)" + cause[1]);
		Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
	}

	public void OnPhotonRandomJoinFailed()
	{
		showDialog("无法随机加入房间（当前没有房间）");
		Debug.Log("OnPhotonRandomJoinFailed got called. Happens if no room is available (or all full or invisible or closed). JoinrRandom filter-options can limit available rooms.");
	}

	override public void OnCreatedRoom()
	{
		Debug.Log("OnCreatedRoom");
		//PhotonNetwork.LoadLevel(SceneNameGame);
		infoText.text = "房间创建成功";
		showDialog("房间创建成功，点击PLAY开始游戏");
		joinRoomButton.gameObject.SetActive(false);
		randomJoinRoomButton.gameObject.SetActive(false);
		createRoomButton.gameObject.SetActive(false);
		scrollView.gameObject.SetActive(false);
	}

	override public void OnDisconnectedFromPhoton()
	{
		showDialog("与服务器的连接已断开");
	}

	public void OnFailedToConnectToPhoton(object parameters)
	{
		this.connectFailed = true;
		showDialog("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.ServerAddress);
	}

	override public void OnConnectedToMaster()
	{
		Debug.Log("As OnConnectedToMaster() got called, the PhotonServerSetting.AutoJoinLobby must be off. Joining lobby by calling PhotonNetwork.JoinLobby().");
		PhotonNetwork.JoinLobby();
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		Debug.Log("有人连接成功" + newPlayer.NickName);
		Debug.Log("是不是主机？" + PhotonNetwork.isMasterClient);
	}


	private void showDialog(string dialogText)
	{
		dialogCanvas.gameObject.SetActive(true);
		dialogContent.text = dialogText;	
	}

}
