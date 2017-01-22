using UnityEngine;
using System.Collections;


public class ButtonOrbit : MonoBehaviour {

	public SpriteRenderer[] m_BtnRenderers;
	public Transform m_Pivot;
	public Vector3 m_Direction;


	public Sprite[] tempSprites;

	[SerializeField]
	private bool m_Orbiting=false;

	void Awake()
	{
		m_Pivot=transform;

		m_BtnRenderers=GetComponentsInChildren<SpriteRenderer>();
	}



	public void SetVisible(bool _visible)
	{
		for(int i=0;i<m_BtnRenderers.Length;i++)
		{
			m_BtnRenderers[i].enabled=_visible;
		}
	}

	void Update()
	{
		if(m_Orbiting)
		{
			m_Pivot.Rotate(m_Direction,Space.Self);
			for(int i=0;i<m_BtnRenderers.Length;i++)
			{
				m_BtnRenderers[i].transform.up = Vector3.forward;
			}
		}
	}

	public void SetIcon(GlobalShit.WaveType[] _type)
	{
		if(_type.Length>m_BtnRenderers.Length)
			return;

		for(int i=0;i<m_BtnRenderers.Length;i++)
		{
			if(i<_type.Length)
			{
				m_BtnRenderers[i].enabled=true;
				m_BtnRenderers[i].sprite = tempSprites[(int)_type[i]];
			}
			else
			{
				m_BtnRenderers[i].enabled=false;				
			}
		}
	}

	public void SetIcon(GlobalShit.WaveType _type)
	{
		m_BtnRenderers[0].enabled=true;
		m_BtnRenderers[0].sprite = tempSprites[(int)_type];
	}

	public void Orbit(bool _orbit)
	{
		m_Orbiting=_orbit;
	}
}
