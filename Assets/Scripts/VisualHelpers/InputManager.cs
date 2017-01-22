using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    void Awake()
    {
        AudioManager.Instance.Initialization();
    }

	void Start()
    {
        AudioManager.Instance.PlayMusic(AudioCore.MusicID.BGMusic_GGJ_Track01);
    }
	// Update is called once per frame
	void Update () 
	{

		if(Input.GetButtonDown("Fire1"))
		{

			if(!GameManager.Instance.OnGame)
				GameManager.Instance.StartGame();

			if(GameManager.Instance.OnGameOver)
				GameManager.Instance.RestartGame();


			RaycastHit hit; 
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
			if ( Physics.Raycast (ray,out hit,100.0f)) 
			{
				ButtonReceiver br = hit.transform.GetComponent<ButtonReceiver>();
				if(br!=null)
				{
					br.OnClick();
				}
			}
		}

	}
}
