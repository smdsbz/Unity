using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainCanvas : MonoBehaviour {

	public Canvas settingsCanvas;
	public Button settingsButton;
	public Button closeButton;

	// Use this for initialization
	void Start () {
		settingsCanvas.gameObject.SetActive(false);
		settingsButton.onClick.AddListener (OnSettingsButtonClick);
		closeButton.onClick.AddListener (OnCloseButtonClick);
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
}
