using UnityEngine;
using System.Collections;

public class BodyCtrl : MonoBehaviour {
	public float rotateAngle = 10.0f;
	public float rotateSpeed = 30.0f;

	Quaternion targetRot;

	void FixedUpdate() {
		transform.localRotation = Quaternion.Lerp (transform.localRotation, targetRot, rotateSpeed * Time.fixedDeltaTime);
	}

	public void UpdateVelocity(float angle, Vector3 newV, float rotateRate) {
		targetRot = Quaternion.AngleAxis (rotateRate * rotateAngle, Vector3.Cross (Vector3.up, transform.InverseTransformDirection(newV)));
	}
}
