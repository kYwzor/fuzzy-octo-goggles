using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class ProximityDetectorScript : DetectorScript {
	void Update () {
		GameObject[] blocks;
		float max = 0, current = 0;

		if (useAngle) {
			blocks = GetVisible ("Block");
		} else {
			blocks = GetAll ("Block");
		}
		numObjects = blocks.Length;

		foreach (GameObject block in blocks) {
			// I'm not sure what I should be doing here
			current = (transform.position - block.transform.position).sqrMagnitude;
			if (current > max) {
				max = current;
			}

		}
		strength = numObjects > 0 ? 1.0f / (max + 1) : 0;
	}
}
