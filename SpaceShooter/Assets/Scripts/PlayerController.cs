using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	// 想在 Inspector 视图显示, 就需要为 Boundary 类添加可序列化的属性 [System.Serializable]
	[System.Serializable]
	public class Boundary
	{
		// 用于管理飞船活动的边界值, XZ 平面
		public float xMin, xMax, zMin, zMax;
	}

	// 速度控制变量
	public float speed;
	public Boundary boundary;
	// 飞船倾斜系数
	public float tilt = 4.0f;

	// 子弹的预制体
	public GameObject shot;
	// 子弹出生位置
	public Transform shotSpwan;
	// 子弹的发射率 间隔时间
	public float fireRate;
	// 下次可以发射的最早时间(发射时间大于此值)
	private float nextFire;

	void Update ()
	{
		if ((Input.GetButton ("Fire1") || Input.GetKey(KeyCode.Space)) && Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			// 生成一枚子弹
			Instantiate (shot, shotSpwan.position, shotSpwan.rotation);  
			// 发射子弹的声音播放
			GetComponent<AudioSource> ().Play ();
	
		}
	
	}

	void FixedUpdate ()
	{
		// 得到水平方向输入
		float moveHorizontal = Input.GetAxis ("Horizontal");
		// 得到垂直方向输入
		float moveVertical = Input.GetAxis ("Vertical");
		// 用上面的水平方向和垂直方向输入创建一个 Vector3 变量, 作为刚体速度, 是一个矢量
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		Rigidbody rb = GetComponent<Rigidbody> ();
		if (rb != null) {
			rb.velocity = movement * speed;
			// Mathf.Clamp 限定刚体的活动范围
			rb.position = new Vector3 (
				Mathf.Clamp (rb.position.x, boundary.xMin, boundary.xMax), 
				0.0f, 
				Mathf.Clamp (rb.position.z, boundary.zMin, boundary.zMax)
			);
			// 飞船左右移动时有一定的倾斜效果, 
			// 绕 Z 轴旋转, 往左运动 X 轴上速度为负值, 旋转的角度为逆时针正值, 所以要乘以一个负系数
			rb.rotation = Quaternion.Euler (0.0f, 0.0f, rb.velocity.x * -tilt);

		}

	}
}
