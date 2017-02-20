using UnityEngine;
using System.Collections;

public class BGRotators : MonoBehaviour
{
    public Transform m_Transform;
    public float m_RotateSpeed;
	public bool m_Active = false;
	
	void Start()
	{
		m_Active = false;
	}

	void Update ()
    {
		if(!m_Active) return;
        m_Transform.Rotate(Vector3.forward * Time.deltaTime * m_RotateSpeed);
	}
}
