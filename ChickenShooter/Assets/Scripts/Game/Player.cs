using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Player : NetworkBehaviour
{

	public int life = 5;

	// Component References
	Transform	playerTransform;
	CharacterController	controller;
	Animator	anim;
	Weapon		weapon;

	// Camera : Component
	Transform camTransform;
	// third person
	Vector3 camOffset = new Vector3(1f, 4.6f, -2.2f);
	// Vector3 camOffset = new Vector3(0f, 4.5f, 0f); // first person

	// Attributes
	float	walkSpeed 	= 4f;
	float	runSpeed 	= 14;
	float	gravity 	= 2f;
	// States - for animations
	bool	isJogging 	= false;
	bool	isRunning 	= false;
	bool	isAiming 	= false;



	void Start()
	{
		/* set references */
		if (!isLocalPlayer) {
			return;
		}
		// Transform : Player
		playerTransform = this.transform;
		// CharCont : Player
		controller = this.GetComponent<CharacterController>();
		// Transform : Camera
		camTransform = Camera.main.transform;
		// Animator
		anim = GetComponent <Animator>();
		// Weapon
		// TODO: temporary solution
		//       should be handled by dedicated PickUpWeapon function
		weapon = this.GetComponentInChildren(typeof(Weapon)) as Weapon;

		// Set Camera start position
		// set rotation
		camTransform.parent = playerTransform;
		camTransform.rotation = playerTransform.rotation;
		// set position
		camTransform.position = playerTransform.position;
		camTransform.localPosition += camOffset;

		// Lock mouse
		Cursor.lockState = CursorLockMode.Locked;
	}
	// Start


	void Update()
	{
		if (!isLocalPlayer) {
			return;
		}
		Contorl();
		Animate();
	}
	// Update



	void Contorl()
	{
		/*    contorl player movement */

		//// Player ////
		// Set Transform.position
		// Set player Transform
		float x = 0f, y = 0f, z = 0f;
		if (Input.GetKey(KeyCode.Space)) {
			y += 1f;
		}
		if (Input.GetKey(KeyCode.LeftControl)) {
			y -= 1f;
		}
		if (Input.GetKey(KeyCode.LeftShift)) {
			if (!isRunning) {
				isRunning = true;
			}
			y -= gravity * Time.deltaTime;
			z += Input.GetAxisRaw("Vertical") * runSpeed * Time.deltaTime;
			x += Input.GetAxisRaw("Horizontal") * runSpeed * Time.deltaTime;
		} else {
			if (isRunning) {
				isRunning = false;
			}
			y -= gravity * Time.deltaTime;
			z += Input.GetAxisRaw("Vertical") * walkSpeed * Time.deltaTime;
			x += Input.GetAxisRaw("Horizontal") * walkSpeed * Time.deltaTime;
		}
		// assign
		controller.Move(playerTransform.TransformDirection(new Vector3(x, y, z)));

		if (x != 0 || z != 0) {
			isJogging = true;
		} else {
			isJogging = false;
		}

		Vector3 newPlayerEuler = playerTransform.eulerAngles;
		newPlayerEuler.y += Input.GetAxis("Mouse X");
		playerTransform.eulerAngles = newPlayerEuler;

		//// Camera ////
		// set position
		camTransform.position = playerTransform.position;
		camTransform.localPosition += camOffset;
		// set rotation
		camTransform.eulerAngles = new Vector3(
			camTransform.localEulerAngles.x - Input.GetAxis("Mouse Y"),
			playerTransform.eulerAngles.y - 0f, // character not holding firmly
			0f // playerTransform.EulerAngles.z
		);

		// Mouse Click
		if (!isAiming && Input.GetKeyDown(KeyCode.Mouse1)) {
			isAiming = true;
		} else if (isAiming && Input.GetKeyUp(KeyCode.Mouse1)) {
			isAiming = false;
		}
	}



	void Animate()
	{
		anim.SetBool("IsJogging", isJogging);
		anim.SetBool("IsAiming", isAiming);
		anim.SetBool("IsRunning", isRunning);
		if (isAiming && !isJogging && Input.GetKeyDown(KeyCode.Mouse0)) {
			if (weapon.PlayFireAnimation(this)) {
				anim.SetTrigger("Fire");
			}
		}

		// check if death
		if (this.life < 0) {
			anim.SetTrigger("Death");
			Debug.Log("Should be dead now!");
		} else {
			Debug.Log("Still Alive!");
			Debug.Log(this.life);
		}
	}



	void OnDrawGizmos()
	{

		Gizmos.DrawIcon(this.transform.position, "Spawn.tif");
	}
	// OnDrawGizmos


}
// public class Player
