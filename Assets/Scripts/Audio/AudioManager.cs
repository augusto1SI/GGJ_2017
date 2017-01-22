using UnityEngine;
using System.Collections;

public class AudioManager : AudioCore
{
	
	private static AudioManager _instance;
	public static AudioManager Instance
	{
		get { return _instance; }
	}

	private bool m_MutedMusic = false;
	private bool m_MutedSFX = false;

	public bool m_DebugShowNamesAndID;

	void Awake()
	{
		_instance=this;
		DontDestroyOnLoad (this);
	}

	/// <summary>
	/// Initialization for the manager. Call this before anything else.
	/// </summary>
	public void Initialization()
	{
		AudioSources.Instance.Initialize ();
		StartCoroutine (AudioTick ());

		if(m_DebugShowNamesAndID)
		{
			for(int i = 0; i < (int)MusicID.None - 1; ++i)
			{
				Debug.Log("MusicID: " + ((MusicID)i).ToString() + " corresponds to " + AudioSources.Instance.m_Music[i].name);
			}

            for (int i = 0; i < (int)SFXID.None - 1; ++i)
			{
				Debug.Log("SFXID: " + ((MusicID)i).ToString() + " corresponds to " + AudioSources.Instance.m_SFX[i].name);
			}

			for(int i = 0; i < (int)LoopSFXID.None - 1; ++i)
			{
				Debug.Log("LoopSFXID: " + ((MusicID)i).ToString() + " corresponds to " + AudioSources.Instance.m_SFX_Loop[i].name);
			}
		}
		//In case of reading preferences (such as keeping the music muted if the player turned it off)
		//Add a read for the boolean variables and then proceed to mute
	}

	/// <summary>
	/// Entry point for the state machine. This function will handle the cases for each music that plays
	/// </summary>
	/// <param name="_emded">Music ID of the one that has ended.</param>
	public void MusicHasEnded(MusicID _ended)
	{
		//Do state machine code here
		switch(_ended)
		{
			case MusicID.BGMusic_Library:
				PlayMusic(_ended);
				break;
			case MusicID.BGMusic_Match3:
				PlayMusic(_ended);
				break;
			case MusicID.BGMusic_Stack:
				PlayMusic(_ended);
				break;
			default:
				Debug.Log(_ended.ToString() + " music case not handled");
				break;
		}
	}

	/// <summary>
	/// Gets the get current Music ID.
	/// </summary>
	/// <value>The get current Music ID.</value>
	public MusicID GetCurrentMusicID
	{
		get { return AudioSources.Instance.GetCurrentMusicID; }
	}

	/// <summary>
	/// Plays a specific music. The function will decide how to process the request depending of the parameters
	/// </summary>
	/// <param name="_toPlay">The ID of the music to play.</param>
	/// <param name="_useFadeIn">If set to <c>true</c> use fade in.</param>
	/// <param name="_useFadeOut">If set to <c>true</c> use fade out.</param>
	/// <param name="_synced">If set to <c>true</c> the music will be synced to the time.</param>
	public void PlayMusic(MusicID _toPlay, bool _useFadeIn = false, bool _useFadeOut = false, bool _synced = false)
	{
		//Validate track to play

		//Proced to choose the respective play action
		if(_useFadeIn || _useFadeOut)
		{
			if(_synced)
				AudioSources.Instance.MusicSourcesPlayFadeWithSync(_toPlay,_useFadeIn,_useFadeOut);
			else
				AudioSources.Instance.MusicSourcesPlayFadeNoSync(_toPlay,_useFadeIn,_useFadeOut);
		}
		else
			AudioSources.Instance.MusicSourcesPlay(_toPlay);
	}

	/// <summary>
	/// Plays a sound effect.
	/// </summary>
	/// <param name="_toPlay">ID of the sound effect to play.</param>
	/// <param name="_pitch">A varied pitch in case the sound effect needs to sound differently.</param>
	public void PlaySFX(SFXID _toPlay, float _pitch = 1)
	{
		AudioSources.Instance.SfxPlay (_toPlay, _pitch);
	}

	/// <summary>
	/// Plays a loop-able sound effect.
	/// </summary>
	/// <param name="_toPlay">ID of the loop-able sound effect to play.</param>
	public void PlayLoopSFX(LoopSFXID _toPlay)
	{
		AudioSources.Instance.LoopSfxPlay (_toPlay);
	}

	/// <summary>
	/// Tries to stop a loop-able sound effect, if it is currently playing.
	/// </summary>
	/// <param name="_toStop">ID of the loop-able sound effect to stop.</param>
	public void StopLoopSFX(LoopSFXID _toStop)
	{
		AudioSources.Instance.LoopSfxStop (_toStop);
	}

	/// <summary>
	/// A toggle between muting and unmuting the music. To be used in conjunction with the music button in-game.
	/// </summary>
	/// <value><c>true</c> if the music is muted; otherwise, <c>false</c>.</value>
	public bool ToggleMusicButton()
	{
		m_MutedMusic = !m_MutedMusic;

		if (m_MutedMusic)
			AudioSources.Instance.MusicSourcesMute ();
		else
			AudioSources.Instance.MusicSourcesVolume ();

		return m_MutedMusic;
	}

	/// <summary>
	/// Directly mutes the music.
	/// </summary>
	public void MuteMusic()
	{
		AudioSources.Instance.MusicSourcesMute ();
		m_MutedMusic = true;
	}

	/// <summary>
	/// Directly unmutes the music.
	/// </summary>
	public void UnmuteMusic()
	{
		AudioSources.Instance.MusicSourcesVolume ();
		m_MutedMusic = false;
	}

	/// <summary>
	/// A toggle between muting and unmuting sound effects. To be used in conjunction with the sound effect button in-game.
	/// </summary>
	/// <value><c>true</c> if the sound effects are muted; otherwise, <c>false</c>.</value>
	public bool ToggleSFXButton()
	{
		m_MutedSFX = !m_MutedSFX;
		
		if (m_MutedSFX)
			AudioSources.Instance.SfxSourcesMute ();
		else
			AudioSources.Instance.SfxSourcesVolume ();

		return m_MutedSFX;
	}

	/// <summary>
	/// Directly mutes the sound effects.
	/// </summary>
	public void MuteSFX()
	{
		AudioSources.Instance.SfxSourcesMute ();
		m_MutedSFX = true;
	}
	
	/// <summary>
	/// Directly unmutes the sound effects.
	/// </summary>
	public void UnmuteSfx()
	{
		AudioSources.Instance.SfxSourcesVolume ();
		m_MutedSFX = false;
	}

	/// <summary>
	/// Mutes/Unmutes all audio. Use sparingly.
	/// </summary>
	/// <param name="_mute">If set to <c>true</c> mute.</param>
	public void MuteAll(bool _mute)
	{
		if (_mute)
			AudioListener.volume = 0;
		else
			AudioListener.volume = 1;
	}

	/// <summary>
	/// Gets the boolean indicating whether the music is muted or not.
	/// </summary>
	/// <value><c>true</c> if the music is muted; otherwise, <c>false</c>.</value>
	public bool IsMusicMuted
	{
		get { return m_MutedMusic; }
	}

	/// <summary>
	/// Gets the boolean indicating whether the sound effects are muted or not.
	/// </summary>
	/// <value><c>true</c> if the sound effects are muted; otherwise, <c>false</c>.</value>
	public bool IsSFXMuted
	{
		get { return m_MutedSFX; }
	}

	IEnumerator AudioTick()
	{
		while(true)
		{
			AudioSources.Instance.SfxOnTick();
			yield return new WaitForSeconds(0.5f);
		}
	}
}
