using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VisualUnitLarva : VisualUnit {

	public SpriteRenderer m_Renderer;
	public ButtonOrbit m_Orbit;
	public Sprite[] tempSprites;
	public Cooldown m_Cooldown;



	void Awake()
	{
		m_Renderer=GetComponent<SpriteRenderer>();
		m_Orbit=GetComponentInChildren<ButtonOrbit>();
	}

	void Start()
	{
		m_Cooldown=CooldownPool.Instance.GetCooldown();
	}

	public void SetLevelFeedback(byte _level,GlobalShit.WaveType _type)
	{
		m_Renderer.sprite = tempSprites[_level];
	}

	public override void SetVisible (bool _visible)
	{
		base.SetVisible (_visible);

		m_Renderer.enabled=_visible;
	}

	public override void SetOrbitVisible (bool _visible)
	{
		base.SetOrbitVisible (_visible);

		m_Orbit.SetVisible(_visible);
		m_Orbit.Orbit(_visible);
	}

	public void DisplayCooldown(float _delay)
	{
		m_Cooldown.DisplayCooldown(_delay,m_Orbit.transform.position);
	}

	public void SequenceProgress(int _progress)
	{
		m_Orbit.IgnoreIconsLowerThan(_progress);
	}
}
