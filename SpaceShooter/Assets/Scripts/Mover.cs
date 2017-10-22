using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{
	// 速度
	public float speed;

	void Start ()
	{
		GetComponent<Rigidbody> ().velocity = transform.forward * speed;
	}

}
