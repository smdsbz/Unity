using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demo : MonoBehaviour {

	public Canvas packCanvas;
	// Use this for initialization
	void Start () {
		packCanvas.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (!packCanvas.isActiveAndEnabled) {
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
	}
}
