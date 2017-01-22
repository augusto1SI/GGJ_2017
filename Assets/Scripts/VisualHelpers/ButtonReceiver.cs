using UnityEngine;
using System.Collections;

public class ButtonReceiver : MonoBehaviour {

	public delegate void ClickAction();
	public event ClickAction OnClicked;

	public void OnClick()
	{
		if(OnClicked!= null)
			OnClicked();
	}
}
