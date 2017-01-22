using UnityEngine;
using System.Collections;

public class ArtDispenser : MonoBehaviour {

	private static ArtDispenser _instance;
	public static ArtDispenser Instance
	{
		get{
			return _instance;
		}
	}

	void Awake()
	{
		_instance=this;
	}

	public Sprite[] m_NoteIcons;
	public Color m_HighlightColor;
	public Color m_OpaqueColor;
	public AnimFrameLibrary[] m_AnimLibraries;

	public AnimFrameLibrary GetAnimLibrary(GlobalShit.WaveType _type)
	{
		return m_AnimLibraries[((int)_type)];
	}

	public Sprite GetNoteIcon(int _idx)
	{
		
		return m_NoteIcons[_idx-1];
	}
}
