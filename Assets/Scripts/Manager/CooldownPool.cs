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
	int m_CountElders=0;
	public List<Cooldown> m_Cooldowns=new List<Cooldown>();
	public List<Cooldown> m_CooldownsElder=new List<Cooldown>();

	void Awake()
	{
		_instance=this;
	}

	public Cooldown GetLarvaCooldown()
	{
		if(m_Cooldowns.Count==m_Count)
		{
			Debug.LogError("DEBE HABER IGUAL NUMERO DE TIMER - SMALL QUE DE LARVAS");
			return null;
		}
			
		m_Count++;
		return m_Cooldowns[m_Count-1];

	}

	public Cooldown GetElderCooldown()
	{
		if(m_CooldownsElder.Count==m_CountElders)
		{
			Debug.LogError("DEBE HABER IGUAL NUMERO DE TIMER - BIG QUE DE ELDERS");
			return null;
		}

		m_CountElders++;
		return m_CooldownsElder[m_CountElders-1];

	}

}
