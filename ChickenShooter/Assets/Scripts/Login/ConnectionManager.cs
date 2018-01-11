using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : Photon.PunBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		Debug.Log("有人连接成功" + newPlayer.NickName);
		Debug.Log("是不是主机？" + PhotonNetwork.isMasterClient);
	}


}
