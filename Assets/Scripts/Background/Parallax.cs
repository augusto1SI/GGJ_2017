using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
	public UnitPlayer m_Player;
	public float m_Multiplier;
	private float m_StartHeight;
	private Vector3 m_Pos;
	// Use this for initialization
	void Start ()
	{
		m_StartHeight = transform.position.y;
		m_Player.MoveCallback += UpdateParallax;
	}
	
	// Update is called once per frame
	void UpdateParallax (Vector3 _pos)
	{
		m_Pos = _pos * m_Multiplier;
		m_Pos.y = m_StartHeight;
		transform.position = m_Pos;
	}

	void OnDisable()
	{
		m_Player.MoveCallback -= UpdateParallax;
	}
}
