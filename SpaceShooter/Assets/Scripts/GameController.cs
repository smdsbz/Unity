using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	// 小行星数组
	public GameObject[] hazards;
	// 随机生成小行星的位置
	public Vector3 spawnValues;
	// 每一波小行星生成的数量
	public int hazardCount;
	// 每次生成小行星对象后延迟的时间, 单位秒
	public float spawnWait;
	// 表示开始生成小行星对象前等待的时间
	public float startWait;
	// 表示两批小行星阵列间的时间间隔
	public float waveWait;
	// 更新计分 Text 的组件
	public Text scoreText;
	// 保存当前分值
	private int score;
	// 更新 Text 组件的显示
	public Text gameOverText;
	// 游戏是否结束
	private bool gameOver;
	// 更新添加的 Text 组件
	public Text restartText;
	// 是否重新开始游戏, 只有游戏结束时重新开始
	private bool restart;

	void Start ()
	{
		score = 0;
		UpdateScore ();
		gameOverText.text = "";
		gameOver = false;
		restartText.text = "";
		restart = false;
		StartCoroutine (SpawnWave ());
	}

	void Update ()
	{
		if (restart) {
			if (Input.GetKeyDown (KeyCode.R)) {
				Application.LoadLevel (Application.loadedLevel);
			}
		}
	}

	// 一波一波地生成小行星
	IEnumerator SpawnWave ()
	{
		yield return new WaitForSeconds (startWait);

		while (true) {
			if (gameOver) {
				restartText.text = "按[R]键重新开始";
				restart = true;
				break;
			}
			for (int i = 0; i < hazardCount; i++) {
				GameObject hazard = hazards [Random.Range (0, hazards.Length)];
				Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
				Instantiate (hazard, spawnPosition, Quaternion.identity);  // 生成随机的小行星
				yield return new WaitForSeconds (spawnWait);
			}

			yield return new WaitForSeconds (waveWait);
		}
	}

	void UpdateScore ()
	{
		scoreText.text = "得分 : " + score;
	}

	public void AddScore (int newScoreValue)
	{
		score += newScoreValue;
		UpdateScore ();
	}

	public void GameOver ()
	{
		gameOver = true;
		gameOverText.text = "游戏结束";
	}
}
