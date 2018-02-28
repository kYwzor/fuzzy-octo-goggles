using UnityEngine;
using System.Collections;
using System;

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
	
		foreach (GameObject light in lights) {
			float r = light.GetComponent<Light> ().range;
			strength += 1.0f / ((transform.position - light.transform.position).sqrMagnitude / r + 1);
			if (car.debugLines) {
				Debug.DrawLine (transform.position, light.transform.position);
			}
		}

		if (numObjects > 0) {
			strength = strength / numObjects;
		}
	}
}
