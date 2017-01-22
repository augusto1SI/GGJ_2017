using UnityEngine;
using System.Collections;

public class BGController : MonoBehaviour
{
    public Material m_Material;
    public float m_Speed;

    private Vector2 m_Offset = Vector2.zero;
	
	// Update is called once per frame
	void Update ()
    {
        m_Offset.x += Time.deltaTime * m_Speed;

        if (m_Offset.x >= 1f) m_Offset.x -= 1f;

        m_Material.SetTextureOffset("_CloudTexture", m_Offset);
	}
}
