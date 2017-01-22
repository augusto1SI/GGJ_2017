using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
	protected NavMeshAgent agent;

	public float m_WalkSpeed = 2f;
	public float m_RunSpeed = 6f;
	public float m_TurnSpeed = 0f;
	public float m_PushForce = 500f;

	protected Weapon weapon;


	// Use this for initialization
	public virtual void Start ()
	{

		agent = GetComponent<NavMeshAgent> ();
		if (!agent)
		{
			Debug.LogError(name+" has no NavMeshAgent!");
			enabled=false;
			return;
		}

		weapon = GetComponentInChildren<Weapon>();

		if(weapon)
		{
			if(weapon.transform != transform)
			{
				if(weapon.GetComponent<Collider>())
					weapon.GetComponent<Collider>().enabled=false;
				if(weapon.GetComponent<Rigidbody>())
					weapon.GetComponent<Rigidbody>().isKinematic=true;
			}
		}
	}

	// Update is called once per frame
	public virtual void Update ()
	{

	}
}
