using UnityEngine;
using System.Collections;

[System.Serializable]
public struct SAnimation
{
	public string name;
	public bool m_IsLoop;
	public int m_Rounds;
	public SAnimGoal[] m_AnimGoals;
	public bool m_PlayAfterFinish;
	public int m_AnimToPlayAfterFinish;
}

[System.Serializable]
public struct SAnimGoal
{
	public Sprite m_Sprite;
	public float m_Duration;
	public bool m_IsKeyFrame;

	//EDITOR
	public bool m_AdvancedSettings;

	//MOVEMENT
	public bool m_UseOffset;
	public bool m_UseRefFrom;
	public bool m_UseRefTo;
	public Vector2 m_OffsetFrom;
	public Vector2 m_OffsetTo;
	public Transform m_TransformFrom;
	public Transform m_TransformTo;
	public AnimationCurve m_MoveCurveX;
	public AnimationCurve m_MoveCurveY;

	//SCALE
	public bool m_UseScale;
	public Vector2 m_ScaleFrom;
	public Vector2 m_ScaleTo;
	public AnimationCurve m_ScaleCurveX;
	public AnimationCurve m_ScaleCurveY;

	//TINT
	public bool m_UseTint;
	public Color m_ColorFrom;
	public Color m_ColorTo;
	public AnimationCurve m_ColorCurve;

	//PARTICLES 
	public bool m_PlayParticles;
	public ParticleSystem m_Particles;
}
