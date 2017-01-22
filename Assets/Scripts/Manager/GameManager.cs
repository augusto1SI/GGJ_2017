using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	private static GameManager _instance;
	public static GameManager Instance
	{
		get{
			return _instance;
		}
	}

	public SpriteAnim m_Anim;

	void Awake () {
		_instance=this;
	}

	public bool OnGame=false;


	public bool OnGameOver=false;

	public void RestartGame()
	{
		SceneManager.LoadScene(0);
	}

	public void StartGame()
	{
		OnGame=true;
		m_Anim.Play(1);
	}

	public void GameOver()
	{
		OnGame=false;
		OnGameOver=true;
		m_Anim.Play(0);
	}
}
