using UnityEngine;
using System.Collections;

public class WallMgr : MonoBehaviour {

	void OnCollisionEnter(Collision co) {
		if(co.gameObject.CompareTag("Robot")) {
			BroadcastMessage("Show");
		}
	}

	void OnCollisionExit(Collision co) {
		if(co.gameObject.CompareTag("Robot")) {
			BroadcastMessage("Disappear");
		}
	}

}
