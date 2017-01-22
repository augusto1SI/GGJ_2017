using UnityEngine;
using System.Collections;

public class WaveTriggerController : MonoBehaviour
{
    public SphereCollider m_Collider;
    public float m_MinSize;
    public float m_MaxSize;

    private float m_SizeETA;
    private float m_TimeMultiplier;
    private bool m_IsDoingAnimation;
	// Use this for initialization
	public void Initialize (float _animLength)
    {
        m_IsDoingAnimation = false;
        m_TimeMultiplier = 1f / _animLength;

        m_Collider.enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!m_IsDoingAnimation) return;

        m_SizeETA += Time.deltaTime * m_TimeMultiplier;

        if(m_SizeETA >= 1f)
        {
            m_SizeETA = 0f;
            m_IsDoingAnimation = false;
            m_Collider.enabled = false;
        }

        m_Collider.radius = Mathf.Lerp(m_MinSize, m_MaxSize, m_SizeETA);
	}

    public bool TryDoWave()
    {
        if (m_IsDoingAnimation) return false;

        m_IsDoingAnimation = true;
        m_SizeETA = 0f;

        m_Collider.enabled = true;
        m_Collider.radius = m_MinSize;

        return true;
    }
}
