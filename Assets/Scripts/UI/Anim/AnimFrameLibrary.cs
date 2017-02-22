using UnityEngine;
using System.Collections;

public class AnimFrameLibrary : MonoBehaviour {

	public SAnimation[] m_Anims;

	public bool IsValidAnimID(int _animID)
	{
		return m_Anims.Length>_animID;
	}

	public bool IsLoop(int _animID)
	{
		return m_Anims[_animID].m_IsLoop;
	}

	public int GetFramesCount(int _animID)
	{
		return m_Anims[_animID].m_AnimGoals.Length;
	}

	public float GetFrameDuration(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_Duration;
	}

	public bool UseADifferentSprite(int _animID, int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_Sprite!=null;
	}

	public Sprite GetFrameSprite(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_Sprite;
	}

	public int GetAnimRounds(int _animID)
	{
		return m_Anims[_animID].m_Rounds;
	}

	public bool IsAfterAnimEnabled(int _animID)
	{
		return m_Anims[_animID].m_PlayAfterFinish;
	}

	public int GetAfterAnimSelected(int _animID)
	{
		return m_Anims[_animID].m_AnimToPlayAfterFinish;
	}

	public bool IsAKeyFrame(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_IsKeyFrame;
	}

	public bool UseAdvancedSettings(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_AdvancedSettings;
	}

	public bool ShouldTint(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_UseTint;
	}

	public Color GetColorFrom(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_ColorFrom;
	}

	public Color GetColorTo(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_ColorTo;
	}

	public AnimationCurve GetColorCurve(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_ColorCurve;
	}

	public bool ShouldMove(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_UseOffset;
	}

	public Vector2 GetOffsetFrom(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_OffsetFrom;
	}

	public Vector2 GetOffsetTo(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_OffsetTo;
	}

	public AnimationCurve GetMoveCurveX(int _animID, int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_MoveCurveX;
	}

	public AnimationCurve GetMoveCurveY(int _animID, int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_MoveCurveY;
	}

	public bool ShouldScale(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_UseScale;
	}

	public Vector2 GetScaleFrom(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_ScaleFrom;
	}

	public Vector2 GetScaleTo(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_ScaleTo;
	}

	public AnimationCurve GetScaleCurveX(int _animID, int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_ScaleCurveX;
	}

	public AnimationCurve GetScaleCurveY(int _animID, int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_ScaleCurveY;
	}

	public bool ShouldPlayParticles(int _animID,int _frameID)
	{
		return m_Anims[_animID].m_AnimGoals[_frameID].m_PlayParticles;
	}

	public void PlayParticles(int _animID,int _frameID)
	{
		if(m_Anims[_animID].m_AnimGoals[_frameID].m_Particles!=null)
		{
			m_Anims[_animID].m_AnimGoals[_frameID].m_Particles.Play(true);
		}
	}
}
