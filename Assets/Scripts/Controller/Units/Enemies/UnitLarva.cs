using UnityEngine;
using System.Collections;

public class UnitLarva : UnitAI {

	public GlobalShit.WaveType m_LastReceivedWave;
	public GlobalShit.WaveType m_WaveNeeded;

	public UnitPlayer m_Player;
	public VisualUnitLarva m_Visual;

	private ButtonReceiver m_ClickReceiver;

	private byte m_Level=0;
	private byte m_MaxLevel=3;

	private int m_Uses=5;
	private int m_RemainingUses;

	private float m_FollowDistance=2;
	public  GlobalShit.WaveType[] m_EvolvingWaveSequence;
	public GlobalShit.WaveType m_UseWaveType;

	private int m_SequenceCount=0;

	private float m_EvolvingCooldown=3;

	// Use this for initialization
	public override void Start () {
		base.Start ();

		m_Player=FindObjectOfType<UnitPlayer>();

		if(m_Visual==null)
			m_Visual=GetComponentInChildren<VisualUnitLarva>();

		if (worldMask == -1)
			worldMask =1 << LayerMask.NameToLayer ("World");

		m_ClickReceiver=GetComponentInChildren<ButtonReceiver>();

		m_ClickReceiver.OnClicked += OnClick;

		agent.angularSpeed = m_TurnSpeed;

		StartCoroutine(Think());
	}


	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}



	IEnumerator Think()
	{
		while(true)
		{
			switch(m_State)
			{
				case UnitAIState.Inert:
					yield return StartCoroutine(Inert());
				break;
				case UnitAIState.Alive:
					m_WaveNeeded=GlobalShit.GetRandomBasicWave();
					yield return StartCoroutine(Alive());
				break;
				case UnitAIState.Awake:
					m_WaveNeeded=m_EvolvingWaveSequence[0];
					m_SequenceCount=0;
					yield return StartCoroutine(Awaken());
				break;
				case UnitAIState.Follow:
					m_RemainingUses=m_Uses;
					yield return StartCoroutine(Follow());
				break;
				case UnitAIState.Dead:
					yield return null;
				break;
			}
			yield return null;
		}
	}

	#region STATE_INERT
	IEnumerator Inert()
	{
		m_Visual.SetVisible(false);
		m_Visual.SetOrbitVisible(false);
		while(!GlobalShit.IsABasicWave(m_LastReceivedWave))
		{
			yield return null;
		}
		m_LastReceivedWave=GlobalShit.WaveType.None;
		m_State=UnitAIState.Alive;
		yield return null;
	}
	#endregion

	#region STATE_ALIVE
	IEnumerator Alive()
	{
		m_Visual.SetVisible(true);
		SetLevel(0);
		m_Visual.SetLevelFeedback(m_Level,m_UseWaveType);
		while(m_LastReceivedWave!=m_WaveNeeded)
		{
			yield return null;
		}
		m_LastReceivedWave=GlobalShit.WaveType.None;
		m_WaveNeeded=GlobalShit.WaveType.None;
		m_State=UnitAIState.Awake;
		yield return null;
	}
	#endregion

	#region STATE_AWAKE
	IEnumerator Awaken()
	{
		SetLevel(1);
		m_Visual.SetLevelFeedback(m_Level,m_UseWaveType);
		m_Visual.SetOrbitVisible(true);
		m_Visual.m_Orbit.SetIcon(m_EvolvingWaveSequence);
		while(!IsMaxLevel())
		{
			if(m_LastReceivedWave==m_WaveNeeded)
			{
				m_LastReceivedWave=GlobalShit.WaveType.None;
				m_SequenceCount++;
				m_Visual.SequenceProgress(m_SequenceCount);
				if(m_SequenceCount==m_EvolvingWaveSequence.Length)
				{
					IncreaseLevel();
					m_Visual.SetLevelFeedback(m_Level,m_UseWaveType);
					m_SequenceCount=0;

					if(IsMaxLevel())
					{
						m_Visual.SetOrbitVisible(false);
					}
					else
					{
						m_WaveNeeded=m_EvolvingWaveSequence[m_SequenceCount];
						m_Visual.m_Orbit.SetIcon(m_EvolvingWaveSequence);
					}

					m_Visual.DisplayCooldown(m_EvolvingCooldown);
					yield return new WaitForSeconds(m_EvolvingCooldown);
				}
				else
				{
					m_WaveNeeded=m_EvolvingWaveSequence[m_SequenceCount];	
				}
			}
			yield return null;
		}
		m_Visual.SetOrbitVisible(false);
		m_State=UnitAIState.Follow;
		yield return null;
	}

	void SetLevel(byte _level)
	{
		m_Level=_level;
	}

	bool IsMaxLevel()
	{
		return m_Level==m_MaxLevel;
	}

	void IncreaseLevel(byte _amount=1)
	{
		if(!IsMaxLevel())
			m_Level++;
	}
	#endregion

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if(!hit.rigidbody || hit.rigidbody.isKinematic || hit.transform.tag=="Player")
			return;

		Vector3 pushDir = new Vector3 (hit.moveDirection.x, 0, hit.moveDirection.z);

		hit.rigidbody.AddForceAtPosition( pushDir*m_PushForce,hit.point);
	}


	#region STATE_FOLLOW
	IEnumerator Follow()
	{
		agent.speed = m_WalkSpeed;
		agent.updateRotation = false;

		Vector3 lastKnowingTargetPosition = m_Player.transform.position;

		while(m_Uses>0)
		{

			lastKnowingTargetPosition = m_Player.transform.position;

			Vector3 delta = lastKnowingTargetPosition - transform.position;
			Vector3 dir = delta;
			dir.y =0f;
			dir.Normalize();

			float distance = Vector3.Distance(lastKnowingTargetPosition,transform.position);


			if(distance>m_FollowDistance)
			{
				agent.Resume();
				agent.SetDestination(lastKnowingTargetPosition);
				transform.forward = Vector3.RotateTowards(transform.forward,dir,m_TurnSpeed * Time.deltaTime *- Mathf.Deg2Rad,1f);
			}
			else
				agent.Stop();


			yield return null;
		}

		agent.updateRotation = true;
	}
	#endregion

	#region STATE_DEAD
	public void OnClick()
	{
		if(m_State!=UnitAIState.Follow)
			return;

		//TODO: send the player the note here!!
		m_RemainingUses--;
		if(m_RemainingUses==0)
		{
			m_State=UnitAIState.Dead;		
			m_Visual.SetVisible(false);
			//TODO: Inform the player that this larva has been carried by the clown!!
		}
			
	}
	#endregion

}
