
using UnityEngine;
using System.Collections;

public class ImageMove : MonoBehaviour {

	public Transform m_RecTransformTarget;

	private byte m_Key;
	private bool m_Moving;

	private float m_CurTime;
	private float m_AnimDuration;

	private float m_InitialTime;

	private Vector2 m_From;
	private Vector2 m_To;

	void Start()
	{
		if(m_RecTransformTarget==null)
			m_RecTransformTarget=transform;
	}

	public void MoveTo(Vector2 _from,Vector2 _to,float _duration=1)
	{
		m_Key++;

		if(_duration<0)
		{
			Debug.Log("Animation cannot be played with a negative parameter as duration");
			_duration=1;
		}
		//initialize animation values
		m_AnimDuration=_duration;
		m_From=_from;
		m_To=_to;
		m_CurTime=0;

		//start the animating loop
		StartCoroutine(AnimTick(m_Key));
	}

	IEnumerator AnimTick(byte _curKey)
	{
		m_RecTransformTarget.localPosition=Vector2.Lerp(m_From,m_To,m_CurTime);

		do
		{
			m_CurTime=Mathf.Clamp(m_CurTime+=Time.deltaTime,0,m_AnimDuration);
			m_RecTransformTarget.localPosition=Vector2.Lerp(m_From,m_To,m_CurTime/m_AnimDuration);
			yield return 0;
		}while(_curKey==m_Key&&m_CurTime<m_AnimDuration);
	}

}