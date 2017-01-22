using UnityEngine;
using System.Collections;

public class BGRotators : MonoBehaviour
{
    public Transform m_Transform;
    public float m_RotateSpeed;
	
	// Update is called once per frame
	void Update ()
    {
        m_Transform.Rotate(Vector3.forward * Time.deltaTime * m_RotateSpeed);
	}
}
