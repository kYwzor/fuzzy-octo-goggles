using UnityEngine;
using System.Collections;
using System;

// Subclass that detects blocks. Defines block-specific behaviour
public class ProximityDetectorScript : DetectorScript {
	void Update () {
		GameObject[] blocks;
		float min, current;

		if (useAngle) {
			blocks = GetVisible ("Block");
		} else {
			blocks = GetAll ("Block");
		}


		numObjects = blocks.Length;		//not needed for anything, just for viewing on editor
		min = Mathf.Infinity;	// We assume that the minimum is the highest possible value

		// The cycle below finds the distance to the closest blocks
		foreach (GameObject block in blocks) {
			current = (transform.position - block.transform.position).magnitude;
			min = Mathf.Min (min, current);
		}

		// Draws a debugging line to the closest block
		if (car.debugLines) {
			foreach (GameObject block in blocks) {
				if (min == (transform.position - block.transform.position).magnitude) {
					Debug.DrawLine (transform.position, block.transform.position);
					break;
				}
			}
		}

		// Block strength function
		strength = 1.0f / (min + 1);
	}
}
