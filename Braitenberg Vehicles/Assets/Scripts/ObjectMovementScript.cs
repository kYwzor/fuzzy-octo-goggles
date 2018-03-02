using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovementScript : MonoBehaviour {
	// Update is called once per frame
	public GameObject controlledObject;
	void Update () {
		float x = Input.GetAxis ("Horizontal");
		float y = Input.GetAxis ("Vertical");
		if (controlledObject != null)
			controlledObject.transform.Translate(new Vector3(x, 0, y));
	}
}
