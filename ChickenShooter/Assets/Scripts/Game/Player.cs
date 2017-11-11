using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {

	public Transform target;//要跟随的目标
	public float distance=0f;//摄像机离目标的距离
	public float height=0.0f;//摄像机离目标的高度
	public float heightDamping=0f;//水平跟随平滑系数
	public float rotationDamping=0f;//跟随高度变化系数
	public float refRotation=0f;
	public float refHeight=0f;

	// Exposed Parameters
	public int life = 5;

	// Component References
	Transform playerTransform;
	CharacterController controller;
	Animator anim;

	// Camera : Component
	Transform camTransform;
	// float camHeight = 1.4f;
	Vector3 camOffset = new Vector3(1f, 4.6f, -2.2f);
	// Vector3 camOffset = new Vector3(0f, 4.5f, 0f);

	// Attributes
	float 	walkSpeed = 4f;
	float  	runSpeed = 14;
	float		gravity = 2f;
	// States
	bool		isJogging = false;
	bool 		isRunning = false;
	bool 		isAiming = false;



	void Start () {
		/*	set references */
		if (!isLocalPlayer) {
			return;
		}
		// Transform : Player
		playerTransform = this.transform;
		// CharCont : Player
		controller = this.GetComponent <CharacterController> ();
		// Transform : Camera
		camTransform = Camera.main.transform;
		// Animator
		anim = GetComponent <Animator> ();

		// Set Camera start position
		// set rotation
		camTransform.parent = playerTransform;
		camTransform.rotation = playerTransform.rotation;
		// set position
		camTransform.position = playerTransform.position;
		camTransform.localPosition += camOffset;

		// Lock mouse
		Cursor.lockState = CursorLockMode.Locked;
	}	// Start


	void Update () {

		if (life <= 0 || !isLocalPlayer) { return; }
		Contorl();
		Animate();
		FireBullet();	// DEBUG
	}	// Update


	/*
	void cameraUpdate (Transform target, Transform transform) {
			if(target){
				float targetRotationAngle=target.eulerAngles.y;//目标的朝向
				float targetHeight=target.position.y+height;//得到跟随的高度

				float cameraRotationAngle=transform.eulerAngles.y;//摄像机的朝向
				float cameraHeight=transform.position.y;//摄像机的高度

				cameraRotationAngle=Mathf.SmoothDampAngle(cameraRotationAngle,targetRotationAngle,ref refRotation,rotationDamping);//从摄像机目前的角度变换到目标的角度

				cameraHeight=Mathf.SmoothDamp(cameraHeight,targetHeight,ref refHeight,heightDamping);//从摄像机目前的高度平滑变换到目标的高度

				Quaternion cameraRotation=Quaternion.Euler(0,cameraRotationAngle,0);//每帧在Y轴上旋转摄像机 旋转的角度为cameraRotationAngle 因为上面的代码已经得到了每帧要从摄像机当前的角度变换到目标角度cameraRotationAngle

				//下面几句代码主要设置摄像机的位置
				transform.position=target.position;
				transform.position-= cameraRotation * Vector3.forward * distance;
				transform.position=new Vector3(transform.position.x, cameraHeight,transform.position.z);

				//使摄像机一直朝着目标方向
				transform.LookAt(target);
			}
	}
	*/


	void Contorl() {
		/*	contorl player movement */

		//// Player ////
		// Set Transform.position
		// Set player Transform
		float x = 0f, y = 0f, z = 0f;
		 if (Input.GetKey(KeyCode.Space)) { y += 1f;}
		 if (Input.GetKey(KeyCode.LeftControl)) { y -= 1f; }
		if (Input.GetKey(KeyCode.LeftShift)) {
			if (!isRunning) { isRunning = true; }
			y -= gravity * Time.deltaTime;
			z += Input.GetAxisRaw("Vertical") * runSpeed * Time.deltaTime;
			x += Input.GetAxisRaw("Horizontal") * runSpeed * Time.deltaTime;
		} else {
			if (isRunning) { isRunning = false; }
			y -= gravity * Time.deltaTime;
			z += Input.GetAxisRaw("Vertical") * walkSpeed * Time.deltaTime;
			x += Input.GetAxisRaw("Horizontal") * walkSpeed * Time.deltaTime;
		}
		// assign
		controller.Move(
				playerTransform.TransformDirection(new Vector3(x, y, z)));

		if (x!=0 || z!=0) { isJogging = true; }
		else { isJogging = false; }

		Vector3 newPlayerEuler = playerTransform.eulerAngles;
		newPlayerEuler.y += Input.GetAxis("Mouse X");
		playerTransform.eulerAngles = newPlayerEuler;

		//// Camera ////
		// set rotation
		// camTransform.rotation = playerTransform.rotation;
		// set position
		camTransform.position = playerTransform.position;
		camTransform.localPosition += camOffset;
		camTransform.eulerAngles = new Vector3(
				camTransform.localEulerAngles.x - Input.GetAxis("Mouse Y"),
				playerTransform.eulerAngles.y - 0f, // character not holding firmly
				0f // playerTransform.EulerAngles.z
		);


		//cameraUpdate (playerTransform, camTransform);

		// Mouse Click
		if (!isAiming && Input.GetKeyDown(KeyCode.Mouse1)) { isAiming = true; }
		else if (isAiming && Input.GetKeyUp(KeyCode.Mouse1)) { isAiming = false; }
	}	// def Contorl



	void Animate() {

		anim.SetBool("IsJogging", isJogging);
		anim.SetBool("IsAiming", isAiming);
		anim.SetBool("IsRunning", isRunning);
		if (isAiming && !isJogging && Input.GetKeyDown(KeyCode.Mouse0)) {
			anim.SetTrigger("Fire");
			// FireBullet();
		}
	}



	GameObject FireBullet() {
		/**	call this after a bullet is fired
		Param:	void
		Return:	the <GameObject> that is hit || null
		*/

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 100)) {
			Debug.DrawLine(ray.origin, hit.point);
			Debug.Log(hit.collider.gameObject);
			return hit.collider.gameObject;
		}

		return null;

	}	// FireBullet




	void OnDrawGizmos() {

		Gizmos.DrawIcon(this.transform.position, "Spawn.tif");
	}	// OnDrawGizmos


}	// public class Player
