using UnityEngine;
using System.Collections;

public class DebugSpawnWave : MonoBehaviour
{
    public Animation m_Animation;
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.A))
            m_Animation.Play();
	}
}
