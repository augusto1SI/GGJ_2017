using UnityEngine;
using System.Collections;

public class SpriteAnim : MonoBehaviour {
	public SpriteRenderer m_SpriteRenderer;
	
	public AnimFrameLibrary m_Library;
	public ImageMove m_SpriteMove;
	public ImageScale m_SpriteScale;
	public SpriteColor m_SpriteColor;

	private int m_AnimImmediateID;
	private int m_AnimImmediateRounds;

	private int m_AnimIDHold;
	private int m_AnimRoundsHold;
	private int m_CurAnimID;
	private int m_RemainingRounds;
	
	private byte m_Key=0;

	private bool m_Paused;

	//play and return the duration for the
	//first key frame appears
	//and the duration of the animation
	public void Play(int _animID,out float _duration,out float _end)
	{
		_end=0;
		_duration=0;

		if(m_SpriteRenderer==null)
			m_SpriteRenderer=GetComponent<SpriteRenderer>();

		if(m_SpriteMove==null)
			m_SpriteMove=GetComponent<ImageMove>();

		if(m_SpriteScale==null)
			m_SpriteScale=GetComponent<ImageScale>();

		if(m_SpriteColor==null)
			m_SpriteColor=GetComponent<SpriteColor>();

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

		if(m_SpriteRenderer!=null)
		{
			m_SpriteRenderer.sprite=m_Library.GetFrameSprite(_animID,_goalID);

			//Tint
			if(m_Library.ShouldTint(_animID,_goalID))
			{
				m_SpriteColor.TintTo(m_Library.GetColorFrom(_animID,_goalID),m_Library.GetColorTo(_animID,_goalID),GetAnimDuration(_animID));
			}

			//Displacement
			if(m_Library.ShouldMove(_animID,_goalID)&&m_SpriteMove!=null)
			{
				m_SpriteMove.MoveTo(m_Library.GetOffsetFrom(_animID,_goalID),m_Library.GetOffsetTo(_animID,_goalID),GetAnimDuration(_animID));
			}

			//Scaling
			if(m_Library.ShouldScale(_animID,_goalID)&&m_SpriteScale!=null)
			{
				m_SpriteScale.ScaleTo(m_Library.GetScaleFrom(_animID,_goalID),m_Library.GetScaleTo(_animID,_goalID),GetAnimDuration(_animID));
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
