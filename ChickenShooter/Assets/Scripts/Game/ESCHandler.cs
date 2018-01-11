using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ESCHandler : MonoBehaviour {

	public Canvas ESCCanvas;
	public Button cancelButton;
	public Button homeButton;
	public Button exitButton;


	// Use this for initialization
	void Start () {
		cancelButton.onClick.AddListener(() =>
		{
			ESCCanvas.gameObject.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
		});
		homeButton.onClick.AddListener(() => { 
			ESCCanvas.gameObject.SetActive(false);
			PhotonNetwork.Disconnect();
			SceneManager.LoadScene(0);
		});
		exitButton.onClick.AddListener(() => {
			Application.Quit();
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
