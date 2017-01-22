using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class UnitPlayer : Unit
{

	private CharacterController control;
	private Transform head;
	private float headPitch = 0f;
	public float headPitchMax = 60f;
	public float jumpSpeed = 5f;

	private float gravity = 0f;
	private float fakeZeroGravity = -0.25f;

	private Vector3 lookPosition = Vector3.zero;

	public Texture crosshair;

	public override void Start ()
	{
		base.Start ();

		agent.updatePosition = agent.updateRotation = false;

		control = GetComponent<CharacterController>();

		head = transform.FindChild("Head");

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

//		if(Input.GetButton("Run"))
//			move*=m_RunSpeed;
//		else
			move*=m_WalkSpeed;

		move=transform.TransformDirection(move);

		//jUMP AND GRAVITY CODE
		if(control.isGrounded)
		{
			if(Input.GetButtonDown("Jump"))
				gravity=jumpSpeed;
			else
				gravity=fakeZeroGravity;
		}
		else
		{
			gravity += Physics.gravity.y * Time.deltaTime;
		}

		move.y = gravity;

		control.Move(move *Time.deltaTime);

		//LOOK CODE
		if(head)
		{
			headPitch -= Input.GetAxis("Mouse Y") * m_TurnSpeed * Time.deltaTime;
			headPitch = Mathf.Clamp(headPitch, -headPitchMax, headPitchMax);
			head.rotation = transform.rotation;
			head.Rotate(headPitch, 0f, 0f);
		}

		//AIMING
		RaycastHit hit;
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f,0f));

		lookPosition = Camera.main.transform.position + (Camera.main.transform.forward * Camera.main.farClipPlane);

		if(Physics.Raycast(ray, out hit))
		{
			lookPosition = hit.point;
		}

		if(weapon)
		{
			weapon.transform.LookAt(lookPosition);
			weapon.Fire = Input.GetButton("Fire1");
		}

		base.Update();
	}

	void OnGUI()
	{
		if(crosshair)
		{
			GUI.DrawTexture(new Rect(Screen.width * 0.5f - (crosshair.width *0.5f), Screen.height * 0.5f -(crosshair.height * 0.5f), crosshair.width, crosshair.height), crosshair);
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if(!hit.rigidbody || hit.rigidbody.isKinematic)
			return;

		Vector3 pushDir=new Vector3(hit.moveDirection.x, 0,hit.moveDirection.z);

		hit.rigidbody.AddForceAtPosition(pushDir * m_PushForce, hit.point);
	}

}
