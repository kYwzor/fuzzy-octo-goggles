using UnityEngine;
using System.Collections;
using System;

/* 
 * Detector superclass. 
 * Defines the behaviour of output functions and their possible configurations (thresholds, limits and mode)
 * Finds objects within the angle defined on the editor 
*/
public abstract class DetectorScript : MonoBehaviour {
	public float angle;
	protected bool useAngle = true;

	public float strength;
	public int numObjects;

	protected CarBehaviour car;


	void Start () {
		// If the angle is 360 or above a flag is activated to simplify the search function
		if (angle >= 360) {
			useAngle = false;
		}

		// We need to grab the car's script in order to draw debugging lines. Not needed otherwise
		car = transform.parent.parent.GetComponent<CarBehaviour>();
	}

	// Get linear output value
	public float GetLinearOutput(float minActivation, float maxActivation, float minValue, float maxValue, bool altered)
	{
		if (strength < minActivation || strength > maxActivation)
			return minValue;
		
		return Mathf.Clamp(altered ? 1 - strength : strength, minValue, maxValue);
	}

	// Get gaussian output value
	public float GetGaussianOutput(double mu, double sigma, float minActivation, float maxActivation, float minValue, float maxValue, bool altered)
	{
		Debug.Log (strength);
		if (strength < minActivation || strength > maxActivation)
			return minValue;

		float gauss = (float)(Math.Exp (-Math.Pow ((strength - mu), 2) / (2 * Math.Pow (sigma, 2))));

		return Mathf.Clamp(altered ? 1 - gauss : gauss, minValue, maxValue);
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
