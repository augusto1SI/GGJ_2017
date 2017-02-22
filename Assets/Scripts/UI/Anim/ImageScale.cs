
using UnityEngine;
using System.Collections;

public class ImageScale : MonoBehaviour {

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

	public void ScaleTo(Vector2 _to,float _duration=1)
	{
		m_Key++;

		//initialize animation values
		m_AnimDuration=Mathf.Clamp(_duration,0.1f,100.0f);
		m_From=m_RecTransformTarget.localScale;
		m_To=_to;
		m_CurTime=0;

		//start the animating loop

		StartCoroutine(AnimTick(m_Key));
	}

	public void ScaleTo(Vector2 _from,Vector2 _to,float _duration=1)
	{
		m_Key++;

		//initialize animation values
		m_AnimDuration=Mathf.Clamp(_duration,0.1f,100.0f);
		m_From=_from;
		m_To=_to;
		m_CurTime=0;

		//start the animating loop
		StartCoroutine(AnimTick(m_Key));
	}

	IEnumerator AnimTick(byte _curKey)
	{
		m_RecTransformTarget.localScale=Vector2.Lerp(m_From,m_To,m_CurTime);
		m_InitialTime=Time.time;
		do
		{
			m_CurTime=Mathf.Clamp(Time.time-m_InitialTime,0,m_AnimDuration);
			m_RecTransformTarget.localScale=Vector2.Lerp(m_From,m_To,m_CurTime/m_AnimDuration);
			yield return 0;
		}while(_curKey==m_Key&&m_CurTime<m_AnimDuration);
	}

}