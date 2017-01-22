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
		TypeG,
		TypeH,
		TypeI,
		TypeJ,
		TypeK,
		TypeL,
		TypeM,
		TypeN,
		MAX
	};

	private static WaveType[] m_BasicWave=new WaveType[]{WaveType.TypeA,WaveType.TypeB,WaveType.TypeC};
	private static WaveType[] m_ParasiteWave=new WaveType[]{WaveType.TypeI,WaveType.TypeJ,WaveType.TypeK,WaveType.TypeL,WaveType.TypeM,WaveType.TypeN};

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

	public static WaveType GetRandomParasiteWave()
	{
		return m_ParasiteWave[Random.Range(0,m_ParasiteWave.Length)];
	}

	public static WaveType GetRandomBasicWave()
	{
		return m_BasicWave[Random.Range(0,m_BasicWave.Length)];
	}

}
