using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	
	// Update is called once per frame
	void Update () 
	{

		if(Input.GetButtonDown("Fire1"))
		{
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
