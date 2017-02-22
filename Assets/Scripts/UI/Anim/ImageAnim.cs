using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageAnim : MonoBehaviour {
	[Header("REFERENCES")]
	public Image m_Renderer;
	public AnimFrameLibrary m_Library;
	public ImageMove m_Move;
	public ImageScale m_Scale;
	public ImageColor m_Tint;

	private int m_AnimImmediateID;
	private int m_AnimImmediateRounds;

	private int m_AnimIDHold;
	private int m_AnimRoundsHold;
	private int m_CurAnimID;
	private int m_RemainingRounds;
	
	private byte m_Key=0;

	private bool m_Paused;

	[Header("SPECIFIERS")]
	public bool m_SetNativeSize;
	public bool m_PlayOnStart=false;
	public int m_IndexToStart=0;

	void Start()
	{

		if(m_Renderer==null)
			m_Renderer=GetComponent<Image>();

		if(m_Move==null)
			m_Move=GetComponent<ImageMove>();

		if(m_Scale==null)
			m_Scale=GetComponent<ImageScale>();

		if(m_Tint==null)
			m_Tint=GetComponent<ImageColor>();
		
		if(m_Library==null)
		{
			m_Library=GetComponent<AnimFrameLibrary>();
		}

		if(m_PlayOnStart)
			Play(m_IndexToStart);
	}


	//play and return the duration for the
	//first key frame appears
	//and the duration of the animation
	public void Play(int _animID,out float _duration,out float _end)
	{
		_end=0;
		_duration=0;

		if(m_Library==null)
		{
			Debug.LogWarning(name+" has not a library assigned!");
			return;
		}

		if(!m_Library.IsValidAnimID(_animID))
			return;

		m_Key=(m_Key==byte.MaxValue)?(byte)1:(byte)(m_Key+1);

		m_CurAnimID=_animID;
		m_RemainingRounds=m_Library.GetAnimRounds(_animID);
		m_RemainingRounds=m_RemainingRounds==0?1:m_RemainingRounds;
		StartCoroutine(PlayAnimation(m_Key));

		_duration = GetKeyFrameDuration(_animID);
		_end=GetAnimDuration(_animID);
	}

	//play and return the duration for the
	//first key frame appears
	public void Play(int _animID,out float _duration)
	{
		_duration=0;

		if(m_Library==null)
		{
			Debug.LogWarning(name+" has not a library assigned!");
			return;
		}

		if(!m_Library.IsValidAnimID(_animID))
			return;

		m_Key=(m_Key==byte.MaxValue)?(byte)1:(byte)(m_Key+1);

		m_CurAnimID=_animID;
		m_RemainingRounds=m_Library.GetAnimRounds(_animID);
		m_RemainingRounds=m_RemainingRounds==0?1:m_RemainingRounds;
		StartCoroutine(PlayAnimation(m_Key));

		_duration = GetKeyFrameDuration(_animID);
	}

	//simple play animation (no return value)
	public void Play(int _animID)
	{
		if(m_Library==null)
		{
			Debug.LogWarning(name+" has not a library assigned!");
			return;
		}

		if(!m_Library.IsValidAnimID(_animID))
			return;
		
		m_Key=(m_Key==byte.MaxValue)?(byte)1:(byte)(m_Key+1);

		m_CurAnimID=_animID;
		m_RemainingRounds=m_Library.GetAnimRounds(_animID);
		m_RemainingRounds=m_RemainingRounds==0?1:m_RemainingRounds;
		StartCoroutine(PlayAnimation(m_Key));
	}
	
	IEnumerator PlayAnimation(byte _curKey)
	{
		do
		{
			for(int i=0;i<m_Library.GetFramesCount(m_CurAnimID);i++)
			{
				yield return new WaitForSeconds(MoveNextGoal(m_CurAnimID,i));

				while(m_Paused)
					yield return 0;

				//if this animation its not the current one then stop this coroutine
				if(_curKey!=m_Key)
				{
					i=m_Library.GetFramesCount(m_CurAnimID);
				}
			}
		}while(_curKey==m_Key&&KeepLooping(m_CurAnimID));

		//do we have to play an animation after this one?
		if(_curKey==m_Key&&m_Library.IsAfterAnimEnabled(m_CurAnimID))
		{
			Play(m_Library.GetAfterAnimSelected(m_CurAnimID));
		}
	}
	
	//change the animation ID so all the coroutines will stop playing as soon as possible
	public void Stop()
	{
		if(m_CurAnimID==0)
			m_Key=1;
		else
			m_Key=0;
	}
	
	bool KeepLooping(int _idx)
	{
		if(m_Library.IsLoop(_idx))
		{
			if(m_RemainingRounds==-1)
				return true;
			else
			{
				m_RemainingRounds--;
				return m_RemainingRounds!=0;
			}
		}
		else
			return false;
	}
	
	float MoveNextGoal(int _animID,int _goalID)
	{

		if(m_Renderer!=null)
		{
			//Sprite
			if(m_Library.UseADifferentSprite(_animID,_goalID))
				m_Renderer.sprite=m_Library.GetFrameSprite(_animID,_goalID);
			
			//if the component require native size then do it
			if(m_SetNativeSize)
				m_Renderer.SetNativeSize();

			if(m_Library.UseAdvancedSettings(_animID,_goalID))
			{
				//Tint
				if(m_Library.ShouldTint(_animID,_goalID)&&m_Tint!=null)
				{
					m_Tint.TintTo(m_Library.GetColorFrom(_animID,_goalID),m_Library.GetColorTo(_animID,_goalID),m_Library.GetColorCurve(_animID,_goalID),m_Library.GetFrameDuration(_animID,_goalID));
				}

				//Displacement
				if(m_Library.ShouldMove(_animID,_goalID)&&m_Move!=null)
				{
					m_Move.MoveTo(m_Library.GetOffsetFrom(_animID,_goalID),m_Library.GetOffsetTo(_animID,_goalID),m_Library.GetMoveCurveX(_animID,_goalID),m_Library.GetMoveCurveY(_animID,_goalID),m_Library.GetFrameDuration(_animID,_goalID));
				}

				//Scaling
				if(m_Library.ShouldScale(_animID,_goalID)&&m_Scale!=null)
				{
					m_Scale.ScaleTo(m_Library.GetScaleFrom(_animID,_goalID),m_Library.GetScaleTo(_animID,_goalID),m_Library.GetScaleCurveX(_animID,_goalID),m_Library.GetScaleCurveY(_animID,_goalID),m_Library.GetFrameDuration(_animID,_goalID));
				}

				//Particles
				if(m_Library.ShouldPlayParticles(_animID,_goalID))
					m_Library.PlayParticles(_animID,_goalID);
			}
				
			return m_Library.GetFrameDuration(_animID,_goalID);
		}
		return 0;
	}

	float GetKeyFrameDuration(int _animID)
	{
		float _duration=0;
		for(int i=0;i<m_Library.GetFramesCount(_animID);i++)
		{
			if(m_Library.IsAKeyFrame(_animID,i))
				return _duration;
			_duration+=m_Library.GetFrameDuration(_animID,i);
		}
		return _duration;
	}

	public float GetAnimDuration(int _animID)
	{
		float _duration=0;
		for(int i=0;i<m_Library.GetFramesCount(_animID);i++)
		{
			_duration+=m_Library.GetFrameDuration(_animID,i);
		}
		return _duration;
	}

	public void Pause(bool _pause)
	{
		m_Paused=_pause;
	}
}
