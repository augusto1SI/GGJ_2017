using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class UnitPlayer : Unit
{

	private CharacterController control;

	public SpriteAnim m_Anim;

	private byte m_MaxFollowers=10;
	private byte m_CurrentFollowers=0;

	private float gravity = 0f;

	private Vector3 lookPosition = Vector3.zero;

	public Texture crosshair;

	public WavePool m_WavePool;

	public ButtonReceiver m_ButtonA;
	public ButtonReceiver m_ButtonB;
	public ButtonReceiver m_ButtonC;

    private Vector3 m_LastMousePosition;
    private float m_DistanceTolerance = 1f;
    private float m_MaxAccel = 25f;

    public Transform m_VisualPlayer;
    private Vector3 m_RotationLeft = new Vector3(-90, -35, 0);
    private Vector3 m_RotationRight = new Vector3(-90, 35, 0);

	public override void Start ()
	{
		base.Start ();

		agent.updatePosition = agent.updateRotation = false;

		m_WavePool=GetComponentInChildren<WavePool>();

		control = GetComponent<CharacterController>();

		m_ButtonA.OnClicked += ClickButtonA;
		m_ButtonB.OnClicked += ClickButtonB;
		m_ButtonC.OnClicked += ClickButtonC;

		if(m_Anim==null)
			m_Anim=GetComponentInChildren<SpriteAnim>();

		m_Anim.Play(0);

		if(!control)
		{
			Debug.LogError(name+" has no CharacterController");
			enabled=false;
			return;
		}

        m_LastMousePosition = transform.position;
	}

	public override void Update()
	{
		//TURN CODE
		transform.Rotate(0f,Input.GetAxis("Mouse X") * m_TurnSpeed * Time.deltaTime, 0f);

        Vector3 move = Vector3.zero;
		//MOVE CODE
        if (Input.touchCount > 0)
        {
            if(Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                m_LastMousePosition = Input.GetTouch(0).position;
                m_LastMousePosition.z = transform.position.z;
                m_LastMousePosition = Camera.main.ScreenToWorldPoint(m_LastMousePosition);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            RaycastHit hit; 
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.gameObject.name=="PhysixSpace")
                {
                    m_LastMousePosition = Input.mousePosition;
                    m_LastMousePosition.z = transform.position.z;
                    m_LastMousePosition = Camera.main.ScreenToWorldPoint(m_LastMousePosition);
                }
            }
            //m_LastMousePosition.y = transform.position.y;
        }

        move = Vector3.Normalize(m_LastMousePosition - transform.position);

        m_VisualPlayer.eulerAngles = Vector3.Lerp(m_RotationLeft, m_RotationRight, (move.x + 1) * 0.5f);
		//Vector3 move=new Vector3(Input.GetAxis("Horizontal"),0f,Input.GetAxis("Vertical"));

		//move.Normalize();

		move*=m_MaxAccel;

		move=transform.TransformDirection(move);

		//jUMP AND GRAVITY CODE
		if(!control.isGrounded)
		{
			gravity += Physics.gravity.y * Time.deltaTime;
		}

		move.y = gravity;

		control.Move(move *Time.deltaTime);

		base.Update();
	}

	/*
	void OnGUI()
	{
		if(crosshair)
		{
			GUI.DrawTexture(new Rect(Screen.width * 0.5f - (crosshair.width *0.5f), Screen.height * 0.5f -(crosshair.height * 0.5f), crosshair.width, crosshair.height), crosshair);
		}
	}
	*/

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
