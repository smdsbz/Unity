using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Player : Photon.PunBehaviour, IPunObservable
{


	public int life = 5;
	public readonly int maxLife = 5;
	private demo demoD;
	private Blood blood;

	// Component References
	Transform	playerTransform;
	CharacterController	controller;
	Rigidbody rigidBody;
	Animator	anim;
	Weapon weapon;

	// Camera : Component
	Transform camTransform;
	// third person
	Vector3 camOffset = new Vector3(1f, 4.6f, -2.2f);
	// Vector3 camOffset = new Vector3(0f, 4.5f, 0f); // first person
	static float animFloat_midpoint = 0;

	// Attributes
	float	walkSpeed = 2f;
	float	runSpeed = 4;
	//	float	gravity = 2f;
	// States - for animations
	bool	isJogging = false;
	bool	isRunning = false;
	bool	isAiming = false;

	bool isDead = false;
	bool isGrounded = false;



	void Start()
	{
		
		if (!photonView.isMine) {
			return;
		}
		demoD = GameObject.Find("Scripts").GetComponent<demo>();
		blood = GameObject.Find("BloodCanvas").GetComponent<Blood>();

		demoD.bindPlayer(photonView.owner);

		// Transform : Player
		playerTransform = this.transform;
		// CharCont : Player
		controller = this.GetComponent <CharacterController>();
		rigidBody = this.GetComponent <Rigidbody>();
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

	[PunRPC]
	void MinusLife(int player_viewid)
	{
		Debug.Log("自己的id：" + this.photonView.ownerId + "传来的ID：" + player_viewid);
		
		if (player_viewid == this.photonView.ownerId) {
			life--; 
		}
	}


	void Update()
	{

		if (!photonView.isMine) {
			return;
		}

		if (life < 0 && !isDead) {
			isDead = true;
			anim.SetTrigger("Death");
			demoD.showDiedDialog();
			camTransform.parent = null;
			return;
		}

		this.GroundDetection();

		if (isDead) {
			Debug.Log(this + "is dead!");
			return;
		}
		Contorl();
		Animate();
	}
	// Update


	public void OnGUI() {
		if (!photonView.isMine) {
			return;
		}
		Debug.Log(blood);
		blood.SetProgress((float)life / maxLife);
	}


	bool GroundDetection()
	{
		RaycastHit hitToFloor;
		float distance = 1.3f;
		if (Physics.Raycast(transform.position + Vector3.up, new Vector3(0, -1, 0), out hitToFloor, distance)) {
			Debug.DrawLine(transform.position, hitToFloor.point);
			this.isGrounded = true;
			return true;
		} else {
			this.isGrounded = false;
			return false;
		}
	}



	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {	// self
			stream.SendNext(this.life);
			stream.SendNext(this.isDead);
		} else {	// duplicate
			this.life = (int)stream.ReceiveNext();
			this.isDead = (bool)stream.ReceiveNext();
		}
	}


	void Contorl()
	{
		/*    contorl player movement */

		//Debug.Log("Current velocity " + rigidbody.velocity.magnitude);

		//// Player ////
		// Set Transform.position
		// Set player Transform
		float x = 0f, y = 0f, z = 0f;
		if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
			y += 1.5f;
//			y += 0f;
//			this.rigidbody.velocity += Vector3.up * 1.3f;
		}
//		if (Input.GetKey(KeyCode.LeftControl)) {
//			y -= 1f;
//		}
		if (Input.GetKey(KeyCode.LeftShift)) {
			//y -= gravity * Time.deltaTime;
			z += Input.GetAxisRaw("Vertical") * runSpeed * Time.deltaTime;
			x += Input.GetAxisRaw("Horizontal") * runSpeed * Time.deltaTime;
		} else {
			//y -= gravity * Time.deltaTime;
			z += Input.GetAxisRaw("Vertical") * walkSpeed * Time.deltaTime;
			x += Input.GetAxisRaw("Horizontal") * walkSpeed * Time.deltaTime;
		}
		// assign
		controller.Move(playerTransform.TransformDirection(new Vector3(x, y, z)));

		if (x != 0 || z != 0) {
			isJogging = true;
			if (Input.GetKey(KeyCode.LeftShift)) {
				if (!isRunning) {
					isRunning = true;
				}
			}
		} else {
			isJogging = false;
			if (isRunning) {
				isRunning = false;
			}
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

		// Set aiming status
		if (!isAiming && Input.GetKeyDown(KeyCode.Mouse1)) {
			isAiming = true;
		} else if (isAiming && Input.GetKeyUp(KeyCode.Mouse1)) {
			isAiming = false;
		}

		// Scoping
		if (isAiming && !isJogging && !isRunning && animFloat_midpoint < 1f) {
			animFloat_midpoint += 7f * Time.deltaTime;
			Camera.main.fieldOfView = Mathf.Lerp(80f, 30f, animFloat_midpoint);
		} else if (isAiming && animFloat_midpoint >= 1f) {
			Camera.main.fieldOfView = 30f;
		} else if (!isAiming) {
			animFloat_midpoint = 0f;
			Camera.main.fieldOfView = 80f;
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

	}


	public void CostOtherLife(Player other)
	{
		int other_id = other.photonView.ownerId;
		other.photonView.RPC("MinusLife", PhotonTargets.All, other_id);
	}


	void OnDrawGizmos()
	{

		Gizmos.DrawIcon(this.transform.position, "Spawn.tif");
	}
	// OnDrawGizmos





}
// public class Player
