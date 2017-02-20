using UnityEngine;
using System.Collections;

public class PlayerParticlesController : MonoBehaviour
{
	public Movement m_Movement;

	public ParticleSystem m_IdleParticles;
	public Transform m_TransformParticleDeaccel;
	public ParticleSystem m_ParticleAtDeaccel;
	public ParticleSystem m_VariableTrail;
	private float m_TrailParticlesMaxSize;
	private Color m_TrailColor;

	// Use this for initialization
	void Start ()
	{
		m_TrailColor = m_VariableTrail.startColor;
		m_TrailParticlesMaxSize = m_VariableTrail.startSize;
		m_TrailColor.a = 0f;

		m_IdleParticles.Play ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_TrailColor.a = TestTouchRadii.MinMaxLerp;
		m_VariableTrail.startSize = m_TrailParticlesMaxSize * m_TrailColor.a;
		m_VariableTrail.startColor = m_TrailColor;


		if(m_Movement.Braking)
		{
			m_TransformParticleDeaccel.localRotation = Quaternion.Euler (new Vector3(0f,m_Movement.GetMovementAngle,0f));
			if(!m_ParticleAtDeaccel.isPlaying)
				m_ParticleAtDeaccel.Play();
		}
		else
		{
			if(m_ParticleAtDeaccel.isPlaying)
				m_ParticleAtDeaccel.Stop();
		}
	
		
		if(m_Movement.GetNetMovement < 0.02f)
		{
			if(!m_IdleParticles.isPlaying) m_IdleParticles.Play();
		}
		else
		{
			if(m_IdleParticles.isPlaying) m_IdleParticles.Stop();
		}
	}
}
