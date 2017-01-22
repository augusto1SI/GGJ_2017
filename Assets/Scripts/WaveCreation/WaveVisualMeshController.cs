using UnityEngine;
using System.Collections;

public class WaveVisualMeshController : MonoBehaviour
{
    public SkinnedMeshRenderer m_WaveRenderer;

    private float m_ETA;
    private float m_Length;
    private bool m_ActiveVisuals;

    private float m_Offset = 0.4f;

	public void Initialize (float _animLength)
    {
        m_ActiveVisuals = false;
        m_Length = _animLength - m_Offset;

        m_WaveRenderer.enabled = false;
	}

    public void ActivateVisuals()
    {
        if (m_ActiveVisuals) return;
        m_ActiveVisuals = true;

        m_ETA = 0f;
        m_WaveRenderer.enabled = true;
    }
	
	void Update ()
    {
        if (m_ActiveVisuals) UpdateVisuals();
	}

    void UpdateVisuals()
    {
        m_ETA += Time.deltaTime;

        if (m_ETA < m_Length) return;

        m_WaveRenderer.enabled = false;
        m_ActiveVisuals = false;
    }
}
