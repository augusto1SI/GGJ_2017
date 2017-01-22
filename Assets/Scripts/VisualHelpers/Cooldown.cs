using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour {

	public Image m_Image;
	private float m_ETA=0;

	public void DisplayCooldown(float _delay,Vector3 _position)
	{
		m_Image.transform.position=_position;
		m_Image.enabled=true;
		m_ETA=0;
		StartCoroutine(Show(_delay));
	}

	IEnumerator Show(float _delay)
	{
		while(m_ETA<_delay)
		{
			m_ETA+=Time.deltaTime;
			m_Image.fillAmount=m_ETA/_delay;
			yield return null;
		}
		m_Image.enabled=false;
	}

}
