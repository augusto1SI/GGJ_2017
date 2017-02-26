using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LarvaCreator : MonoBehaviour
{
	public GameObject m_LarvaPrefab;

	public void SetLarvas()
	{
		UnitLarva[] _larvas = GameObject.FindObjectsOfType (typeof(UnitLarva)) as UnitLarva[];
		List<UnitLarva> _larvaList = new List<UnitLarva> (_larvas);

		for(int i = 0; i < _larvaList.Count; ++i)
		{
			if(_larvaList[i].m_ComesFromElder)
			{
				_larvaList.RemoveAt(i);
				i=-1;
			}
		}

		_larvas = _larvaList.ToArray ();

		for(int i = 0; i < _larvas.Length; ++i)
		{
			if(_larvas[i].m_ComesFromElder) continue;

			GameObject _go = Instantiate(m_LarvaPrefab);
			_go.transform.SetParent(_larvas[i].transform.parent);
			_go.transform.localPosition = _larvas[i].transform.localPosition;
			_go.name = "Larva_" + i.ToString();
			UnitLarva _ul = _go.GetComponent<UnitLarva>();
			_ul.m_Tier = _larvas[i].m_Tier;
			DestroyImmediate(_larvas[i].gameObject);
		}
	}
}
