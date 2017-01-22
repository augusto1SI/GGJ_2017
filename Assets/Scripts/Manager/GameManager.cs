using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private static GameManager _instance;
	public static GameManager Instance
	{
		get{
			return _instance;
		}
	}

	void Awake () {
		_instance=this;
	}

	public bool OnGame=false;

	public void GameOver()
	{
		
	}
}
