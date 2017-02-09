using UnityEngine;
using System.Collections;

public class BGController : MonoBehaviour
{
    public Material m_Material;
    public float m_CloudHorzSpeed1;
	public float m_CloudHorzSpeed2;
	public float m_CloudVertSpeed1;
	public float m_CloudVertSpeed2;

	private Vector2 m_CloudOffset1 = Vector2.zero;
	private Vector2 m_CloudOffset2 = Vector2.zero;
	
	// Update is called once per frame
	void Update ()
    {
		m_CloudOffset1.x += Time.deltaTime * m_CloudHorzSpeed1;
		m_CloudOffset1.y += Time.deltaTime * m_CloudVertSpeed1;

		m_CloudOffset2.x += Time.deltaTime * m_CloudHorzSpeed2;
		m_CloudOffset2.y += Time.deltaTime * m_CloudVertSpeed2;

		m_CloudOffset1.x %= 1f;
		m_CloudOffset1.y %= 1f;
		m_CloudOffset2.x %= 1f;
		m_CloudOffset2.y %= 1f;

		m_Material.SetFloat("_CloudTexXOffset1", m_CloudOffset1.x);
		m_Material.SetFloat("_CloudTexYOffset1", m_CloudOffset1.y);
		m_Material.SetFloat("_CloudTexXOffset2", m_CloudOffset2.x);
		m_Material.SetFloat("_CloudTexYOffset2", m_CloudOffset2.y);
	}
}
