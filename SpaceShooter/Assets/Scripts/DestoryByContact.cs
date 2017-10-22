using UnityEngine;
using System.Collections;

public class DestoryByContact : MonoBehaviour
{
	// 小行星爆炸时的粒子对象
	public GameObject explosion;
	// 飞船与小行星碰撞飞船爆炸的粒子对象
	public GameObject playerExplosion;
	// 表示小行星被击中后玩家分值增加的数量
	public int scoreValue;
	// 表示在游戏对象 GameController 上绑定的脚本 GameController.cs
	private GameController gameController;

	void Start ()
	{
		GameObject go = GameObject.FindWithTag ("GameController");
		if (go != null) {
			gameController = go.GetComponent<GameController> ();
		} else {
			Debug.Log ("Cannot Find a tag of GameController");
		}
		if (gameController == null) {
			Debug.Log ("Cannot Find the Script of GameController.cs");
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Boundary" || other.tag == "Enemy") {
			return;
		}
		if (explosion != null) {
			// 在小行星销毁的位置生成一个爆炸效果, explosion 是小行星的位置
			Instantiate (explosion, transform.position, transform.rotation);  
			gameController.AddScore (scoreValue);
		} 

		if (other.tag == "Player") {
			// 在玩家飞机销毁的位置生成一个爆炸效果, playerExplosion 是飞船的位置
			Instantiate (playerExplosion, other.transform.position, other.transform.rotation);  
			gameController.GameOver ();
		}
		// 销毁跟小行星碰撞的物体
		Destroy (other.gameObject);  
		// 销毁小行星
		Destroy (this.gameObject);   
	}
}
