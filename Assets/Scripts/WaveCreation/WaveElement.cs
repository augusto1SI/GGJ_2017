﻿using UnityEngine;
using System.Collections;

public class WaveElement : MonoBehaviour
{
    public int m_WaveTypeID;
    public Animation m_WaveAnimation;
    public WaveTriggerController m_WaveTriggerController;

	public bool TryDoWave ()
    {
	    if(m_WaveTriggerController.TryDoWave())
        {
            m_WaveAnimation.Play();
            return true;
        }

        return false;
	}

    void OnTriggerEnter(Collider _col)
    {
        //TODO: Fill this with whatever is necessary
    }
}
