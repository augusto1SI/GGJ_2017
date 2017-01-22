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
	private static WaveType[] m_LarvaWave=new WaveType[]{WaveType.TypeD,WaveType.TypeE,WaveType.TypeF,WaveType.TypeG,WaveType.TypeH,WaveType.TypeI,WaveType.TypeJ,WaveType.TypeK,WaveType.TypeL};





	private static WaveType[] m_EvolvingSequenceTypeD=new WaveType[]{WaveType.TypeC,WaveType.TypeB,WaveType.TypeB};
	private static WaveType[] m_EvolvingSequenceTypeE=new WaveType[]{WaveType.TypeA,WaveType.TypeB,WaveType.TypeA};
	private static WaveType[] m_EvolvingSequenceTypeF=new WaveType[]{WaveType.TypeC,WaveType.TypeC,WaveType.TypeA};
	private static WaveType[] m_EvolvingSequenceTypeG=new WaveType[]{WaveType.TypeA,WaveType.TypeE,WaveType.TypeA,WaveType.TypeE,WaveType.TypeE};
	private static WaveType[] m_EvolvingSequenceTypeH=new WaveType[]{WaveType.TypeF,WaveType.TypeC,WaveType.TypeF,WaveType.TypeF,WaveType.TypeC};
	private static WaveType[] m_EvolvingSequenceTypeI=new WaveType[]{WaveType.TypeD,WaveType.TypeD,WaveType.TypeB,WaveType.TypeB,WaveType.TypeD};
	private static WaveType[] m_EvolvingSequenceTypeJ=new WaveType[]{WaveType.TypeE,WaveType.TypeH,WaveType.TypeE,WaveType.TypeH,WaveType.TypeD,WaveType.TypeH};
	private static WaveType[] m_EvolvingSequenceTypeK=new WaveType[]{WaveType.TypeF,WaveType.TypeD,WaveType.TypeG,WaveType.TypeG,WaveType.TypeG,WaveType.TypeD};
	private static WaveType[] m_EvolvingSequenceTypeL=new WaveType[]{WaveType.TypeI,WaveType.TypeF,WaveType.TypeI,WaveType.TypeE,WaveType.TypeF,WaveType.TypeI};


	private static WaveType[] m_EvolvingSequenceTypeM=new WaveType[]{WaveType.TypeJ,WaveType.TypeG,WaveType.TypeG,WaveType.TypeK,WaveType.TypeH,WaveType.TypeD,WaveType.TypeL,WaveType.TypeE};
	private static WaveType[] m_EvolvingSequenceTypeN=new WaveType[]{WaveType.TypeL,WaveType.TypeI,WaveType.TypeI,WaveType.TypeJ,WaveType.TypeH,WaveType.TypeE,WaveType.TypeK,WaveType.TypeF};

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

	public static WaveType GetRandomLarvaWave(int _tier)
	{
		switch(_tier)
		{
			case 0:
				return m_LarvaWave[Random.Range(0,3)];
			break;
			case 1:
				return m_LarvaWave[Random.Range(3,6)];
			break;
			case 2:
				return m_LarvaWave[Random.Range(6,9)];
			break;
			default:
				return m_LarvaWave[Random.Range(0,m_LarvaWave.Length)];
			break;
		}
	}

	public static WaveType[] GetEvolvingLarvaSequence(WaveType _type)
	{
		switch(_type)
		{
			case WaveType.TypeD:
				return m_EvolvingSequenceTypeD;
			break;
			case WaveType.TypeE:
				return m_EvolvingSequenceTypeE;
			break;
			case WaveType.TypeF:
				return m_EvolvingSequenceTypeF;
			break;
			case WaveType.TypeG:
				return m_EvolvingSequenceTypeG;
			break;
			case WaveType.TypeH:
				return m_EvolvingSequenceTypeH;
			break;
			case WaveType.TypeI:
				return m_EvolvingSequenceTypeI;
			break;
			case WaveType.TypeJ:
				return m_EvolvingSequenceTypeJ;
			break;
			case WaveType.TypeK:
				return m_EvolvingSequenceTypeK;
			break;
			case WaveType.TypeL:
				return m_EvolvingSequenceTypeL;
			break;
		}
		return m_EvolvingSequenceTypeD;
	}

}
