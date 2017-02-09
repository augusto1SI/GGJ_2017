using UnityEngine;
using System.Collections;

public class UnitPlayer : Unit
{
	public delegate void MoveChange(Vector3 _pos);
	public event MoveChange MoveCallback;

	public SpriteAnim m_Anim;

	private byte m_MaxFollowers=10;
	private byte m_CurrentFollowers=0;

	public WavePool m_WavePool;

	public ButtonReceiver m_ButtonA;
	public ButtonReceiver m_ButtonB;
	public ButtonReceiver m_ButtonC;

    private Vector3 m_LastTouchPosition;

	private float m_SpeedIncrement = 0.5f; //1 over Time to reach max speed = this value (for Lerp purposes)
	private float m_SpeedDecrement = 0.7f; //1 over Time to reach max speed = this value (for Lerp purposes)

	private float m_DistanceIgnore = 1f;
	private float m_MinDistance = 3f;
	private float m_MaxDistance = 6f;
	private float m_CurrentDistance;

	private float m_MaxSpeedAtMinDistance = 3f;
	private float m_MaxSpeedAtMaxDistance = 8f;
	private float m_SpeedTimeLerp;
	private float m_SpeedDistanceLerp;
	private float m_SpeedRadiusLerp;
	private float m_FinalSpeedLerp;
	private float m_LerpIgnore;

    public Transform m_VisualPlayer;
    private Vector3 m_RotationLeft = new Vector3(-90, -35, 0);
    private Vector3 m_RotationRight = new Vector3(-90, 35, 0);
	private Vector3 m_DifferenceVector;
	private Vector3 m_MovementDirection;
	private Vector3 m_FinalMovementSpeed;
	private Vector3 m_FinalMovementSpeedNoDelta;
	private Vector3 m_NormalizedFinalSpeed;
	private Vector3 m_TempPosition;

	private Vector3 m_ProbablePosition;
	private float m_MaxRadius = 70f;
	private float m_MinRadius = 60f;
	private float m_PositionDistanceFromCenter;
	private Vector3 m_NewPosition;
	private Vector3 m_RepositionVector;

	private bool m_InTouch;
	private bool m_PlayStopParticles = false;

	private float m_PlayStopParticlesTolerance = 0.3f;

	private Ray m_Ray;
	private RaycastHit m_RayHit;

	public TextMesh m_Text;

	public ParticleSystem m_IdleParticles;
	public ParticleSystem m_ParticleAtStop;
	public ParticleSystem m_VariableTrail;
	private float m_TrailParticlesMaxSize;
	private Color m_TrailColor;

	public override void Start ()
	{
		base.Start ();
	
		m_TrailColor = m_VariableTrail.startColor;
		m_TrailParticlesMaxSize = m_VariableTrail.startSize;
		m_TrailColor.a = 0f;

		agent.updatePosition = agent.updateRotation = false;

		m_WavePool=GetComponentInChildren<WavePool>();

		m_ButtonA.OnClicked += ClickButtonA;
		m_ButtonB.OnClicked += ClickButtonB;
		m_ButtonC.OnClicked += ClickButtonC;

		if(m_Anim==null)
			m_Anim=GetComponentInChildren<SpriteAnim>();

		m_Anim.Play(0);

        m_LastTouchPosition = Vector3.zero;
		m_InTouch = false;
		m_IdleParticles.Play ();
	}

	/*GUIStyle _style = new GUIStyle();

	void OnGUI()
	{
		_style.fontSize = 70;
		GUI.Label (new Rect (0, 0, 300, 300), (1.0f / Time.smoothDeltaTime).ToString ());//, _style);
	}*/

	public override void Update()
	{
		//TURN CODE
		transform.Rotate(0f,Input.GetAxis("Mouse X") * m_TurnSpeed * Time.deltaTime, 0f);
		m_Text.text = (1.0f / Time.smoothDeltaTime).ToString ();

		m_MovementDirection = Vector3.zero;
		//MOVE CODE
#if UNITY_IOS
		if(Input.touchCount > 0)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Began)
				m_InTouch = true;
			
			if(Input.GetTouch(0).phase == TouchPhase.Moved)
				UpdateLastTouchPosition(Input.GetTouch(0).position);
			
			if(Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
				m_InTouch = false;
		}
#else

        if (Input.GetMouseButtonDown(0))
			m_InTouch = true;

		if (Input.GetMouseButton(0))
			UpdateLastTouchPosition(Input.mousePosition);

		if(Input.GetMouseButtonUp(0))
			m_InTouch = false;
#endif
		m_DifferenceVector = m_LastTouchPosition - m_TempPosition;
		m_CurrentDistance = Vector3.Magnitude (m_DifferenceVector);
		m_MovementDirection = Vector3.Normalize (m_DifferenceVector);
		
		m_SpeedDistanceLerp = (Mathf.Max(m_MinDistance, Mathf.Min(m_MaxDistance, m_CurrentDistance)) - m_MinDistance)/(m_MaxDistance - m_MinDistance);
		m_SpeedRadiusLerp = 1f - Mathf.Min(0.9f, ((Mathf.Max(m_MinRadius, Mathf.Min(m_MaxRadius, m_TempPosition.magnitude)) - m_MinRadius)/(m_MaxRadius - m_MinRadius)));
		m_LerpIgnore = (Mathf.Max(m_DistanceIgnore, Mathf.Min(m_MinDistance, m_CurrentDistance)) - m_DistanceIgnore)/(m_MinDistance - m_DistanceIgnore);

		if(m_InTouch && m_LerpIgnore >= 1f) IncrementSpeed();
		else DecrementSpeed();

		if(m_SpeedTimeLerp > 0)
		{
			m_TempPosition = transform.position;
			m_TempPosition.y = 0f;

			m_FinalMovementSpeedNoDelta = m_MovementDirection * Mathf.Lerp (m_MaxSpeedAtMinDistance, m_MaxSpeedAtMaxDistance, m_SpeedDistanceLerp) * m_SpeedTimeLerp * m_SpeedRadiusLerp * m_LerpIgnore;
			m_FinalSpeedLerp = (Mathf.Max(m_MaxSpeedAtMinDistance, Mathf.Min(m_MaxSpeedAtMaxDistance, m_FinalMovementSpeedNoDelta.magnitude)) - m_MaxSpeedAtMinDistance)/(m_MaxSpeedAtMaxDistance - m_MaxSpeedAtMinDistance);
			m_FinalMovementSpeed = m_FinalMovementSpeedNoDelta * Time.deltaTime;

			m_ProbablePosition = m_TempPosition + m_FinalMovementSpeed;

			if(m_ProbablePosition.magnitude > m_MaxRadius)
			{
				m_NewPosition = m_ProbablePosition.normalized * m_MaxRadius;
				m_FinalMovementSpeed = (m_NewPosition - m_TempPosition);

				transform.position = m_NewPosition;
			}
			else
				transform.position += m_FinalMovementSpeed;

			m_NormalizedFinalSpeed = Vector3.Normalize(m_FinalMovementSpeed);

			m_VisualPlayer.eulerAngles = Vector3.Lerp(m_RotationLeft, m_RotationRight, ((m_NormalizedFinalSpeed.x * m_SpeedTimeLerp * m_SpeedDistanceLerp) + 1) * 0.5f);
			agent.SetDestination(m_VisualPlayer.position);

			m_VariableTrail.startSize = m_TrailParticlesMaxSize * m_FinalSpeedLerp;
			m_TrailColor.a = m_FinalSpeedLerp;
			m_VariableTrail.startColor = m_TrailColor;

			if(MoveCallback != null)
				MoveCallback(transform.position);
		}
		base.Update();
	}

	void IncrementSpeed()
	{
		if(m_IdleParticles.isPlaying) m_IdleParticles.Stop();

		if(m_SpeedTimeLerp >= 1f) return;

		m_SpeedTimeLerp += Time.deltaTime * m_SpeedIncrement;

		if(m_SpeedTimeLerp > m_PlayStopParticlesTolerance) m_PlayStopParticles = true;

		if(m_SpeedTimeLerp >= 1f) m_SpeedTimeLerp = 1f;
	}

	void DecrementSpeed()
	{
		if(m_SpeedTimeLerp <= 0f) return;
		
		m_SpeedTimeLerp -= Time.deltaTime * m_SpeedDecrement;

		if(m_SpeedTimeLerp <= m_PlayStopParticlesTolerance && m_PlayStopParticles) { m_ParticleAtStop.Play(); m_PlayStopParticles = false; }

		if(m_SpeedTimeLerp <= 0f)
		{
			m_SpeedTimeLerp = 0f;
			if(!m_IdleParticles.isPlaying) m_IdleParticles.Play();
		}
	}

	void UpdateLastTouchPosition(Vector3 _input)
	{
		m_Ray = Camera.main.ScreenPointToRay(_input);
		if (Physics.Raycast(m_Ray, out m_RayHit, 100.0f))
		{
			if (m_RayHit.collider.gameObject.name=="PhysixSpace")
			{
				m_LastTouchPosition = _input;
				m_LastTouchPosition = Camera.main.ScreenToWorldPoint(_input);
				m_LastTouchPosition.y = 0;
			}
		}
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if(!hit.rigidbody || hit.rigidbody.isKinematic)
			return;

		Vector3 pushDir=new Vector3(hit.moveDirection.x, 0,hit.moveDirection.z);

		hit.rigidbody.AddForceAtPosition(pushDir * m_PushForce, hit.point);
	}

	public void ShootWave(GlobalShit.WaveType _type)
	{
		m_WavePool.DoWaveOfType(((int)_type));
	}

	void ClickButtonA()
	{
		ShootWave(GlobalShit.WaveType.TypeA);
		m_Anim.Play(1);
	}

	void ClickButtonB()
	{
		ShootWave(GlobalShit.WaveType.TypeB);
		m_Anim.Play(2);
	}

	void ClickButtonC()
	{
		ShootWave(GlobalShit.WaveType.TypeC);
		m_Anim.Play(3);
	}

	public bool CanIFollowYou()
	{
		return m_CurrentFollowers<m_MaxFollowers;
	}
}
