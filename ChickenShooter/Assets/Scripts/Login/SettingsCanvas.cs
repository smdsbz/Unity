using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SettingsCanvas : MonoBehaviour {
	public Canvas settingsCanvas;
	public Button saveButton;
	public Button cancelButton;

	// Use this for initialization
	void Start () {
		saveButton.onClick.AddListener (OnSaveButtonClick);
		cancelButton.onClick.AddListener (OnCancelButtonClick);
	}

	// Update is called once per frame
	void Update () {

	}

	private void OnSaveButtonClick () {
		// TODO: save sth.
		SaveSettings();
		settingsCanvas.gameObject.SetActive(false);
	}

	private void OnCancelButtonClick () {
		settingsCanvas.gameObject.SetActive(false);
	}

	private void SaveSettings () {
	}
}
