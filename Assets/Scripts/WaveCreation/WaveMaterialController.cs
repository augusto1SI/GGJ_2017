using UnityEngine;
using System.Collections;

public class WaveMaterialController : MonoBehaviour
{
    public Material[] m_WaveMaterials;
    public float m_WaveOndulatingSpeed;
    public float m_WaveTextureOffsetSpeed;

    private float m_OndulatingETA;
    private Vector2 m_TextureOffsetETA;

	// Use this for initialization
	void Start ()
    {
        m_OndulatingETA = 0f;
        m_TextureOffsetETA = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_OndulatingETA += Time.deltaTime * m_WaveOndulatingSpeed;
        if (m_OndulatingETA >= 1f) m_OndulatingETA -= 1f;

        m_TextureOffsetETA.y += Time.deltaTime * m_WaveTextureOffsetSpeed;
        if (m_TextureOffsetETA.y >= 1f) m_TextureOffsetETA.y -= 1f;

        foreach(Material _m in m_WaveMaterials)
        {
            _m.SetFloat("_NoiseOffset", m_OndulatingETA);
            _m.SetTextureOffset("_MainWave", m_TextureOffsetETA);
        }
	}
}
