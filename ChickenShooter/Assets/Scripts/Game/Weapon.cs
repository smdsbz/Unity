using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(PhotonView))]
public class Weapon : Photon.PunBehaviour
{

	public GameObject playerRightHand;	// TODO: temporary
	public GameObject flame;

	int magazine_size = 300;

	float flameDuration = -1f;
	float fireRate = -1f;
	GameObject clone_flame;
	Player player;


	void Awake()
	{
		this.transform.parent = playerRightHand.transform;
		player = this.GetComponentInParent<Player>();		// TODO: temporary
	}



	public bool PlayFireAnimation(Player player)
	{	/* Instantiate VFX, Audio clip
	     *
	     * Args:
	     * 		Player player	- the player who's firing this weapon
	     * 
	     * Return:
	     * 		(no return)
		 */
		if (this.magazine_size <= 0) {
			return false;
		}

		if (this.fireRate > 0f) {
			return false;
		}

		// VFX
		clone_flame = Instantiate(flame, this.transform.position,
			this.transform.rotation, this.transform) as GameObject;
		clone_flame.transform.localPosition += new Vector3(0, 0, 60f);
		this.flameDuration = 0.13f;
		this.fireRate = 0.53f;

		// Weapon Asset control
		this.magazine_size -= 1;

		// Causing damage
		this.CheckBulletHit(this.player);

//		player.anim.SetTrigger("Fire");
		return true;
	}



	public GameObject CheckBulletHit(Player player)
	{	/*	Check if hit a <Player>
		 *	
		 *	Args:
		 *		player	- self
		 *
		 *	Return:
		 *		the GameObject that is hit, 
		 *		or null if no player's hit
		 */
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 100)) {
			Debug.DrawLine(ray.origin, hit.point);
			Debug.Log(hit.collider.gameObject);

			// causing damage
			if (hit.collider.gameObject.CompareTag("Player")) {
				Player other = hit.collider.gameObject.GetComponent(typeof(Player)) as Player;
				other.life -= 1;
//				PhotonView photonView = PhotonView.Get(this);
//				photonView.RPC("MinusLife", PhotonTargets.All);
				player.CostOtherLife(other);
				Debug.Log(other);
				Debug.Log(other.life);
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
		if (fireRate > 0f) {
			fireRate -= Time.deltaTime;
		}

		// don't do anything if the guns not pick up
		if (player == null) {
			return;
		}

	}
}
