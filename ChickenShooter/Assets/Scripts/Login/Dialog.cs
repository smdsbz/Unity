using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Dialog : MonoBehaviour {

	public Canvas dialogCanvas;
	public Button dialogButton;

	// Use this for initialization
	void Start () {
		dialogButton.onClick.AddListener(() => {
			dialogCanvas.gameObject.SetActive(false);
		
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
