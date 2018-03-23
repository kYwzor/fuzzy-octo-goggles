using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDeletionScript : MonoBehaviour {
	// Collider needs to be "Is Trigger"
	void OnTriggerEnter(Collider c){
		Destroy (gameObject);
	}
}
