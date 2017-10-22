using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour
{
	// tumble 是旋转系数
	public float tumble;
	void Start ()
	{
		// angularVelocity 表示刚体的角速度;  insideUnitSphere 表示单位长度半径球体内的一个随机点(向量)
		// 乘积结果描述了在半径长度为 tumble 的球体中的随机点
		// 由此就可以实现刚体以一个随机的角速度旋转
		GetComponent<Rigidbody> ().angularVelocity = Random.insideUnitSphere * tumble;
	}
}
