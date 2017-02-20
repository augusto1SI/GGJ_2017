using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(LarvaCreator))]
public class LarvaCreatorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		LarvaCreator LC = (LarvaCreator)target;

		LC.m_LarvaPrefab = EditorGUILayout.ObjectField ("Larva prefab", LC.m_LarvaPrefab, typeof(GameObject), true) as GameObject;

		if(GUILayout.Button("Set Larvas"))
		{
			LC.SetLarvas();
		}
	}
}
