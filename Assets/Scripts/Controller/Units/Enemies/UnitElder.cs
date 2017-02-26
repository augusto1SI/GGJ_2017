using UnityEngine;
using System.Collections;

public class UnitElder : UnitAI {

	public GlobalShit.WaveType m_WaveNeeded;

	public UnitPlayer m_Player;
	public VisualUnitElder m_Visual;

	private byte m_Level=0;
	private byte m_MaxLevel=3;

	public float m_FollowDistance=2;
	public GlobalShit.WaveType[] m_WakeUpWaveSequence;
	public GlobalShit.WaveType[] m_EvolvingWaveSequence;
	public GlobalShit.WaveType m_UseWaveType;

	private int m_SequenceCount=0;

	public float m_EvolvingCooldown=30;

    public AudioSource m_AudioInert;
	public AudioSource m_AudioAwaken;
    public AudioSource m_AudioActive;
	public AudioSource m_AudioSpit;

	public SpriteAnim m_Anim;

	public ParticleSystem m_IdleParticles;

	public UnitLarva[] m_ElderLarvas;
	private int m_SpitIndex = 0;

	// Use this for initialization
	public override void Start ()
	{
		base.Start ();

		m_Player=FindObjectOfType<UnitPlayer>();

		if(m_Visual==null)
			m_Visual=GetComponentInChildren<VisualUnitElder>();

		if (worldMask == -1)
			worldMask =1 << LayerMask.NameToLayer ("World");

		StartCoroutine(Think());

		m_IdleParticles.Play ();

		for(int i = 0; i < m_ElderLarvas.Length; ++i)
		{
			m_ElderLarvas[i].m_UseWaveType = m_UseWaveType;
			m_ElderLarvas[i].ManualStart();
			m_ElderLarvas[i].transform.SetParent(this.transform.parent);

			Vector3 _temp = m_ElderLarvas[i].m_Visual.transform.localPosition;
			_temp.y = 2.5f;
			m_ElderLarvas[i].m_Visual.transform.localPosition = _temp;
			
			m_ElderLarvas[i].SetInitialPosition(transform.position);
			m_ElderLarvas[i].ManualStartThinkCoroutine(UnitAIState.WaitToBeAwokenByElder);
		}
	}


	// Update is called once per frame
	public override void Update ()
	{
		base.Update ();
	}



	IEnumerator Think()
	{
		while(true)
		{
			switch(m_State)
			{
				case UnitAIState.Inert:
					m_Anim.m_Library = ArtDispenser.Instance.GetElderAnimLibrary(m_UseWaveType-GlobalShit.WaveType.TypeM);
					m_Anim.Play(0);
                    m_AudioInert.Play();
					m_WaveNeeded=m_WakeUpWaveSequence[0];
					m_SequenceCount=0;
					yield return StartCoroutine(Inert());
				break;
				case UnitAIState.Alive:
					m_WaveNeeded=m_EvolvingWaveSequence[0];
					m_SequenceCount=0;
					yield return StartCoroutine(Alive());
				break;
				case UnitAIState.Spit:
					yield return StartCoroutine(Spit());
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
		m_Visual.SetOrbitVisible(true);
		m_Visual.m_Orbit.SetIcon (m_WakeUpWaveSequence);
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
		m_Visual.SetOrbitVisible (false);
		yield return null;
		m_Anim.m_KeyframeCallback += KeyframeCallback;
		m_State=UnitAIState.Morphing;

	}

	void KeyframeCallback()
	{
		m_Anim.m_KeyframeCallback -= KeyframeCallback;
		m_AudioInert.Stop ();
		m_AudioAwaken.Play ();
		m_Anim.Play (1);
		Invoke ("ChangeToAlive", m_Anim.GetAnimDuration (1));
	}

	void ChangeToAlive()
	{
		m_AudioActive.Play ();
		m_State=UnitAIState.Alive;
	}

	#endregion

	#region STATE_ALIVE
	IEnumerator Alive()
	{
		SetLevel(0);
	//	m_Visual.SetLevelFeedback(m_Level,m_UseWaveType);
		while(!HasFreeLarvas)
		{
			yield return null;
		}
		m_Visual.SetOrbitVisible(true);
		m_Visual.m_Orbit.SetIcon(m_EvolvingWaveSequence);
		m_SequenceCount = 0;
		while(m_SequenceCount<m_EvolvingWaveSequence.Length)
		{
			if(m_LastReceivedWave==m_WaveNeeded)
			{
				m_LastReceivedWave=GlobalShit.WaveType.None;
				m_SequenceCount++;
				if(m_SequenceCount<m_EvolvingWaveSequence.Length)
				{
					m_Visual.SequenceProgress(m_SequenceCount);
					m_WaveNeeded=m_EvolvingWaveSequence[m_SequenceCount];	
				}
			}
			yield return null;
		}
		m_State=UnitAIState.Spit;
		yield return null;
	}
	#endregion

	#region STATE_SPIT
	IEnumerator Spit()
	{
		m_Visual.SetOrbitVisible(false);
		m_Anim.Play (3);
		m_AudioSpit.Play();

		int _tempIndex = GetFreeLarva;

		if(_tempIndex != -1)
		{
			m_ElderLarvas[_tempIndex].ProceedToFollow(GetSpitIndex);
		}

		yield return new WaitForSeconds(m_Anim.GetAnimDuration(3));

		m_State=UnitAIState.Alive;
		yield return null;
	}

	int GetFreeLarva
	{
		get
		{
			for(int i = 0; i < m_ElderLarvas.Length; ++i)
			{
				if(!m_ElderLarvas[i].IsActiveMinion)
					return i;
			}

			return -1;
		}
	}

	bool HasFreeLarvas
	{
		get
		{
			foreach(UnitLarva _larva in m_ElderLarvas)
			{
				if(!_larva.IsActiveMinion)
					return true;
			}
			return false;
		}
	}

	int GetSpitIndex
	{
		get
		{
			++m_SpitIndex;
			m_SpitIndex %= 2;
			return m_SpitIndex;
		}
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
}
