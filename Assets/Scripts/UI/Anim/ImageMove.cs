
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

	private AnimationCurve m_AnimCurveX;
	private AnimationCurve m_AnimCurveY;

	private Vector2 m_TempPos;
	void Start()
	{
		if(m_RecTransformTarget==null)
			m_RecTransformTarget=transform;
	}

	public void MoveTo(Vector2 _from,Vector2 _to,AnimationCurve _curveX,AnimationCurve _curveY,float _duration=1)
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
		m_AnimCurveX=_curveX;
		m_AnimCurveY=_curveY;
		m_CurTime=0;

		//start the animating loop
		StartCoroutine(AnimTick(m_Key));
	}

	IEnumerator AnimTick(byte _curKey)
	{
		//teleport to initial position
		m_RecTransformTarget.localPosition=m_From;
		m_TempPos=m_From;
		float ETA=0;
		do
		{
			//increase time for this frame
			m_CurTime=Mathf.Clamp(m_CurTime+=Time.deltaTime,0,m_AnimDuration);
			//get the delta time in range (0-1)
			ETA=m_CurTime/m_AnimDuration;
			//get the proper axis values from the given curves
			m_TempPos.x=Mathf.Lerp(m_From.x,m_To.x,m_AnimCurveX.Evaluate(ETA));
			m_TempPos.y=Mathf.Lerp(m_From.y,m_To.y,m_AnimCurveY.Evaluate(ETA));
			m_RecTransformTarget.localPosition=m_TempPos;

			yield return 0;
		}while(_curKey==m_Key&&m_CurTime<m_AnimDuration);
	}

}