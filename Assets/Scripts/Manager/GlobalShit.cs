using UnityEngine;
using System.Collections;

public static class GlobalShit {

	public enum WaveType
	{
		None,
		TypeA,
		TypeB,
		TypeC,
		TypeD,
		TypeE,
		TypeF,
		MAX
	};

	private static WaveType[] m_BasicWave=new WaveType[]{WaveType.TypeA,WaveType.TypeB,WaveType.TypeC};

	public static bool IsABasicWave(WaveType _type)
	{
		if(_type==WaveType.None)
			return false;
		for(int i=0;i<m_BasicWave.Length;i++)
		{
			if(_type==m_BasicWave[i])
				return true;
		}
		return false;
	}

	public static WaveType GetRandomBasicWave()
	{
		return m_BasicWave[Random.Range(0,m_BasicWave.Length)];
	}

}
