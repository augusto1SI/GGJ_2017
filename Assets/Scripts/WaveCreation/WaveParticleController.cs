using UnityEngine;
using System.Collections;

public class WaveParticleController : MonoBehaviour
{
    public ParticleSystem[] m_WaveParticles;

    private float m_ETA;
    private float m_Length;
    private bool m_ActiveParticles;
	// Use this for initialization
	public void Initialize (float _animLength)
    {
        m_Length = _animLength;
        m_ActiveParticles = false;

        for(int i = 0; i < m_WaveParticles.Length; ++i)
            m_WaveParticles[i].Stop();
	}
	
    public void ActivateParticles()
    {
        if (m_ActiveParticles) return;
        m_ActiveParticles = true;

        m_ETA = 0f;

        for (int i = 0; i < m_WaveParticles.Length; ++i)
            m_WaveParticles[i].Play();
    }

	void Update ()
    {
        if (m_ActiveParticles) UpdateParticles();
	}

    void UpdateParticles()
    {
        m_ETA += Time.deltaTime;

        if (m_ETA < m_Length) return;

        for (int i = 0; i < m_WaveParticles.Length; ++i)
            m_WaveParticles[i].Stop();

        m_ActiveParticles = false;
    }
}
