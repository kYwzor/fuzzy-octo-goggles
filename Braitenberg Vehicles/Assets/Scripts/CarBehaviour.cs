using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
	public float MaxSpeed;
	public WheelCollider RR;
	public WheelCollider RL;

	private Rigidbody m_Rigidbody;
	public float m_LeftWheelSpeed;
	public float m_RightWheelSpeed;
	private float m_axleLength;

	public bool debugLines;

	public enum OutputedWheel{Left,	Right};
	public enum OutputFunction{Linear, Gaussian};
	public enum ConnectionType{Excitatory, Inhibitory};

	public bool drawLines; 

	[System.Serializable]
	public struct DetectorData //Holds info for each sensor
	{
		public DetectorScript detector;
		public OutputedWheel wheel;
		public ConnectionType type;
		public OutputFunction function;

		public float gaussianAverage;
		public float gaussianStandardDeviation;

		public float minActivation;
		public float maxActivation;
		public float minValue;
		public float maxValue;

	}

	public DetectorData[] detectors;

	void Start ()
	{
		m_Rigidbody = GetComponent<Rigidbody> ();
		m_axleLength = (RR.transform.position - RL.transform.position).magnitude;
	}

	void FixedUpdate ()
	{
		float leftOutput = 0, rightOutput = 0, aux;
		int leftCount = 0, rightCount = 0;

		foreach (DetectorData data in detectors) {
			if (data.function == OutputFunction.Linear)
				aux = data.detector.GetLinearOutput (data.minActivation, data.maxActivation, data.minValue, data.maxValue, data.type == ConnectionType.Excitatory);
			else
				aux = data.detector.GetGaussianOutput (data.gaussianAverage, data.gaussianStandardDeviation, data.minActivation, data.maxActivation, data.minValue, data.maxValue, data.type == ConnectionType.Excitatory);
			
			if (data.wheel == OutputedWheel.Left) {
				leftCount++;
				leftOutput += aux;
				
			} else {
				rightCount++;
				rightOutput += aux;
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
