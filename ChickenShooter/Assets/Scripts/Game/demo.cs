using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class demo : Photon.PunBehaviour {

	public GameObject playerPrefab;
	public Canvas packCanvas;
	public Canvas dialogCanvas;
	public Canvas ESCCanvas;

	public Text dialogContent;
	public Button dialogButton;
	private bool isDead = false;
	private PhotonPlayer currentPlayer;

	public void Awake()
	{
		// in case we started this demo with the wrong scene being active, simply load the menu scene
		if (!PhotonNetwork.connected)
		{
			SceneManager.LoadScene(1);
			return;
		}
		//PhotonNetwork.isMessageQueueRunning = true;
		// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
		// PhotonNetwork.Instantiate(this.playerPrefab.name, transform.position, Quaternion.identity, 0); 
	}


	// Use this for initialization
	void Start () {
		PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f,100f,0f),Quaternion.identity, 0);
		packCanvas.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (!packCanvas.isActiveAndEnabled && 
		    !dialogCanvas.isActiveAndEnabled && 
		    !ESCCanvas.isActiveAndEnabled) {
			Cursor.lockState = CursorLockMode.Locked;
		}

		if (Input.GetKeyDown (KeyCode.Tab)) {
			if (packCanvas.isActiveAndEnabled) {
				packCanvas.gameObject.SetActive (false);
				Cursor.lockState = CursorLockMode.Locked;
			} else {
				packCanvas.gameObject.SetActive (true);
				Cursor.lockState = CursorLockMode.Confined;
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
		{
			if (ESCCanvas.isActiveAndEnabled)
			{
				ESCCanvas.gameObject.SetActive(false);
				Cursor.lockState = CursorLockMode.Locked;
			}
			else
			{
				ESCCanvas.gameObject.SetActive(true);
				Cursor.lockState = CursorLockMode.Confined;
			}
		}

		if (isDead) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				PhotonNetwork.DestroyPlayerObjects(currentPlayer);
				PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f,100f,0f),Quaternion.identity, 0);
				dialogCanvas.gameObject.SetActive(false);
				isDead = false;
			}
		}
	}


	public void OnMasterClientSwitched(PhotonPlayer player)
	{
		Debug.Log("OnMasterClientSwitched: " + player);

		string message;
		InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message

		if (chatComponent != null)
		{
			// to check if this client is the new master...
			if (player.IsLocal)
			{
				message = "You are Master Client now.";
			}
			else
			{
				message = player.NickName + " is Master Client now.";
			}


			chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
		}
	}


	override public void OnDisconnectedFromPhoton()
	{
		Debug.Log("OnDisconnectedFromPhoton");

		// back to main menu
		SceneManager.LoadScene(1);
	}

override public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
	}

override public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerConnected: " + player);
	}

override public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("OnPlayerDisconneced: " + player);
	}

	public void OnFailedToConnectToPhoton()
	{
		Debug.Log("OnFailedToConnectToPhoton");

		// back to main menu
		SceneManager.LoadScene(1); 
	}


	public void showDiedDialog() { 
		if (!dialogCanvas.gameObject.GetActive()) {
			Cursor.lockState = CursorLockMode.Confined;
			isDead = true;
                showDialog("BETTER LUCK NEXT TIME!");
				dialogButton.onClick.AddListener(() =>
				{
					PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f,100f,0f),Quaternion.identity, 0);
					isDead = false;
				});
			}	
	}

	public void showDialog(string dialogText)
	{
		dialogCanvas.gameObject.SetActive(true);
		dialogContent.text = dialogText;	
	}

	public void bindPlayer(PhotonPlayer player)
	{
		this.currentPlayer = player;
	}

}
