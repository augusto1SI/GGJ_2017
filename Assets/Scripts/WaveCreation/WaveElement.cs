﻿using UnityEngine;
using System.Collections;

public class WaveElement : MonoBehaviour
{
    public int m_WaveTypeID;
    public Animation m_WaveAnimation;
    public WaveTriggerController m_WaveTriggerController;
    public WaveParticleController m_WaveParticleController;
	public WaveVisualMeshController m_WaveVMController;

    void Start()
    {
        m_WaveTriggerController.Initialize(m_WaveAnimation.clip.length);
        m_WaveParticleController.Initialize(m_WaveAnimation.clip.length);
		m_WaveVMController.Initialize(m_WaveAnimation.clip.length);
    }

	public bool TryDoWave ()
    {
	    if(m_WaveTriggerController.TryDoWave())
        {
            m_WaveAnimation.Play();
            m_WaveParticleController.ActivateParticles();
			m_WaveVMController.ActivateVisuals();
            return true;
        }

        return false;
	}

    void OnTriggerEnter(Collider _col)
    {
        //TODO: Fill this with whatever is necessary
		if(_col.GetComponent<UnitAI>()!=null)
		{
			_col.GetComponent<UnitAI>().m_LastReceivedWave=(GlobalShit.WaveType)m_WaveTypeID;
		}
    }
}
