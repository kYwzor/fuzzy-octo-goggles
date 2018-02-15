using UnityEngine;
using System.Collections;

public class CarBehaviour : MonoBehaviour {
	
	public float MaxSpeed;
	public WheelCollider RR;
	public WheelCollider RL;
	public LightDetectorScript LeftLD;
	public LightDetectorScript RightLD;

	private Rigidbody m_Rigidbody;
	public float m_LeftWheelSpeed;
	public float m_RightWheelSpeed;
	private float m_axleLength;

	//Acrescentei este codigo top da net que da para mudar as coisas numa dropdown list no unity para nao termos de estar sempre a mudar em codigo
	public enum wiringMode {Default, Reversed};
	public wiringMode wiring;
	public enum outputMode{Linear, Gaussian};
	public outputMode output;
	void Start()
	{
		m_Rigidbody = GetComponent<Rigidbody> ();
		m_axleLength = (RR.transform.position - RL.transform.position).magnitude;
	}

	void FixedUpdate () {
		//Valores lidos pelos sensores de luz, podem ser:
		//Linear - return = strength
		//Gaussian - return = formula top, da para ver na wikipedia. fiz com b = 0.5 e c = 0.12, pareceu me que era o que eles queriam no enunciado
		float leftSensor, rightSensor;
		if (output == outputMode.Gaussian) {	
			leftSensor = LeftLD.GetGaussianOutput (0.5f, 0.12f);
			rightSensor = RightLD.GetGaussianOutput (0.5f, 0.12f);
		}
		else{
			leftSensor = LeftLD.GetLinearOutput ();
			rightSensor = RightLD.GetLinearOutput ();

		}

		//Isto e uma merda que ele falou na aula de ligar os sensores as rodas normalmente (Default) ou cruzados (Reversed)
		//o Default ainda esta meio esquisito mas eu acho que e o comportamento esperado tbh, se conseguirem descancerizar ou tiverem ideias e top
		if (wiring == wiringMode.Default) {
			m_LeftWheelSpeed = leftSensor * MaxSpeed;
			m_RightWheelSpeed = rightSensor * MaxSpeed;
		}
		else{
			m_LeftWheelSpeed = rightSensor * MaxSpeed;
			m_RightWheelSpeed = leftSensor * MaxSpeed;
		}
			
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
