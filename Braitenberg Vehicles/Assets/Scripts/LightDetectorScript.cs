using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class LightDetectorScript : MonoBehaviour {

	public float angle;
	private bool useAngle = true;

	public float returnValue;
	public float strength;
	public int numObjects;

	void Start () {
		strength = 0;
		numObjects = 0;

		if (angle >= 360) {
			useAngle = false;
		}
	}

	void Update () {
		GameObject[] lights;

		if (useAngle) {
			lights = GetVisibleLights ();
		} else {
			lights = GetAllLights ();
		}

		strength = 0;
		numObjects = lights.Length;
	
		foreach (GameObject light in lights) {
			float r = light.GetComponent<Light> ().range;
			strength += 1.0f / ((transform.position - light.transform.position).sqrMagnitude / r + 1);
		}

		if (numObjects > 0) {
			strength = strength / numObjects;
		}
	}

	// Get linear output value
	public float GetLinearOutput()
	{
		return strength;
	}

	// Get gaussian output value
	public float GetGaussianOutput(double mu, double sigma)
	{
		//Formula tirada da wikipedia :^)
		returnValue = (float)( Math.Exp (-Math.Pow ((strength - mu), 2) / (2 * Math.Pow (sigma, 2))));
		return returnValue;

	}

		
	// Returns all "Light" tagged objects. The sensor angle is not taken into account.
	GameObject[] GetAllLights()
	{
		return GameObject.FindGameObjectsWithTag ("Light");
	}

	// Returns all "Light" tagged objects that are within the view angle of the Sensor. 
	// Only considers the angle over the y axis. Does not consider objects blocking the view.
	GameObject[] GetVisibleLights()
	{
		ArrayList visibleLights = new ArrayList();
		float halfAngle = angle / 2.0f;

		GameObject[] lights = GameObject.FindGameObjectsWithTag ("Light");

		foreach (GameObject light in lights) {
			Vector3 toVector = (light.transform.position - transform.position);
			Vector3 forward = transform.forward;
			toVector.y = 0;
			forward.y = 0;
			float angleToTarget = Vector3.Angle (forward, toVector);

			if (angleToTarget <= halfAngle) {
				visibleLights.Add (light);
			}
		}

		return (GameObject[])visibleLights.ToArray(typeof(GameObject));
	}


}
