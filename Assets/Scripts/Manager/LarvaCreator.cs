using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LarvaCreator : MonoBehaviour
{
	public GameObject m_LarvaPrefab;

	public void SetLarvas()
	{
		UnitLarva[] _larvas = GameObject.FindObjectsOfType (typeof(UnitLarva)) as UnitLarva[];

		for(int i = 0; i < _larvas.Length; ++i)
		{
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
