using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainCanvas : MonoBehaviour {

	public Canvas settingsCanvas;
	public Button settingsButton;
	public Button closeButton;
	public Button playButton;

	// Use this for initialization
	void Start () {
		settingsCanvas.gameObject.SetActive(false);
		settingsButton.onClick.AddListener (OnSettingsButtonClick);
		closeButton.onClick.AddListener (OnCloseButtonClick);
		playButton.onClick.AddListener (OnPlayButtonClick);
	}

	// Update is called once per frame
	void Update () {

	}

	private void OnSettingsButtonClick () {
		settingsCanvas.gameObject.SetActive(true);
	}

	private void OnCloseButtonClick () {
		Application.Quit ();
	}

	private void OnPlayButtonClick () {
		SceneManager.LoadScene (1);
	}
}
