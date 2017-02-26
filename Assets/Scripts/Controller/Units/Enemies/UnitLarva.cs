using UnityEngine;
using System.Collections;

public class UnitLarva : UnitAI {

	public GlobalShit.WaveType m_WaveNeeded;

	public UnitPlayer m_Player;
	public VisualUnitLarva m_Visual;

	private ButtonReceiver m_ClickReceiver;

	private byte m_Level=0;
	private byte m_MaxLevel=2;

	public int m_Tier=-1;

	private int m_Uses=5;
	private int m_ElderUses=2;
	private int m_RemainingUses;

	private float m_FollowDistance=2;
	public  GlobalShit.WaveType[] m_EvolvingWaveSequence;
	public GlobalShit.WaveType m_UseWaveType;

	private int m_SequenceCount=0;

	public float m_EvolvingCooldown=15;

	private Vector3 m_InitialPosition;

	private SpriteAnim m_Anim;
	private SpriteAnim m_SpitAnim;
	private int m_SpitAnimIndex;

	public ParticleSystem m_ParticleEvolution;
	public ParticleSystem m_ParticleEvolving;
	public BGRotators m_ParticleRotator;
	private float m_ParticleTimeOffset = 0.1f;

	public ParticleSystem m_StandardLarvaParticles;
	public ParticleSystem m_YLW_EvolvingParticles;
	public ParticleSystem m_RED_EvolvingParticles;
	public ParticleSystem m_BLU_EvolvingParticles;
	public ParticleSystem m_YLW_EvolvedParticles;
	public ParticleSystem m_RED_EvolvedParticles;
	public ParticleSystem m_BLU_EvolvedParticles;
	public ParticleSystem m_TrailParticles;

	public bool m_ComesFromElder = false;
	private bool m_AwokenByElder = false;


	private UnitAIState State
	{
		get{
			return m_State;
		}

		set{
			m_State=value;
		}
	}

	// Use this for initialization
	public override void Start () {
		if (m_ComesFromElder)
			return;

		base.Start ();

		m_Player=FindObjectOfType<UnitPlayer>();

		if(m_Visual==null)
			m_Visual=GetComponentInChildren<VisualUnitLarva>();

		if (worldMask == -1)
			worldMask =1 << LayerMask.NameToLayer ("World");

		m_ClickReceiver=GetComponentInChildren<ButtonReceiver>();

		m_ClickReceiver.OnClicked += OnClick;

		m_Anim = transform.GetChild(0).GetComponent<SpriteAnim>();

		agent.angularSpeed = m_TurnSpeed;

		Vector3 _temp = m_Visual.transform.localPosition;
		_temp.y = 2.5f;
		m_Visual.transform.localPosition = _temp;

		m_InitialPosition=transform.position;

		ManualStartThinkCoroutine();
	}

	public void ManualStart()
	{
		base.Start ();

		m_AwokenByElder = false;

		m_Player=FindObjectOfType<UnitPlayer>();
		
		if(m_Visual==null)
			m_Visual=GetComponentInChildren<VisualUnitLarva>();
		
		if (worldMask == -1)
			worldMask =1 << LayerMask.NameToLayer ("World");
		
		m_ClickReceiver=GetComponentInChildren<ButtonReceiver>();
		
		m_ClickReceiver.OnClicked += OnClick;
		
		m_Anim = transform.GetChild(0).GetComponent<SpriteAnim>();
		m_SpitAnim = gameObject.GetComponent<SpriteAnim>();
		
		agent.angularSpeed = m_TurnSpeed;
		m_Anim.m_Library = ArtDispenser.Instance.GetAnimLibrary(m_UseWaveType);
	}

	public void SetInitialPosition(Vector3 _v)
	{
		m_InitialPosition = _v;
	}

	public void ManualStartThinkCoroutine(UnitAIState _state = UnitAIState.Inert)
	{
		State = _state;
		StartCoroutine(Think());
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update ();
	}

	void PlayStageIdleParticles(int _stage)
	{

		if (_stage == 0)
		{
			m_Anim.m_Particles = m_StandardLarvaParticles;
			return;
		}

		m_TrailParticles.Play();

		switch(m_UseWaveType)
		{
			//YELLOW MINIONS
			case GlobalShit.WaveType.TypeD:
			case GlobalShit.WaveType.TypeG:
			case GlobalShit.WaveType.TypeJ:
				m_Anim.m_Particles = _stage == 1 ? m_YLW_EvolvingParticles : m_YLW_EvolvedParticles;
				m_TrailParticles.Stop();
				break;
			//RED MINIONS
			case GlobalShit.WaveType.TypeE:
			case GlobalShit.WaveType.TypeH:
			case GlobalShit.WaveType.TypeK:
			case GlobalShit.WaveType.TypeM:
			case GlobalShit.WaveType.TypeN:
				m_Anim.m_Particles = _stage == 1 ? m_RED_EvolvingParticles : m_RED_EvolvedParticles;
				break;
			//BLUE MINIONS
			case GlobalShit.WaveType.TypeF:
			case GlobalShit.WaveType.TypeI:
			case GlobalShit.WaveType.TypeL:
			case GlobalShit.WaveType.TypeO:
				m_Anim.m_Particles = _stage == 1 ? m_BLU_EvolvingParticles : m_BLU_EvolvedParticles;
				break;
		}
	}

	IEnumerator Think()
	{
		while(true)
		{
			switch(State)
			{
				case UnitAIState.Inert:
					m_UseWaveType=GlobalShit.GetRandomLarvaWave(m_Tier);
					m_EvolvingWaveSequence=null;
					m_EvolvingWaveSequence=GlobalShit.GetEvolvingLarvaSequence(m_UseWaveType);
					m_Anim.m_Library = ArtDispenser.Instance.GetAnimLibrary(m_UseWaveType);
					yield return StartCoroutine(Inert());
				break;
				case UnitAIState.Alive:
					m_WaveNeeded=GlobalShit.GetRandomBasicWave();
					m_Anim.Play(0);
					PlayStageIdleParticles(0);
                    AudioManager.Instance.PlaySFX(AudioCore.SFXID.CORRECT);
					yield return StartCoroutine(Alive());
				break;
				case UnitAIState.Awake:
					m_WaveNeeded=m_EvolvingWaveSequence[0];
					m_Anim.Play(1);
					PlayStageIdleParticles(1);
					m_SequenceCount=0;
                    AudioManager.Instance.PlaySFX(AudioCore.SFXID.CORRECT);
					yield return StartCoroutine(Awaken());
				break;
				case UnitAIState.Follow:
					m_RemainingUses=m_ComesFromElder?m_ElderUses:m_Uses;
					yield return StartCoroutine(Follow());
				break;
				case UnitAIState.Dead:
					yield return StartCoroutine(Die());
				break;
				case UnitAIState.WaitToBeAwokenByElder:
					yield return StartCoroutine(WaitToBeAwoken());
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
		State=UnitAIState.Alive;
		yield return null;
	}
	#endregion

	#region STATE_ALIVE
	IEnumerator Alive()
	{
		m_Visual.SetVisible(true);
		SetLevel(0);
//		m_Visual.SetLevelFeedback(m_Level,m_UseWaveType);
		while(m_LastReceivedWave!=m_WaveNeeded)
		{
			yield return null;
		}
		m_LastReceivedWave=GlobalShit.WaveType.None;
		m_WaveNeeded=GlobalShit.WaveType.None;
		State=UnitAIState.Awake;
		yield return null;
	}
	#endregion

	#region STATE_AWAKE
	IEnumerator Awaken()
	{
		SetLevel(1);
//		m_Visual.SetLevelFeedback(m_Level,m_UseWaveType);
		m_Visual.SetOrbitVisible(true);
		m_Visual.m_Orbit.SetIcon(m_EvolvingWaveSequence);
		while(!IsMaxLevel())
		{
			if(m_LastReceivedWave==m_WaveNeeded)
            {
				m_LastReceivedWave=GlobalShit.WaveType.None;
				m_SequenceCount++;
				m_Visual.SequenceProgress(m_SequenceCount);
                AudioManager.Instance.PlaySFX(AudioCore.SFXID.CORRECT);
				if(m_SequenceCount==m_EvolvingWaveSequence.Length)
				{
					IncreaseLevel();
	//				m_Visual.SetLevelFeedback(m_Level,m_UseWaveType);
					m_SequenceCount=0;


					m_Visual.SetOrbitVisible(false);

					m_Visual.DisplayCooldown(m_EvolvingCooldown);
					m_ParticleEvolution.Play(true);
					yield return new WaitForSeconds (m_ParticleTimeOffset);
					m_Anim.Play(2);
					m_ParticleEvolving.Play();
					m_ParticleRotator.m_Active = true;
					yield return new WaitForSeconds (m_EvolvingCooldown-m_ParticleTimeOffset);
					m_ParticleRotator.m_Active = true;
					m_ParticleEvolving.Stop();
					m_ParticleEvolution.Play(true);

					if(!IsMaxLevel())
					{
						
						m_WaveNeeded=m_EvolvingWaveSequence[m_SequenceCount];
						m_Visual.m_Orbit.SetIcon(m_EvolvingWaveSequence);
						m_Visual.SequenceProgress(m_SequenceCount);
						m_Visual.m_Orbit.Orbit(true);
					}
					else
					{
						m_Anim.Play(3);
						PlayStageIdleParticles(2);
					}
				}
				else
				{
					m_WaveNeeded=m_EvolvingWaveSequence[m_SequenceCount];	
				}
			}
			yield return null;
		}
		m_Visual.SetOrbitVisible(false);
		State=UnitAIState.Follow;
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

	#region ELDER_MINION
	IEnumerator WaitToBeAwoken()
	{
		m_Visual.SetVisible(false);
		m_Visual.SetOrbitVisible(false);
		while(m_ComesFromElder && !m_AwokenByElder)
		{
			yield return null;
		}
		m_LastReceivedWave=GlobalShit.WaveType.None;
		m_Visual.SetVisible(true);
		m_Anim.Play (0);
		m_SpitAnim.Play(m_SpitAnimIndex);
		m_TrailParticles.Play();
		yield return new WaitForSeconds(m_SpitAnim.GetAnimDuration(m_SpitAnimIndex)); //duration of the spit move/scale animation
		State=UnitAIState.Follow;
		yield return null;
	}
	#endregion

	#region STATE_DEAD
	public void OnClick()
	{
		if(State!=UnitAIState.Follow)
			return;

		m_Player.ShootWave(m_UseWaveType);
		m_RemainingUses--;
		if(m_RemainingUses==0)
		{
			State=UnitAIState.Dead;		
			m_Anim.Play(m_ComesFromElder?1:4);
			//TODO: Inform the player that this larva has been carried by the clown!!
		}
			
	}

	IEnumerator Die()
	{
		agent.Stop();
		m_TrailParticles.Stop();
		yield return new WaitForSeconds(2);
		m_Visual.SetVisible(false);
		transform.position=m_InitialPosition;
		State=m_ComesFromElder ? UnitAIState.WaitToBeAwokenByElder : UnitAIState.Inert;
		m_AwokenByElder = false;
	}
	#endregion

	#region ELDER_SPECIFIC

	public void ProceedToFollow(int _idx)
	{
		if(!m_ComesFromElder) return;
		m_SpitAnimIndex = _idx;
		m_AwokenByElder = true;
	}

	public bool IsActiveMinion
	{
		get
		{
			return State == UnitAIState.Follow || State == UnitAIState.Dead;
		}
	}
	#endregion
}
