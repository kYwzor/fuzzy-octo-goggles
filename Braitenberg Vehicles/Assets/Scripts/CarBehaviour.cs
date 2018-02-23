using UnityEngine;
using System.Collections;

public class CarBehaviour : MonoBehaviour
{
	public float MaxSpeed;
	public WheelCollider RR;
	public WheelCollider RL;

	private Rigidbody m_Rigidbody;
	public float m_LeftWheelSpeed;
	public float m_RightWheelSpeed;
	private float m_axleLength;

	public enum OutputedWheel{Left,	Right};

	public enum OutputFunction{Linear, Gaussian};

	[System.Serializable]
	public struct DetectorData //Holds info for each sensor
	{
		public DetectorScript detector;
		public OutputedWheel wheel;
		public OutputFunction function;
		public bool inhibitory;

		public float minActivation;
		public float maxActivation;
		public float minValue;
		public float maxValue;
		[HideInInspector]
		public int inhibitoryMultiplier;
	}

	public DetectorData[] detectors;

	void Start ()
	{
		m_Rigidbody = GetComponent<Rigidbody> ();
		m_axleLength = (RR.transform.position - RL.transform.position).magnitude;
		//TESTAR ESTA MERDA
		for (int i = 0; i < detectors.Length; i++) {
			
			detectors[i].inhibitoryMultiplier = detectors[i].inhibitory == true ? -1 : 1;
		}
	}

	void FixedUpdate ()
	{
		float leftOutput = 0, rightOutput = 0;
		int leftCount = 0, rightCount = 0;

		foreach (DetectorData data in detectors) {
			if (data.detector.strength == 0) continue;	//If the sensor doesn't detect anything, it's ignored
			if (data.wheel == OutputedWheel.Left) {
				leftCount++;
				if (data.function == OutputFunction.Linear)
					leftOutput += data.inhibitoryMultiplier * data.detector.GetLinearOutput (data.minActivation, data.maxActivation, data.minValue, data.maxValue);
				else
					leftOutput += data.inhibitoryMultiplier * data.detector.GetGaussianOutput (0.5f, 0.12f, data.minActivation, data.maxActivation, data.minValue, data.maxValue);
			} else {
				rightCount++;
				if (data.function == OutputFunction.Linear)
					rightOutput += data.inhibitoryMultiplier * data.detector.GetLinearOutput (data.minActivation, data.maxActivation, data.minValue, data.maxValue);
				else
					rightOutput += data.inhibitoryMultiplier * data.detector.GetGaussianOutput (0.5f, 0.12f, data.minActivation, data.maxActivation, data.minValue, data.maxValue);
			}
		}

		//Avoiding division by zero!
		m_LeftWheelSpeed = leftCount > 0 ? (leftOutput / leftCount) * MaxSpeed : 0;
		m_RightWheelSpeed = rightCount > 0 ? (rightOutput / rightCount) * MaxSpeed : 0;

			
		//Calculate forward movement
		float targetSpeed = (m_LeftWheelSpeed + m_RightWheelSpeed) / 2;
		Vector3 movement = transform.forward * targetSpeed * Time.deltaTime;

		//Calculate turn degrees based on wheel speed
		float angVelocity = (m_LeftWheelSpeed - m_RightWheelSpeed) / m_axleLength * Mathf.Rad2Deg * Time.deltaTime;
		Quaternion turnRotation = Quaternion.Euler (0.0f, angVelocity, 0.0f);

		//Apply to rigid body
		m_Rigidbody.MovePosition (m_Rigidbody.position + movement);
		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation); 
	}
}
