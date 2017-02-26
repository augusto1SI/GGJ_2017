
using UnityEngine;
using System.Collections;

public class SpriteScale : MonoBehaviour {
	
	public Transform m_RecTransformTarget;
	
	private byte m_Key;
	private bool m_Moving;
	
	private float m_CurTime;
	private float m_AnimDuration;
	
	private Vector2 m_From;
	private Vector2 m_To;
	
	private AnimationCurve m_ScaleCurveX;
	private AnimationCurve m_ScaleCurveY;
	
	private Vector2 m_TempScale;
	
	void Start()
	{
		if(m_RecTransformTarget==null)
			m_RecTransformTarget=transform;
	}
	
	public void ScaleTo(Vector2 _from,Vector2 _to,AnimationCurve _curveX,AnimationCurve _curveY,float _duration=1)
	{
		m_Key++;
		
		//initialize animation values
		m_AnimDuration=Mathf.Clamp(_duration,0.1f,100.0f);
		m_From=_from;
		m_To=_to;
		m_ScaleCurveX=_curveX;
		m_ScaleCurveY=_curveY;
		m_CurTime=0;
		
		//start the animating loop
		StartCoroutine(AnimTick(m_Key));
	}
	
	IEnumerator AnimTick(byte _curKey)
	{
		m_RecTransformTarget.localScale=new Vector3(m_From.x,m_RecTransformTarget.lossyScale.y,m_From.y);
		m_TempScale=m_From;
		float ETA=0;
		do
		{
			//increase time for this frame
			m_CurTime=Mathf.Clamp(m_CurTime+=Time.deltaTime,0,m_AnimDuration);
			//get the delta time in range (0-1)
			ETA=m_CurTime/m_AnimDuration;
			//get the proper axis values from the given curves
			m_TempScale.x=Mathf.Lerp(m_From.x,m_To.x,m_ScaleCurveX.Evaluate(ETA));
			m_TempScale.y=Mathf.Lerp(m_From.y,m_To.y,m_ScaleCurveY.Evaluate(ETA));
			m_RecTransformTarget.localScale=new Vector3(m_TempScale.x,m_RecTransformTarget.lossyScale.y,m_TempScale.y);
			yield return 0;
		}while(_curKey==m_Key&&m_CurTime<m_AnimDuration);
	}
	
}