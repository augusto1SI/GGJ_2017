using UnityEngine;
using System.Collections;

public static class TestTouchRadii
{
	private static float m_RadiusForMaxSpeed = 4f; //Anything farther from this distance will result in the max speed multiplier
	private static float m_RadiusForMinSpeed = 2f; //This distance will result on the normal speed multiplier (One)
	private static float m_RadiusForIgnore = 1.25f; //Anything less than this distance will make ignore the touch
	private static float m_MaxSpeedMultiplier = 3f;
	private static float m_MaxDampening = 0.3f;
	private static float m_SubtractToObtainMinDampRadius = 10f;

	public static float m_TouchDistance;
	public static float m_PlayerDistance;

	private static float m_IgnoreTouchMultiplier;
	private static float m_DampMultiplier;
	private static float m_MinMaxMultiplier;

	public static void Initialize()
	{
		m_IgnoreTouchMultiplier = 1f / (m_RadiusForMinSpeed - m_RadiusForIgnore);
		m_DampMultiplier = 1f / (Boundary.GetMaxRadius - (Boundary.GetMaxRadius - m_SubtractToObtainMinDampRadius));
		m_MinMaxMultiplier = 1f / (m_RadiusForMaxSpeed - m_RadiusForMinSpeed);
	}

	public static float TestIgnoreTouch
	{
		get
		{
			return (Mathf.Max (m_RadiusForIgnore, Mathf.Min (m_RadiusForMinSpeed, m_TouchDistance)) - m_RadiusForIgnore) * m_IgnoreTouchMultiplier;
		}
	}

	public static float TestMinMaxRadius
	{
		get
		{
			return Mathf.Lerp(1f, m_MaxSpeedMultiplier, MinMaxLerp);
		}
	}

	public static float TestOuterRadiusDamp
	{
		get
		{
			return Mathf.Lerp(1f, m_MaxDampening, (Mathf.Max (Boundary.GetMaxRadius - m_SubtractToObtainMinDampRadius, Mathf.Min (Boundary.GetMaxRadius, m_PlayerDistance)) - (Boundary.GetMaxRadius - m_SubtractToObtainMinDampRadius)) * m_DampMultiplier);
		}
	}

	public static float MinMaxLerp
	{
		get
		{
			return (Mathf.Max (m_RadiusForMinSpeed, Mathf.Min (m_RadiusForMaxSpeed, m_TouchDistance)) - m_RadiusForMinSpeed) * m_MinMaxMultiplier;
		}
	}
}
