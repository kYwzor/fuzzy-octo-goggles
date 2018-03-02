using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows to move any assigned GameObject with arrows or WASD
public class ObjectMovementScript : MonoBehaviour {
	public GameObject controlledObject;
	void FixedUpdate () {
		float x = Input.GetAxis ("Horizontal");
		float y = Input.GetAxis ("Vertical");
		if (controlledObject != null)
			controlledObject.transform.Translate(new Vector3(x, 0, y));
	}
}

