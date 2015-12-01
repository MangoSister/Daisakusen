using UnityEngine;
using System.Collections;

public class DetachTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.LeftControl)) {
			BroadcastMessage("Detach");
		}
		if (Input.GetKeyDown(KeyCode.RightControl)) {
			BroadcastMessage("Attach");
		}
	}
}
