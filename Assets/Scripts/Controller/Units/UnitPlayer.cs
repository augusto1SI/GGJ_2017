using UnityEngine;
using System.Collections;

public class UnitPlayer : Unit
{
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

	private float m_MinDistance = 2.25f;
	private float m_MaxDistance = 5f;
	private float m_CurrentDistance;

	private float m_MaxSpeedAtMinDistance = 3f;
	private float m_MaxSpeedAtMaxDistance = 8f;

	private float m_SpeedTimeLerp;
	private float m_SpeedDistanceLerp;

    public Transform m_VisualPlayer;
    private Vector3 m_RotationLeft = new Vector3(-90, -35, 0);
    private Vector3 m_RotationRight = new Vector3(-90, 35, 0);
	private Vector3 m_DifferenceVector;
	private Vector3 m_MovementDirection;
	private Vector3 m_FinalMovementSpeed;
	private Vector3 m_NormalizedFinalSpeed;
	private Vector3 m_TempPosition;

	private bool m_InTouch;

	private Ray m_Ray;
	private RaycastHit m_RayHit;

	public override void Start ()
	{
		base.Start ();

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
	}

	public override void Update()
	{
		//TURN CODE
		transform.Rotate(0f,Input.GetAxis("Mouse X") * m_TurnSpeed * Time.deltaTime, 0f);

		m_MovementDirection = Vector3.zero;
		//MOVE CODE
#if UNITY_IOS || UNITY_ANDROID
#else
        if (Input.GetMouseButtonDown(0))
			m_InTouch = true;

		if (Input.GetMouseButton(0))
			UpdateLastTouchPosition(Input.mousePosition);

		if(Input.GetMouseButtonUp(0))
			m_InTouch = false;
#endif

		m_TempPosition = transform.position;
		m_TempPosition.y = 0f;

		m_DifferenceVector = m_LastTouchPosition - m_TempPosition;
		m_CurrentDistance = Vector3.Magnitude (m_DifferenceVector);
		m_MovementDirection = Vector3.Normalize (m_DifferenceVector);

		if(m_InTouch) IncrementSpeed();
		else DecrementSpeed();

		if(m_SpeedTimeLerp > 0)
		{
			m_SpeedDistanceLerp = (Mathf.Max(m_MinDistance, Mathf.Min(m_MaxDistance, m_CurrentDistance)) - m_MinDistance)/(m_MaxDistance - m_MinDistance);

			m_FinalMovementSpeed = m_MovementDirection * Mathf.Lerp (m_MaxSpeedAtMinDistance, m_MaxSpeedAtMaxDistance, m_SpeedDistanceLerp) * m_SpeedTimeLerp * Time.deltaTime;

			transform.position += m_FinalMovementSpeed;

			m_NormalizedFinalSpeed = Vector3.Normalize(m_FinalMovementSpeed);

			m_VisualPlayer.eulerAngles = Vector3.Lerp(m_RotationLeft, m_RotationRight, ((m_NormalizedFinalSpeed.x * m_SpeedTimeLerp * m_SpeedDistanceLerp) + 1) * 0.5f);

			agent.SetDestination(m_VisualPlayer.position);
		}
		base.Update();
	}

	void IncrementSpeed()
	{
		if(m_SpeedTimeLerp >= 1f) return;

		m_SpeedTimeLerp += Time.deltaTime * m_SpeedIncrement;

		if(m_SpeedTimeLerp >= 1f) m_SpeedTimeLerp = 1f;
	}

	void DecrementSpeed()
	{
		if(m_SpeedTimeLerp <= 0f) return;
		
		m_SpeedTimeLerp -= Time.deltaTime * m_SpeedDecrement;
		
		if(m_SpeedTimeLerp <= 0f) m_SpeedTimeLerp = 0f;
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
