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
	public Canvas dialogCanvas;
	public Text dialogContent;
	public Button dialogButton;

	private AsyncOperation mAsyncOperation;

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Confined;
		settingsCanvas.gameObject.SetActive(false);
		settingsButton.onClick.AddListener (OnSettingsButtonClick);
		closeButton.onClick.AddListener (OnCloseButtonClick);
		playButton.onClick.AddListener (OnPlayButtonClick);
		mAsyncOperation = SceneManager.LoadSceneAsync("demo");
		mAsyncOperation.allowSceneActivation = false;
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
		if (PhotonNetwork.isMasterClient)
		{
			// 加载关卡前临时禁用进一步的网络信息处理
			//PhotonNetwork.isMessageQueueRunning = false;
			//SceneManager.LoadScene(2);
			mAsyncOperation.allowSceneActivation = true;
		}
		else {
			if (!PhotonNetwork.connected)
			{
				// 未连接
				showDialog("未连接到服务器，请检查你的网络连接或重启游戏");
			}
			else {
				if (!PhotonNetwork.inRoom)
				{
					// 不在房间里
                	showDialog("你当前未加入任何房间，请加入或创建房间");
				}
				else { 
					// 不是房主
                	showDialog("你不是房主，只有房主可以开始游戏");
				}
			}
		}
	}

	private void showDialog(string dialogText) {
		dialogCanvas.gameObject.SetActive(true);
		dialogContent.text = dialogText;
	}
}
