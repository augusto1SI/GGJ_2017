using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AnimFrameLibrary))]
public class AnimFrameLibraryEditor : Editor {
	
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		//Display the array
		ShowAnimations(serializedObject.FindProperty("m_Anims"));

		serializedObject.ApplyModifiedProperties();
		//base.OnInspectorGUI();	

	}


	public void ShowAnimations (SerializedProperty _animList) {
		Separator(false);
		EditorGUILayout.PropertyField(_animList);
		EditorGUI.indentLevel += 1;

		if ( _animList.isExpanded) {
			//show list size
			EditorGUILayout.PropertyField(_animList.FindPropertyRelative("Array.size"));

			for (int i=0;i<_animList.arraySize;i++) 
			{

				EditorGUILayout.PropertyField(_animList.GetArrayElementAtIndex(i));
				EditorGUI.indentLevel+=2;
				if(_animList.GetArrayElementAtIndex(i).isExpanded)
				{
					EditorGUILayout.PropertyField(_animList.GetArrayElementAtIndex(i).FindPropertyRelative("name"));
					//Loop or single
					EditorGUILayout.PropertyField(_animList.GetArrayElementAtIndex(i).FindPropertyRelative("m_IsLoop"));
					if(_animList.GetArrayElementAtIndex(i).FindPropertyRelative("m_IsLoop").boolValue)
					{
						EditorGUILayout.PropertyField(_animList.GetArrayElementAtIndex(i).FindPropertyRelative("m_Rounds"));	
					}
					//Play after finish
					EditorGUILayout.PropertyField(_animList.GetArrayElementAtIndex(i).FindPropertyRelative("m_PlayAfterFinish"));
					if(_animList.GetArrayElementAtIndex(i).FindPropertyRelative("m_PlayAfterFinish").boolValue)
					{
						EditorGUILayout.PropertyField(_animList.GetArrayElementAtIndex(i).FindPropertyRelative("m_AnimToPlayAfterFinish"));	
					}

					ShowAnimFrames(_animList.GetArrayElementAtIndex(i).FindPropertyRelative("m_AnimGoals"));

				}
				EditorGUI.indentLevel-=2;
				Separator();
			}
		
		}

		EditorGUI.indentLevel -= 1;
	}


	public void ShowAnimFrames (SerializedProperty _frameList) 
	{
		Separator(false);
		EditorGUILayout.PropertyField(_frameList);
		EditorGUI.indentLevel += 1;

		if ( _frameList.isExpanded) {
			//show list size
			EditorGUILayout.PropertyField(_frameList.FindPropertyRelative("Array.size"));

			for (int i=0;i<_frameList.arraySize;i++) 
			{

				EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i));
			//	EditorGUI.indentLevel+=2;
				if(_frameList.GetArrayElementAtIndex(i).isExpanded)
				{
					//Display the Core frame data
					EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_Sprite"));
					EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_Duration"));
					EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_IsKeyFrame"));

					//Display Color frame data
					EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseTint"));
					if(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseTint").boolValue)
					{
						EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_ColorFrom"));	
						EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_ColorTo"));	
					}

					//Display Displacement frame data
					EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseOffset"));
					if(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseOffset").boolValue)
					{
						EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_OffsetFrom"));	
						EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_OffsetTo"));	
					}

					//Display Displacement frame data
					EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseScale"));
					if(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseScale").boolValue)
					{
						EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_ScaleFrom"));	
						EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_ScaleTo"));	
					}

					//Play Particles at this frame
					EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_PlayParticles"));
					if(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_PlayParticles").boolValue)
					{
						EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_Particles"));	
					}
				}
				//EditorGUI.indentLevel-=2;
				Separator();
			}
		}
		EditorGUI.indentLevel -= 1;
	}
		

	void Separator(bool _internal=true)
	{
		//Separator
		if(_internal)
			GUI.color=Color.black;
		else
			GUI.color=Color.magenta;
		EditorGUILayout.HelpBox("",MessageType.None);
		GUI.color=Color.white;
	}
}