﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public abstract class DetectorScript : MonoBehaviour {
	public float angle;
	protected bool useAngle = true;

	public float strength;
	public int numObjects;

	void Start () {
		if (angle >= 360) {
			useAngle = false;
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
		return (float)(Math.Exp (-Math.Pow ((strength - mu), 2) / (2 * Math.Pow (sigma, 2))));
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