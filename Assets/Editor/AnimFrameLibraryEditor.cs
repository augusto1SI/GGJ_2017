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

		GUI.color=Color.white;
		EditorGUILayout.BeginVertical("Box");
		GUI.color=Color.white;

		EditorGUILayout.PropertyField(_animList);
		EditorGUI.indentLevel += 1;

		if ( _animList.isExpanded) {
			//show list size
			EditorGUILayout.PropertyField(_animList.FindPropertyRelative("Array.size"));

			for (int i=0;i<_animList.arraySize;i++) 
			{
				GUI.color=Color.red;
				EditorGUILayout.BeginVertical("Window");
				GUI.color=Color.white;

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

				EditorGUILayout.EndVertical();
			}
		
		}

		EditorGUI.indentLevel -= 1;

		EditorGUILayout.EndVertical();
	}


	public void ShowAnimFrames (SerializedProperty _frameList) 
	{
		EditorGUILayout.PropertyField(_frameList);
		EditorGUI.indentLevel += 1;

		if ( _frameList.isExpanded) {
			//show list size
			EditorGUILayout.PropertyField(_frameList.FindPropertyRelative("Array.size"));

			for (int i=0;i<_frameList.arraySize;i++) 
			{

				GUI.color=Color.green;
				EditorGUILayout.BeginVertical("Window");
				GUI.color=Color.white;

				EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i));

			
	
				if(_frameList.GetArrayElementAtIndex(i).isExpanded)
				{

					//Display the Core frame data
					EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_Sprite"));
					EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_Duration"));
					EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_IsKeyFrame"));

					EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_AdvancedSettings"));

					if(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_AdvancedSettings").boolValue)
					{

						//############ COLOR #####################

						GUI.color=Color.yellow;
						EditorGUILayout.BeginVertical("Window");
						GUI.color=Color.white;

						//Display Color frame data
						EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseTint"));
						if(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseTint").boolValue)
						{
							EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_ColorFrom"));	
							EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_ColorTo"));
							EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_ColorCurve"));	
						}

						EditorGUILayout.EndVertical();

						//########################################

						//############ MOVE #####################

						GUI.color=Color.cyan;
						EditorGUILayout.BeginVertical("Window");
						GUI.color=Color.white;

						//Display Displacement frame data
						EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseOffset"));
						if(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseOffset").boolValue)
						{
							//Fixed position or transform reference
							EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseRefFrom"));
							if(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseRefFrom").boolValue)
							{
								EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_TransformFrom"));	
							}
							else
							{
								EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_OffsetFrom"));	
							}

							//Fixed position or transform reference
							EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseRefTo"));
							if(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseRefTo").boolValue)
							{
								EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_TransformTo"));						
							}
							else
							{
								EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_OffsetTo"));		
							}							

							EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_MoveCurveX"));	
							EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_MoveCurveY"));
				
						}

						EditorGUILayout.EndVertical();

						//########################################

						//############ SCALE #####################

						GUI.color=Color.blue;
						EditorGUILayout.BeginVertical("Window");
						GUI.color=Color.white;

						//Display Displacement frame data
						EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseScale"));
						if(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_UseScale").boolValue)
						{


							EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_ScaleFrom"));	
							EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_ScaleTo"));							
							EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_ScaleCurveX"));	
							EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_ScaleCurveY"));


						}

						EditorGUILayout.EndVertical();

						//########################################

						//############ PARTICLES #####################

						GUI.color=Color.magenta;
						EditorGUILayout.BeginVertical("Window");
						GUI.color=Color.white;

						//Play Particles at this frame
						EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_PlayParticles"));
						if(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_PlayParticles").boolValue)
						{
							EditorGUILayout.PropertyField(_frameList.GetArrayElementAtIndex(i).FindPropertyRelative("m_Particles"));	
						}

						EditorGUILayout.EndVertical();

						//########################################
					}

				}
				//EditorGUI.indentLevel-=2;

				EditorGUILayout.EndVertical();
			}
		}
		EditorGUI.indentLevel -= 1;
	}
		

	void Separator(bool _internal=true)
	{
		//Separator
		//if(_internal)
			//GUI.color=Color.black;
		//else
			//GUI.color=Color.magenta;
		EditorGUILayout.HelpBox("",MessageType.None);
		//GUI.color=Color.white;
	}
}