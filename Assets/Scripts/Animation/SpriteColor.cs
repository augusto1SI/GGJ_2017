﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpriteColor : MonoBehaviour {

	public SpriteRenderer m_Target;

	private byte m_Key;
	private bool m_Moving;

	private float m_CurTime;
	private float m_AnimDuration;

	private float m_InitialTime;

	private Color m_From;
	private Color m_To;

	void Start()
	{
		if(m_Target==null)
			m_Target=GetComponent<SpriteRenderer>();
	}

	public void TintTo(Color _to,float _duration=1)
	{
		m_Key++;

		//initialize animation values
		m_AnimDuration=Mathf.Clamp(_duration,0.1f,100.0f);
		m_From=m_Target.color;
		m_To=_to;
		m_CurTime=0;

		//start the animating loop

		StartCoroutine(AnimTick(m_Key));
	}

	public void TintTo(Color _from,Color _to,float _duration=1)
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
		m_Target.color=Color.Lerp(m_From,m_To,m_CurTime);
		m_InitialTime=Time.time;
		do
		{
			m_CurTime=Mathf.Clamp(Time.time-m_InitialTime,0,m_AnimDuration);
			m_Target.color=Color.Lerp(m_From,m_To,m_CurTime/m_AnimDuration);
			yield return 0;
		}while(_curKey==m_Key&&m_CurTime<m_AnimDuration);
	}

}