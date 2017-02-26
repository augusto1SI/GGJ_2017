using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    void Awake()
    {
        
    }

	void Start()
    {
        AudioManager.Instance.Initialization();
        AudioManager.Instance.PlayMusic(AudioCore.MusicID.BGMusic_GGJ_Track01);
    }
	// Update is called once per frame
	void Update () 
	{
#if UNITY_IOS
		if(Input.touchCount > 0)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Began)
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
#else
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
//#if UNITY_EDITOR
					br.OnDebugClick();
//#endif
				}
			}
		}
#endif

	}
}
