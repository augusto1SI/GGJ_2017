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
	public Sprite GetNoteIcon(int _idx)
	{
		
		return m_NoteIcons[_idx-1];
	}
}
