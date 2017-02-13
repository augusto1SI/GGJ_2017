using UnityEngine;
using System.Collections;

public static class Boundary
{
	private static float m_MaxRadius = 70f;

	public static Vector3 ValidatePosition(Vector3 _pos)
	{
		if(_pos.magnitude > m_MaxRadius)
		{
			return _pos.normalized * m_MaxRadius;
		}
		else
			return _pos;
	}

	public static float GetMaxRadius
	{
		get
		{
			return m_MaxRadius;
		}
	}
}
