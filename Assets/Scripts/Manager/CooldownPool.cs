using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CooldownPool : MonoBehaviour {

	private static CooldownPool _instance;
	public static CooldownPool Instance
	{
		get{
			return _instance;
		}
	}
		
	int m_Count=0;
	public List<Cooldown> m_Cooldowns=new List<Cooldown>();

	void Awake()
	{
		_instance=this;
	}

	public Cooldown GetCooldown()
	{
		if(m_Cooldowns.Count==m_Count)
		{
			Debug.LogError("DEBE HABER IGUAL NUMERO DE TIMER QUE DE LARVAS");
			return null;
		}
			
		m_Count++;
		return m_Cooldowns[m_Count-1];

	}

}
