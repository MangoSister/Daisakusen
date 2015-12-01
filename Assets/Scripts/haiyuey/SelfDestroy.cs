using UnityEngine;
using System.Collections;

public class SelfDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke ("DestroySelf", 2.0f);
	}

	void DestroySelf() {
		Destroy (gameObject);
	}

}