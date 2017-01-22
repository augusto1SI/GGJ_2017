using UnityEngine;
using System.Collections;

public class Parasite : MonoBehaviour {

	public SpriteRenderer m_Renderer;
	public ButtonOrbit m_Orbit;
	public GlobalShit.WaveType m_Type;
	public SpriteAnim m_Anim;

	public bool m_Alive=false;

	void Awake()
	{
		m_Renderer=GetComponent<SpriteRenderer>();
		m_Orbit=GetComponentInChildren<ButtonOrbit>();
		m_Anim=GetComponentInChildren<SpriteAnim>();


	}

	public void Spawn(GlobalShit.WaveType _type)
	{
		m_Type=_type;
		m_Orbit.SetVisible(true);
		m_Orbit.SetIcon(_type);
		SetVisible(true);
		m_Orbit.Orbit(true);
		m_Alive=true;
		m_Renderer.color=Color.white;
		m_Anim.Play(0);
	}

	void SetVisible(bool _visible)
	{
		m_Renderer.enabled=_visible;
	}

	public void TryRemove(GlobalShit.WaveType _type)
	{
		if(!m_Alive)
			return;

		if(m_Type==_type)
		{
			StartCoroutine(Die());
			m_Orbit.SetVisible(false);
			m_Alive=false;
		}
	}

	IEnumerator Die()
	{
		m_Anim.Play(1);
		yield return new WaitForSeconds(1);
		SetVisible(false);
	}


}
