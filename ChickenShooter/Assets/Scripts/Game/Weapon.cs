using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{

	public GameObject playerRightHand;	// TODO: temporary
	public GameObject flame;

	float flameDuration = -1f;
	public float fireRate = -1f;
	GameObject clone_flame;
	Player player;


	void Awake()
	{
		this.transform.parent = playerRightHand.transform;
		player = this.GetComponentInParent<Player>();		// TODO: temporary
	}



	void PlayFireAnimation(Player player)
	{
		clone_flame = Instantiate(flame, this.transform.position,
			this.transform.rotation, this.transform) as GameObject;
		clone_flame.transform.localPosition += new Vector3(0, 0, 60f);
	}



	GameObject CheckBulletHit(Player player)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 100)) {
			Debug.DrawLine(ray.origin, hit.point);
			Debug.Log(hit.collider.gameObject);

			// causing damage
			if (hit.collider.gameObject.CompareTag("Player")) {
				Player other = hit.collider.gameObject.GetComponent(typeof(Player)) as Player;
				other.life -= 1;
				Debug.Log(other);
			}
			return hit.collider.gameObject;
		}
		return null;
	}



	void Update()
	{
		// be careful of memory leaks!
		if (flameDuration > 0f) {
			flameDuration -= Time.deltaTime;
		} else if (clone_flame != null) {
			Destroy(clone_flame.gameObject);
		}
		// control fire rate
		if (!(fireRate < 0f)) {
			fireRate -= Time.deltaTime;
		}

		// don't do anything if the guns not pick up
		if (player == null) {
			return;
		}

		// fire gun
		if (Input.GetKeyDown(KeyCode.Mouse0) && fireRate < 0) {
			PlayFireAnimation(player);	// play vfx
			CheckBulletHit(player);		// check if hit something
			flameDuration = 0.13f;
			fireRate = 0.53f;
		}
	}


}
