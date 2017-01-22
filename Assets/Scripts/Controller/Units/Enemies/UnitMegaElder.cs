using UnityEngine;
using System.Collections;

public class UnitMegaElder : UnitAI {

	public GlobalShit.WaveType m_WaveNeeded;

	public VisualUnitElder m_Visual;

	public  GlobalShit.WaveType[] m_WakeUpWaveSequence;

	private int m_SequenceCount=0;

    public AudioSource m_AudioAwake;
    public AudioSource m_AudioAffected;

	// Use this for initialization
	public override void Start () {
		base.Start ();

		if(m_Visual==null)
			m_Visual=GetComponentInChildren<VisualUnitElder>();

		if (worldMask == -1)
			worldMask =1 << LayerMask.NameToLayer ("World");

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
                    m_AudioAffected.Play();
					yield return StartCoroutine(Inert());
				break;
				case UnitAIState.Alive:
                    m_AudioAffected.Play();
                    m_AudioAwake.Stop();
					m_WaveNeeded=m_WakeUpWaveSequence[0];
					m_SequenceCount=0;
					yield return StartCoroutine(Alive());
				break;
				case UnitAIState.Awake:
                    m_AudioAffected.Stop();
                    m_AudioAwake.Play();
					GameManager.Instance.GameOver();
					yield return StartCoroutine(Awaken());
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

		while(!GameManager.Instance.OnGame)
		{
			yield return null;
		}
		m_State=UnitAIState.Alive;
		yield return null;
	}
		
	#endregion

	#region STATE_ALIVE
	IEnumerator Alive()
	{
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
		m_Visual.SetOrbitVisible(false);
		while(true)
		{
			yield return null;
		}
		yield return null;
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
