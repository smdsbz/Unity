using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public GameObject playerRightHand;
	public GameObject flame;

	float flameDuration = -1f;
	public float fireRate = -1f;
	GameObject clone_flame;


	void Awake() {

		this.transform.parent = playerRightHand.transform;
	}


	// Update is called once per frame
	void Update () {

		// GunFlare_FX
		if (Input.GetKeyDown(KeyCode.Mouse0) && fireRate < 0) {
			clone_flame = Instantiate(flame, this.transform.position,
					this.transform.rotation, this.transform) as GameObject;
			clone_flame.transform.localPosition += new Vector3(0, 0, 60f);
			flameDuration = 0.13f;
			fireRate = 0.8f;
		}

		if (flameDuration > 0f)	{
			flameDuration -= Time.deltaTime;
		} else if (clone_flame != null) {
			Destroy(clone_flame.gameObject);
		}

		if (!(fireRate < 0f)) {
			fireRate -= Time.deltaTime;
		}

	}	// Update

}
