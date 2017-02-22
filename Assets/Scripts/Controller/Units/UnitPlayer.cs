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
#if UNITY_EDITOR
	public ButtonReceiver[] m_DebugButtons;
#endif

    private Vector3 m_LastTouchPosition;

	private float m_SpeedTimeLerp;
	private float m_SpeedDistanceLerp;
	private float m_SpeedRadiusLerp;
	private float m_FinalSpeedLerp;
	private float m_LerpIgnore;

    public Transform m_VisualPlayer;
    private Vector3 m_RotationLeft = new Vector3(-90, -35, 0);
    private Vector3 m_RotationRight = new Vector3(-90, 35, 0);
	private Vector3 m_FinalMovementSpeedNoDelta;
	private Vector3 m_NormalizedFinalSpeed;

	private Vector3 m_ProbablePosition;
	private float m_PositionDistanceFromCenter;
	private Vector3 m_NewPosition;
	private Vector3 m_RepositionVector;

	private bool m_InTouch;
	private bool m_ValidTouch;

	private Ray m_Ray;
	private RaycastHit m_RayHit;

	public TextMesh m_Text;

	public Movement m_Movement;

	public override void Start ()
	{
		base.Start ();

		agent.updatePosition = agent.updateRotation = false;

		m_WavePool=GetComponentInChildren<WavePool>();

		m_ButtonA.OnClicked += ClickButtonA;
		m_ButtonB.OnClicked += ClickButtonB;
		m_ButtonC.OnClicked += ClickButtonC;

#if UNITY_EDITOR
		for(int i = 0; i < m_DebugButtons.Length; ++i)
		{
			m_DebugButtons[i].m_WaveType = (GlobalShit.WaveType)i+4;
			m_DebugButtons[i].OnDebugClicked += DebugButtonClick;
		}
#endif

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
		m_Text.text = (1.0f / Time.smoothDeltaTime).ToString ();
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
		{
			m_InTouch = true;
			UpdateLastTouchPosition(Input.mousePosition);
		}

		if (Input.GetMouseButton(0))
			UpdateLastTouchPosition(Input.mousePosition);

		if(Input.GetMouseButtonUp(0))
			m_InTouch = false;
#endif
		m_VisualPlayer.eulerAngles = Vector3.Lerp(m_RotationLeft, m_RotationRight, (m_Movement.GetNormalizedXDirection + 1) * 0.5f);
		m_Movement.Move (m_InTouch, m_LastTouchPosition, m_ValidTouch);

		if(MoveCallback != null)
			MoveCallback(m_Movement.GetTransformPosition);

		base.Update();
	}

	void UpdateLastTouchPosition(Vector3 _input)
	{
		m_Ray = Camera.main.ScreenPointToRay(_input);
		if (Physics.Raycast(m_Ray, out m_RayHit, 100.0f))
		{
			m_ValidTouch = m_RayHit.collider.gameObject.name=="PhysixSpace";
		}

		m_LastTouchPosition = _input;
		m_LastTouchPosition = Camera.main.ScreenToWorldPoint(_input);
		m_LastTouchPosition.y = 0;
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

#if UNITY_EDITOR
	void DebugButtonClick(GlobalShit.WaveType _wave)
	{
		ShootWave(_wave);
	}
#endif

	public bool CanIFollowYou()
	{
		return m_CurrentFollowers<m_MaxFollowers;
	}
}
