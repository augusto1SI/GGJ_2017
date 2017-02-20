using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
	public Transform m_Transform;

	public float m_Acceleration  = 0.5f;
	public float m_Decceleration = -0.7f;

	private Vector3 m_TransformPosition = Vector3.zero;
	private Vector3 m_TemporalPosition = Vector3.zero;
	private Vector3 m_PossibleDestination = Vector3.zero;
	private Vector3 m_NormalizedDirectionToDestination = Vector3.zero;
	private Vector3 m_AccelerationVector = Vector3.zero;
	private Vector3 m_SpeedVector = Vector3.zero;
	public float m_MaxSpeedAtMaxDistance;
	public float m_MaxSpeedAtMinDistance;
	private float m_MinSpeedToClamp = 0.3f;
	private float m_PreviousSpeed;

	private bool m_Braking = false;

	void Start()
	{
		TestTouchRadii.Initialize ();
	}

	public void Move(bool _accelerating, Vector3 _possibleDestination, bool _validTouch)
	{
		if(!_accelerating && m_SpeedVector.magnitude < Mathf.Epsilon) return; //If we're not accelerating and the speed is 0, then we're immovile, and thus we don't need to calculate anything

		m_TemporalPosition = m_Transform.position;    //Hold our current position (to have direct access to it)
		m_PossibleDestination = _possibleDestination; //Hold the probable destination (to modify its height component)

		m_TemporalPosition.y = 0f;    //Set the temporal position and the destination height to 0 to avoid moving in that vector
		m_PossibleDestination.y = 0f;

		m_NormalizedDirectionToDestination = m_PossibleDestination - m_TemporalPosition;
		TestTouchRadii.m_TouchDistance = m_NormalizedDirectionToDestination.magnitude;
		TestTouchRadii.m_PlayerDistance = m_TemporalPosition.magnitude;

		if(_accelerating && TestTouchRadii.TestIgnoreTouch > 0 && _validTouch)
		{
			m_Braking = false;
			AccelCode();
		}
		else
		{
			DeaccelCode();
		}

		m_TransformPosition = m_Transform.position;
		m_TransformPosition.y = 0f;

		m_PreviousSpeed = m_SpeedVector.magnitude;;
	}

	void AccelCode()
	{
		m_NormalizedDirectionToDestination.Normalize(); //Obtain the normalized direction to where we need to go
		m_AccelerationVector = m_NormalizedDirectionToDestination * m_Acceleration; //Add to actual direction to destination the probable new direction to destination, to obtain a gradual shift over time
		
		m_SpeedVector = m_SpeedVector + m_AccelerationVector;

		if (m_SpeedVector.magnitude > MaxSpeed)
			m_SpeedVector = m_SpeedVector.normalized * MaxSpeed;
		
		m_Braking = m_SpeedVector.magnitude < m_PreviousSpeed;
		
		m_Transform.position = Boundary.ValidatePosition(m_Transform.position + (m_SpeedVector * Time.deltaTime));
	}

	void DeaccelToMaxSpeed(float _maxSpeed)
	{
		float _originalMagnitude = m_SpeedVector.magnitude;

		m_NormalizedDirectionToDestination.Normalize(); //Obtain the normalized direction to where we need to go
		m_NormalizedDirectionToDestination += Vector3.Normalize (m_SpeedVector);
		m_NormalizedDirectionToDestination *= 0.5f;
		m_AccelerationVector = m_NormalizedDirectionToDestination * m_Decceleration; //Add to actual direction to destination the probable new direction to destination, to obtain a gradual shift over time
		
		m_SpeedVector = m_SpeedVector + m_AccelerationVector;
		
		if (m_SpeedVector.magnitude < _maxSpeed)
			m_SpeedVector = m_SpeedVector.normalized * _maxSpeed;

		if(m_SpeedVector.magnitude < m_PreviousSpeed) m_Braking = true;

		if (m_SpeedVector.magnitude > _originalMagnitude)
			m_SpeedVector = m_SpeedVector.normalized * _originalMagnitude;
		
		m_Transform.position = Boundary.ValidatePosition(m_Transform.position + (m_SpeedVector * Time.deltaTime));
	}

	void DeaccelCode()
	{
		m_NormalizedDirectionToDestination = Vector3.Normalize(m_SpeedVector);
		m_AccelerationVector = m_NormalizedDirectionToDestination * m_Decceleration;
		
		if(m_SpeedVector.magnitude > m_MinSpeedToClamp)
		{
			m_Braking = true;
			m_SpeedVector = m_SpeedVector + m_AccelerationVector;
			m_Transform.position = Boundary.ValidatePosition(m_Transform.position + (m_SpeedVector * Time.deltaTime));
		}
		else
		{
			m_Braking = false;
			m_SpeedVector = Vector3.zero;
		}
	}
	
	public float MaxSpeed
	{
		get
		{
			return Mathf.Lerp(m_MaxSpeedAtMinDistance, m_MaxSpeedAtMaxDistance, TestTouchRadii.MinMaxLerp) * TestTouchRadii.TestOuterRadiusDamp * TestTouchRadii.TestIgnoreTouch ;
		}
	}

	public float GetNormalizedXDirection
	{
		get
		{
			return m_SpeedVector.x / Mathf.Lerp(m_MaxSpeedAtMinDistance, m_MaxSpeedAtMaxDistance, TestTouchRadii.MinMaxLerp);
		}
	}

	public float GetSpeedMagnitude
	{
		get
		{
			return m_SpeedVector.magnitude;
		}
	}

	public Vector3 GetTransformPosition
	{
		get
		{
			return m_TransformPosition;
		}
	}

	public Vector3 GetLastTransformPosition
	{
		get
		{
			return m_TemporalPosition;
		}
	}

	public float GetNetMovement
	{
		get
		{
			return Vector3.Magnitude(GetLastTransformPosition - GetTransformPosition);
		}
	}

	public float GetMovementAngle
	{
		get
		{

			if(m_SpeedVector.magnitude > 0)
			{
				if(m_SpeedVector.z >= 0)
					return 270 + (Mathf.Asin(m_SpeedVector.x/m_SpeedVector.magnitude) * Mathf.Rad2Deg);
				else
					return 90 - Mathf.Asin(m_SpeedVector.x/m_SpeedVector.magnitude) * Mathf.Rad2Deg;
			}
			else
				return 0;
		}
	}

	public bool Braking
	{
		get
		{
			return m_Braking;
		}
	}
}
