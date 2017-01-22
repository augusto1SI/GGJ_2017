using UnityEngine;
using System.Collections;

public class AudioSources : AudioCore
{
	private static AudioSources _instance;
	public static AudioSources Instance
	{
		get { return _instance; }
	}

	void Awake()
	{
		_instance = this;
	}

	public AudioClip[] m_Music;
	public AudioClip[] m_SFX;
	public AudioClip[] m_SFX_Loop;

	//Must be limited to 2
	public AudioSource[] m_MusicSource;
	private MusicID[] m_MusicSourceID = new MusicID[2];
	//This can have as many as your game needs
	public AudioSource[] m_SFX_Source;
	public AudioSource[] m_SFX_LoopSource;

	private int m_MainMusicSource = 0;
	private int m_NewMainMusicSource = 0;

	private bool m_IsInFade = false;
	private bool m_UsingFadeIn = false;
	private bool m_UsingFadeOut = false;

	private float m_MusicVolume = 1;
	private float m_SFX_Volume = 1;

	private float m_FadeDuration = 0.5f;
	private float m_CurFadeETA = 0;
	private float m_MusicCompletionOffset = 0.01f; //UNDONE Was 0.3f

	private byte m_CoroutineToken = 0;

	private bool[] m_SFX_Available;

	private LoopSFXID[] m_LoopSFX_PlayingID;

	/// <summary>
	/// Initialization for the Sources. Call this before anything else.
	/// </summary>
	public void Initialize()
	{
		MusicSourcesInitialize ();
		MusicSourcesStop ();
		MusicSourcesVolume ();

		SfxInitialize ();
		LoopSfxInitialize ();
	}

	/// <summary>
	/// Initialize variables for the music sources
	/// </summary>
	void MusicSourcesInitialize()
	{
		for(int i = 0; i < m_MusicSource.Length; ++i)
		{
			m_MusicSource [i].clip = m_Music [0];
			m_MusicSourceID [i] = MusicID.None;
		}
	}

	/// <summary>
	/// Stops playing whatever the music sources may have
	/// </summary>
	public void MusicSourcesStop()
	{
		for(int i = 0; i < m_MusicSource.Length; ++i)
			m_MusicSource [i].Stop();
	}

	/// <summary>
	/// Gives the current value for volume to the music sources
	/// </summary>
	public void MusicSourcesVolume()
	{
		for(int i = 0; i < m_MusicSource.Length; ++i)
		{
			m_MusicSource [i].volume = m_MusicVolume;
			m_MusicSource [i].mute = false;
		}
	}

	/// <summary>
	/// Mutes the music sources
	/// </summary>
	public void MusicSourcesMute()
	{
		for(int i = 0; i < m_MusicSource.Length; ++i)
			m_MusicSource [i].mute = true;
	}

	/// <summary>
	/// Plays a given music by its ID. This will override the currently-playing music
	/// </summary>
	/// <param name="_toPlay">_to play.</param>
	/// <param name="_loops">_loops.</param>
	public void MusicSourcesPlay(MusicID _toPlay, int _loops = 1)
	{
		if(_toPlay == MusicID.None) return;

		m_MusicSource [m_MainMusicSource].Stop ();
		m_MusicSource [m_MainMusicSource].clip = m_Music [(int)_toPlay];
		m_MusicSource [m_MainMusicSource].Play ();

		m_MusicSourceID [m_MainMusicSource] = _toPlay;

		StartCoroutine(CheckIfMusicEnded(GetMainMusicTime + (GetMainClipLength * (_loops - 1)), ++m_CoroutineToken));
	}

	/// <summary>
	/// Plays a given music ID asynchronously with a fade in/fade out effect if specified.
	/// Should this function be called without both fades specified, MusicSourcesPlay will be used instead.
	/// </summary>
	/// <param name="_toPlay">_to play.</param>
	/// <param name="_useFadeIn">If set to <c>true</c> _use fade in.</param>
	/// <param name="_useFadeOut">If set to <c>true</c> _use fade out.</param>
	/// <param name="_loops">_loops.</param>
	public void MusicSourcesPlayFadeNoSync(MusicID _toPlay, bool _useFadeIn = true, bool _useFadeOut = true, int _loops = 1)
	{
		if(_toPlay == MusicID.None) return;

		if(m_IsInFade) return;

		if(!_useFadeIn && !_useFadeOut)
		{
			MusicSourcesPlay(_toPlay);
			return;
		}

		m_NewMainMusicSource = GetNewMainMusicSource;
		m_MusicSourceID [m_NewMainMusicSource] = _toPlay;

		m_UsingFadeIn  = _useFadeIn;
		m_UsingFadeOut = _useFadeOut;

		m_MusicSource [m_NewMainMusicSource].Stop ();
		m_MusicSource [m_NewMainMusicSource].clip = m_Music [(int)_toPlay];
		m_MusicSource [m_NewMainMusicSource].volume = m_UsingFadeIn ? 0 : m_MusicVolume;
		m_MusicSource [m_NewMainMusicSource].Play ();

		StartCoroutine(CheckIfMusicEnded(GetNewMainMusicTime + (GetNewMainClipLength * (_loops - 1)), ++m_CoroutineToken));

		m_IsInFade = true;
		m_CurFadeETA = 0f;

		StartCoroutine (FadeProcess ());
	}

	/// <summary>
	/// Plays a given music ID asynchronously with a fade in/fade out effect if specified.
	/// Should this function be called without both fades specified, MusicSourcesPlay will be used instead.
	/// </summary>
	/// <param name="_toPlay">_to play.</param>
	/// <param name="_useFadeIn">If set to <c>true</c> _use fade in.</param>
	/// <param name="_useFadeOut">If set to <c>true</c> _use fade out.</param>
	/// <param name="_loops">_loops.</param>
	public void MusicSourcesPlayFadeWithSync(MusicID _toPlay, bool _useFadeIn = true, bool _useFadeOut = true, int _loops = 1)
	{
		if(_toPlay == MusicID.None) return;

		if(m_IsInFade) return;

		if(!_useFadeIn && !_useFadeOut)
		{
			MusicSourcesPlay(_toPlay);
			return;
		}

		m_NewMainMusicSource = GetNewMainMusicSource;
		m_MusicSourceID [m_NewMainMusicSource] = _toPlay;

		m_UsingFadeIn  = _useFadeIn;
		m_UsingFadeOut = _useFadeOut;

		m_MusicSource [m_NewMainMusicSource].Stop ();
		m_MusicSource [m_NewMainMusicSource].clip = m_Music [(int)_toPlay];
		m_MusicSource [m_NewMainMusicSource].volume = 0;
		m_MusicSource [m_NewMainMusicSource].Play ();

		m_MusicSource [m_NewMainMusicSource].timeSamples = m_MusicSource [m_MainMusicSource].timeSamples;

		if(m_MusicSource[m_NewMainMusicSource].time < m_MusicSource[m_NewMainMusicSource].clip.length - m_MusicCompletionOffset)
		{
			StartCoroutine(CheckIfMusicEnded(GetNewMainMusicTimeWithOffset  + (GetNewMainClipLength * (_loops - 1)), ++m_CoroutineToken));
		}
		else
		{
			StartCoroutine(CheckIfMusicEnded(GetNewMainMusicTimeWithOffset + (GetNewMainClipLength * _loops), ++m_CoroutineToken));
		}

		m_IsInFade = true;
		m_CurFadeETA = 0f;

		StartCoroutine (FadeProcess ());
	}

	/// <summary>
	/// The fade process to use between 2 songs
	/// </summary>
	IEnumerator FadeProcess()
	{
		while(m_CurFadeETA < m_FadeDuration)
		{
			if(m_UsingFadeIn && m_MusicSourceID[m_MainMusicSource]!=MusicID.None) m_MusicSource [m_MainMusicSource].volume = Mathf.Lerp(m_MusicVolume, 0, GetFadeLerpValue);
			if(m_UsingFadeOut && m_MusicSourceID[m_NewMainMusicSource]!=MusicID.None) m_MusicSource [m_NewMainMusicSource].volume = Mathf.Lerp(0, m_MusicVolume, GetFadeLerpValue);
			m_CurFadeETA += Time.deltaTime;
			yield return new WaitForSeconds(0);
		}

		m_MusicSource [m_MainMusicSource].volume = 0;
		m_MusicSource [m_NewMainMusicSource].volume = m_MusicVolume;

		m_MainMusicSource = m_NewMainMusicSource;
		m_IsInFade = false;
		m_CurFadeETA = 0f;
	}

	/// <summary>
	/// Checks if the music has ended.
	/// </summary>
	/// <param name="_wait">Time to wait.</param>
	/// <param name="_token">Token to check if the check is still valid.</param>
	IEnumerator CheckIfMusicEnded(float _wait, byte _token)
	{
		yield return new WaitForSeconds (_wait);

		if(_token == m_CoroutineToken)
		{
			AudioManager.Instance.MusicHasEnded(m_MusicSourceID[m_MainMusicSource]);
		}
	}

	/// <summary>
	/// Gets the value for the fade lerp.
	/// </summary>
	/// <value>The fade lerp value.</value>
	float GetFadeLerpValue
	{
		get { return m_CurFadeETA / m_FadeDuration; }
	}

	/// <summary>
	/// Gets a new main music source.
	/// </summary>
	/// <value>The new main music source.</value>
	int GetNewMainMusicSource
	{
		get { return (m_MainMusicSource + 1) % 2; }
	}

	/// <summary>
	/// Gets a value indicating whether <see cref="AudioSources"/> has a fade in progress.
	/// </summary>
	/// <value><c>true</c> if there's a fade in progress; otherwise, <c>false</c>.</value>
	public bool FadeInProgress
	{
		get { return m_IsInFade; }
	}


	/// <summary>
	/// Gets or sets the volume for the music.
	/// </summary>
	/// <value>The music volume.</value>
	public float MusicVolume
	{
		get { return m_MusicVolume; }
		set { m_MusicVolume = value; }
	}

	/// <summary>
	/// Gets the length of the main clip.
	/// </summary>
	/// <value>The length of the main clip.</value>
	float GetMainClipLength
	{
		get { return m_MusicSource[m_MainMusicSource].clip.length; }
	}

	/// <summary>
	/// Gets the length of the new main clip.
	/// </summary>
	/// <value>The length of the new main clip.</value>
	float GetNewMainClipLength
	{
		get { return m_MusicSource[m_NewMainMusicSource].clip.length; }
	}

	/// <summary>
	/// Gets the time it will take for the main music clip to finish.
	/// </summary>
	/// <value>The get main music time.</value>
	float GetMainMusicTime
	{
		get { return m_MusicSource[m_MainMusicSource].clip.length - m_MusicCompletionOffset; }
	}

	/// <summary>
	/// Gets the time it will take for the new main music clip to finish.
	/// </summary>
	/// <value>The get new main music time.</value>
	float GetNewMainMusicTime
	{
		get { return m_MusicSource[m_NewMainMusicSource].clip.length - m_MusicCompletionOffset; }
	}

	/// <summary>
	/// Gets the time it will take for the main music clip to finish, given how far the cuurent one is on its track
	/// </summary>
	/// <value>The get new main music time with offset.</value>
	float GetNewMainMusicTimeWithOffset
	{
		get { return m_MusicSource[m_NewMainMusicSource].clip.length - m_MusicSource[m_NewMainMusicSource].time - m_MusicCompletionOffset; } 
	}

	/// <summary>
	/// Gets the get current music ID.
	/// </summary>
	/// <value>The get current music ID.</value>
	public MusicID GetCurrentMusicID
	{
		get { return m_MusicSourceID [m_MainMusicSource]; }
	}

	/// <summary>
	/// Initializes the variables for the sound effects.
	/// </summary>
	void SfxInitialize()
	{
		m_SFX_Available = new bool[m_SFX_Source.Length];

		for(int i = 0; i < m_SFX_Available.Length; ++i)
			m_SFX_Available[i]  = true;
	}


	/// <summary>
	/// TO BE CALLED ON IN-GAME TICK
	/// Checks if there's available slots for the sound effects to be played.
	/// </summary>
	public void SfxOnTick()
	{
		for(int i = 0; i < m_SFX_Source.Length; ++i)
		{
			if(m_SFX_Available[i]) continue;

			if(!m_SFX_Source[i].isPlaying)
				m_SFX_Available[i] = true;
		}
	}

	/// <summary>
	/// Plays a specific sound effect
	/// </summary>
	/// <param name="_toPlay">Sound effect to play.</param>
	/// <param name="_pitch">Pitch of the sound effect to play.</param>
    public void SfxPlay(GlobalShit.WaveType _toPlay, float _pitch)
	{
        if (_toPlay == GlobalShit.WaveType.None || _toPlay == GlobalShit.WaveType.MAX) return;

		int _id = GetFreeSFXID;

		if(_id == -1) return;

		m_SFX_Source [_id].clip = m_SFX [(int)_toPlay];
		m_SFX_Source [_id].pitch = _pitch;
		m_SFX_Source [_id].Play ();
		m_SFX_Available [_id] = false;
	}

	/// <summary>
	/// Gives the current value of sound effects volume to all the sound effects sources
	/// </summary>
	public void SfxSourcesVolume()
	{
		for(int i = 0; i < m_SFX_Source.Length; ++i)
			m_SFX_Source [i].volume = m_SFX_Volume;
	}

	/// <summary>
	/// Mutes all the sound effects sources
	/// </summary>
	public void SfxSourcesMute()
	{
		for(int i = 0; i < m_SFX_Source.Length; ++i)
			m_SFX_Source [i].volume = 0;
	}

	/// <summary>
	/// Gets an ID of a sound effect slot that's unoccupied
	/// </summary>
	/// <value>The get free SFXI.</value>
	int GetFreeSFXID
	{
		get
		{
			for(int i = 0; i < m_SFX_Available.Length; ++i)
			{
				if(m_SFX_Available[i]) return i;
			}

			return -1;
		}
	}

	/// <summary>
	/// Initializes the variables for the loop-able sound effects
	/// </summary>
	void LoopSfxInitialize()
	{
		m_LoopSFX_PlayingID = new LoopSFXID[m_SFX_LoopSource.Length];

		for (int i = 0; i < m_LoopSFX_PlayingID.Length; ++i)
			m_LoopSFX_PlayingID [i] = LoopSFXID.None;
	}

	/// <summary>
	/// Plays an specific loop-able sound effect
	/// </summary>
	/// <param name="_toPlay">_to play.</param>
	public void LoopSfxPlay(LoopSFXID _toPlay)
	{
		int _id = GetFreeLoopSFXID;

		if(_id == -1) return;

		m_SFX_LoopSource [_id].Stop ();
		m_SFX_LoopSource [_id].clip = m_SFX_Loop [(int)_toPlay];
		m_SFX_LoopSource [_id].Play ();

		m_LoopSFX_PlayingID [_id] = _toPlay;
	}

	/// <summary>
	/// Stops a loop-able sound effect, if applicable
	/// </summary>
	/// <param name="_toStop">_to stop.</param>
	public void LoopSfxStop(LoopSFXID _toStop)
	{
		int _id = GetStopLoopSFXID(_toStop);
		
		if(_id == -1) return;
		
		m_SFX_LoopSource [_id].Stop ();
		
		m_LoopSFX_PlayingID [_id] = LoopSFXID.None;
	}

	/// <summary>
	/// Gives the current value of sound effects volume to all the loop-able sound effects sources
	/// </summary>
	void LoopSfxSourcesVolume()
	{
		for(int i = 0; i < m_SFX_LoopSource.Length; ++i)
			m_SFX_LoopSource [i].volume = m_SFX_Volume;
	}

	/// <summary>
	/// Mutes all the loop-able sound effects sources
	/// </summary>
	void LoopSfxSourcesMute()
	{
		for(int i = 0; i < m_SFX_LoopSource.Length; ++i)
			m_SFX_LoopSource [i].volume = 0;
	}

	/// <summary>
	/// Gets an ID for a free slot for a loop sound effect to play
	/// </summary>
	/// <value>The get free loop slot id</value>
	int GetFreeLoopSFXID
	{
		get
		{
			for(int i = 0; i < m_LoopSFX_PlayingID.Length; ++i)
			{
				if(m_LoopSFX_PlayingID [i] == LoopSFXID.None) return i;
			}

			return -1;
		}
	}

	/// <summary>
	/// Gets the id for the slot that the requested loop-able sfx is playing in
	/// </summary>
	/// <returns>The stop loop SFX slot id</returns>
	/// <param name="_toStop">_to stop.</param>
	int GetStopLoopSFXID(LoopSFXID _toStop)
	{
		for(int i = 0; i < m_LoopSFX_PlayingID.Length; ++i)
		{
			if(m_LoopSFX_PlayingID [i] == _toStop) return i;
		}
		
		return -1;
	}
}
