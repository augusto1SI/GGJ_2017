using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class UnitPlayer : Unit
{

	private CharacterController control;

	private byte m_MaxFollowers=10;
	private byte m_CurrentFollowers=0;

	private float gravity = 0f;

	private Vector3 lookPosition = Vector3.zero;

	public Texture crosshair;

	public WavePool m_WavePool;

	public ButtonReceiver m_ButtonA;
	public ButtonReceiver m_ButtonB;
	public ButtonReceiver m_ButtonC;


	public override void Start ()
	{
		base.Start ();

		agent.updatePosition = agent.updateRotation = false;

		m_WavePool=GetComponentInChildren<WavePool>();

		control = GetComponent<CharacterController>();

		m_ButtonA.OnClicked += ClickButtonA;
		m_ButtonB.OnClicked += ClickButtonB;
		m_ButtonC.OnClicked += ClickButtonC;


		if(!control)
		{
			Debug.LogError(name+" has no CharacterController");
			enabled=false;
			return;
		}
	}

	public override void Update()
	{
		//TURN CODE
		transform.Rotate(0f,Input.GetAxis("Mouse X") * m_TurnSpeed * Time.deltaTime, 0f);

		//MOVE CODE
		Vector3 move=new Vector3(Input.GetAxis("Horizontal"),0f,Input.GetAxis("Vertical"));

		move.Normalize();

		move*=m_WalkSpeed;

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
		Debug.Log("Button A Pressed");
		ShootWave(GlobalShit.WaveType.TypeA);
	}

	void ClickButtonB()
	{
		Debug.Log("Button B Pressed");
		ShootWave(GlobalShit.WaveType.TypeB);
	}

	void ClickButtonC()
	{
		Debug.Log("Button C Pressed");
		ShootWave(GlobalShit.WaveType.TypeC);
	}


	public bool CanIFollowYou()
	{
		return m_CurrentFollowers<m_MaxFollowers;
	}
}
