using UnityEngine;
using System.Collections;
using System;

// Subclass that detects light. Defines light-specific behaviour
public class LightDetectorScript : DetectorScript {
	void Update () {
		GameObject[] lights;

		if (useAngle) {
			lights = GetVisible ("Light");
		} else {
			lights = GetAll ("Light");
		}

		strength = 0;
		numObjects = lights.Length;
	
		// Calculates the strength of every light found and draws a debug line to each of them
		foreach (GameObject light in lights) {
			float r = light.GetComponent<Light> ().range;
			strength += 1.0f / ((transform.position - light.transform.position).sqrMagnitude / r + 1);
			if (car.debugLines) {
				Debug.DrawLine (transform.position, light.transform.position);
			}
		}

		// Averages the light strength
		if (numObjects > 0) {
			strength = strength / numObjects;
		}
	}
}
