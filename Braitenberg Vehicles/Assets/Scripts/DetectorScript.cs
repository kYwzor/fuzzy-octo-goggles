using UnityEngine;
using System.Collections;
using System;

public abstract class DetectorScript : MonoBehaviour {
	public float angle;
	protected bool useAngle = true;

	public float strength;
	public int numObjects;

	protected CarBehaviour car;

	void Start () {
		if (angle >= 360) {
			useAngle = false;
		}

		car = transform.parent.parent.GetComponent<CarBehaviour>();
	}

	// Get linear output value
	public float GetLinearOutput(float minActivation, float maxActivation, float minValue, float maxValue, bool type)
	{
		if (strength < minActivation || strength > maxActivation)
			return minValue;
		
		return Mathf.Clamp(type ? strength : 1 - strength, minValue, maxValue);
	}

	// Get gaussian output value
	public float GetGaussianOutput(double mu, double sigma, float minActivation, float maxActivation, float minValue, float maxValue, bool type)
	{
		if (strength < minActivation || strength > maxActivation)
			return minValue;
		
		float gauss = (float)(Math.Exp (-Math.Pow ((strength - mu), 2) / (2 * Math.Pow (sigma, 2))));

		return Mathf.Clamp(type ? gauss : 1 - gauss, minValue, maxValue);
	}

	// Returns all objects tagged as tag. The sensor angle is not taken into account.
	protected GameObject[] GetAll(String tag)
	{
		return GameObject.FindGameObjectsWithTag (tag);
	}

	// Returns all objects tagged as tag that are within the view angle of the Sensor. 
	// Only considers the angle over the y axis. Does not consider objects blocking the view.
	protected GameObject[] GetVisible(String tag)
	{
		ArrayList visibleList = new ArrayList();
		float halfAngle = angle / 2.0f;

		GameObject[] objects = GameObject.FindGameObjectsWithTag (tag);

		foreach (GameObject obj in objects) {
			Vector3 toVector = (obj.transform.position - transform.position);
			Vector3 forward = transform.forward;
			toVector.y = 0;
			forward.y = 0;
			float angleToTarget = Vector3.Angle (forward, toVector);

			if (angleToTarget <= halfAngle) {
				visibleList.Add (obj);
			}
		}

		return (GameObject[])visibleList.ToArray(typeof(GameObject));
	}
}
