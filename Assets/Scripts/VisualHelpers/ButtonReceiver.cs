using UnityEngine;
using System.Collections;

public class ButtonReceiver : MonoBehaviour {

	public delegate void ClickAction();
	public event ClickAction OnClicked;

#if UNITY_EDITOR
	public delegate void DebugClickAction(GlobalShit.WaveType _wave);
	public event DebugClickAction OnDebugClicked;

	public GlobalShit.WaveType m_WaveType;
#endif

	public void OnClick()
	{
		if(OnClicked!= null)
			OnClicked();
	}

#if UNITY_EDITOR
	public void OnDebugClick()
	{
		if (OnDebugClicked != null)
			OnDebugClicked(m_WaveType);
	}
#endif
}
