using UnityEngine;
using System.Collections;

public class JetCtrl : MonoBehaviour {

	public Rigidbody rb;
	public float rotateAngle = 5.0f;
	public float rotateSpeed = 30.0f;


	Quaternion initialRot;
	Quaternion targetRot;

	void Start() {
		initialRot = transform.localRotation;
	}
	
	void FixedUpdate() {
		transform.localRotation = Quaternion.Lerp (transform.localRotation, targetRot, rotateSpeed * Time.fixedDeltaTime);
	}

	void Update() {
		targetRot = Quaternion.AngleAxis(
			rotateAngle * Mathf.Clamp01(Mathf.Abs(Vector3.Dot(rb.velocity.normalized, rb.transform.right.normalized))),
			Vector3.Cross(Vector3.up, transform.InverseTransformDirection(rb.velocity))) * initialRot;
	}
}
