using UnityEngine;
using System.Collections;


public class ButtonOrbit : MonoBehaviour {

	public SpriteRenderer[] m_BtnRenderers;
	public Transform m_Pivot;
	public Vector3 m_Direction;

	[SerializeField]
	private bool m_Orbiting=false;
    private float m_Speed = 10f;

	public bool m_ShowLog = false; 

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

		if(_visible)
			HighlightFirstIcon();
	}

	void Update()
	{
		if(m_Orbiting)
		{
			m_Pivot.Rotate(m_Direction*Time.deltaTime*m_Speed,Space.Self);
			for(int i=0;i<m_BtnRenderers.Length;i++)
			{
				m_BtnRenderers[i].transform.up = Vector3.forward;
			}
		}
	}

	public void SetIcon(GlobalShit.WaveType[] _type)
	{
		if(m_ShowLog)
			Debug.Log (_type.Length);

		if(_type.Length>m_BtnRenderers.Length)
			return;

		for(int i=0;i<m_BtnRenderers.Length;i++)
		{
			if(i<_type.Length)
			{
				m_BtnRenderers[i].enabled=true;
				m_BtnRenderers[i].sprite = ArtDispenser.Instance.GetNoteIcon((int)_type[i]);
				if(m_ShowLog)
					Debug.Log("Iterating on " + i.ToString() + " to show button with type " + _type[i].ToString());
			}
			else
			{
				m_BtnRenderers[i].enabled=false;
				if(m_ShowLog)
					Debug.Log("Iterating on " + i.ToString() + " to not show");
			}
		}
	}

	public void SetIcon(GlobalShit.WaveType _type)
	{
		m_BtnRenderers[0].enabled=true;
		m_BtnRenderers[0].sprite = ArtDispenser.Instance.GetNoteIcon((int)_type);
	}

	public void Orbit(bool _orbit)
	{
		m_Orbiting=_orbit;
	}

	public void IgnoreIconsLowerThan(int _limit)
	{
		for(int i=0;i<_limit;i++)
		{
			m_BtnRenderers[i].enabled=false;				
		}

		HighlightFirstIcon();
	}

	public void HighlightFirstIcon()
	{
		bool firstFound=false;

		for(int i=0;i<m_BtnRenderers.Length;i++)
		{
			if(m_BtnRenderers[i].enabled)
			{
				if(!firstFound)
				{
					firstFound=true;
					m_BtnRenderers[i].color=ArtDispenser.Instance.m_HighlightColor;
				}
				else
				{
					m_BtnRenderers[i].color=ArtDispenser.Instance.m_OpaqueColor;
				}
			}
		}
	}
}
