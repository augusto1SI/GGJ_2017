using UnityEngine;
using System.Collections;

public class UnitElder : UnitAI {

	public GlobalShit.WaveType m_WaveNeeded;

	public UnitPlayer m_Player;
	public VisualUnitElder m_Visual;

	private ButtonReceiver m_ClickReceiver;

	public Parasite[] m_Parasites;

	private byte m_Level=0;
	private byte m_MaxLevel=3;

	private int m_Uses=5;
	private int m_RemainingUses;

	private float m_InfectionSpreadTime=10;
	private float m_InfectionSpreadETA=0;

	public float m_FollowDistance=2;
	public  GlobalShit.WaveType[] m_WakeUpWaveSequence;
	public  GlobalShit.WaveType[] m_EvolvingWaveSequence;
	public GlobalShit.WaveType m_UseWaveType;

	private int m_SequenceCount=0;

	public float m_EvolvingCooldown=30;

    public AudioSource m_AudioAffected;
    public AudioSource m_AudioAwake;

	private Vector3 m_InitialPosition;

	public SpriteAnim m_Anim;

	public ParticleSystem m_Idle;

	// Use this for initialization
	public override void Start ()
	{
		base.Start ();

		m_Player=FindObjectOfType<UnitPlayer>();

		if(m_Visual==null)
			m_Visual=GetComponentInChildren<VisualUnitElder>();

		if (worldMask == -1)
			worldMask =1 << LayerMask.NameToLayer ("World");

		m_ClickReceiver=GetComponentInChildren<ButtonReceiver>();

		m_Parasites=GetComponentsInChildren<Parasite>();

		m_ClickReceiver.OnClicked += OnClick;

		agent.angularSpeed = m_TurnSpeed;

		StartCoroutine(Think());

		m_InitialPosition=transform.position;

		m_Idle.Play ();
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
					m_Anim.m_Library = ArtDispenser.Instance.GetAnimLibrary(m_UseWaveType);
					m_Anim.Play(0);
                    m_AudioAffected.Play();
					yield return StartCoroutine(Inert());
				break;
				case UnitAIState.Alive:
                    m_AudioAffected.Play();
                    m_AudioAwake.Stop();
					m_WaveNeeded=m_WakeUpWaveSequence[0];
					m_SequenceCount=0;
					m_Anim.Play(1);
					yield return StartCoroutine(Alive());
				break;
				case UnitAIState.Awake:
                    m_AudioAffected.Stop();
                    m_AudioAwake.Play();
					m_WaveNeeded=m_EvolvingWaveSequence[0];
					m_SequenceCount=0;
					yield return StartCoroutine(Awaken());
				break;
				case UnitAIState.Follow:
					m_RemainingUses=m_Uses;
					yield return StartCoroutine(Follow());
				break;
				case UnitAIState.Dead:
					yield return StartCoroutine(Die());
				break;
			}
			yield return null;
		}
	}

	#region STATE_INERT
	IEnumerator Inert()
	{
		gameObject.layer = 0;

		m_Visual.SetVisible(true);
		m_Visual.SetOrbitVisible(false);
		SetupInfection();
		while(StillInfected())
		{
			if(m_LastReceivedWave!=GlobalShit.WaveType.None)
			{
				m_InfectionSpreadETA=0;
				RemoveInfection(m_LastReceivedWave);
				m_LastReceivedWave=GlobalShit.WaveType.None;
			}	
			else
			{
				m_InfectionSpreadETA+=Time.deltaTime;
			}

			if(m_InfectionSpreadETA>=m_InfectionSpreadTime)
			{
				AddInfection();
				m_InfectionSpreadETA=0;
			}
			yield return null;
		}
		m_State=UnitAIState.Alive;
		yield return null;

	}

	void SetupInfection()
	{
		
		for(int i=0;i<m_Parasites.Length;i++)
		{
			m_Parasites[i].Spawn(GlobalShit.GetRandomParasiteWave());	
		}
	}

	bool StillInfected()
	{
		for(int i=0;i<m_Parasites.Length;i++)
		{
			if(m_Parasites[i].m_Alive)
				return true;
		}
		return false;
	}

	void RemoveInfection(GlobalShit.WaveType _type)
	{
		for(int i=0;i<m_Parasites.Length;i++)
		{
			if(m_Parasites[i].m_Alive)
				m_Parasites[i].TryRemove(_type);
		}
	}

	void AddInfection()
	{
		for(int i=0;i<m_Parasites.Length;i++)
		{
			if(!m_Parasites[i].m_Alive)
			{
				m_Parasites[i].Spawn(GlobalShit.GetRandomParasiteWave());
				return;
			}
		}
	}

	#endregion

	#region STATE_ALIVE
	IEnumerator Alive()
	{
		SetLevel(0);
	//	m_Visual.SetLevelFeedback(m_Level,m_UseWaveType);
		m_Visual.SetOrbitVisible(true);
		m_Visual.m_Orbit.SetIcon(m_WakeUpWaveSequence);
		while(m_SequenceCount<m_WakeUpWaveSequence.Length)
		{
			if(m_LastReceivedWave==m_WaveNeeded)
			{
				m_LastReceivedWave=GlobalShit.WaveType.None;
				m_SequenceCount++;
				if(m_SequenceCount<m_WakeUpWaveSequence.Length)
				{
					m_Visual.SequenceProgress(m_SequenceCount);
					m_WaveNeeded=m_WakeUpWaveSequence[m_SequenceCount];	
				}
			}
			yield return null;
		}
		m_State=UnitAIState.Awake;
		yield return null;
	}
	#endregion

	#region STATE_AWAKE
	IEnumerator Awaken()
	{
		SetLevel(1);
	//	m_Visual.SetLevelFeedback(m_Level,m_UseWaveType);
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
				//	m_Visual.SetLevelFeedback(m_Level,m_UseWaveType);
					m_SequenceCount=0;

					if(IsMaxLevel())
					{
						m_Anim.Play(2);
						m_Visual.SetOrbitVisible(false);
					}
					else
					{
						m_WaveNeeded=m_EvolvingWaveSequence[m_SequenceCount];
						m_Visual.m_Orbit.SetIcon(m_EvolvingWaveSequence);
						m_Visual.SequenceProgress(m_SequenceCount);
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

		gameObject.layer = 9;

		Vector3 lastKnowingTargetPosition = m_Player.transform.position;

		while(m_RemainingUses>0)
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

		m_Player.ShootWave(m_UseWaveType);
		m_RemainingUses--;
		if(m_RemainingUses==0)
		{
			m_State=UnitAIState.Dead;	
			m_Anim.Play(3);
			//TODO: Inform the player that this larva has been carried by the clown!!
		}
			
	}

	IEnumerator Die()
	{
		agent.Stop();
		yield return new WaitForSeconds(2);
		m_Visual.SetVisible(false);
		transform.position=m_InitialPosition;
		m_State=UnitAIState.Inert;
	}
	#endregion

}
