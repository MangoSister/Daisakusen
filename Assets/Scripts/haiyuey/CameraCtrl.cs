using UnityEngine;
using System.Collections;

public class CameraCtrl : MonoBehaviour {
	public Transform target;
	public float defaultDistance = 15.0f;
	public float followSpeed = 20.0f;
	public Vector3 relativeDir;

	public enum CamState
	{
		normal,
		side,
		focus,
	}


	public LayerMask cameraCollision;

	private float curDis;

	// Use this for initialization
	void Start () {
		relativeDir = transform.position - target.position;
		relativeDir.Normalize ();
		curDis = defaultDistance;
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		if (Physics.Raycast (target.position, transform.position - target.position, out hit, defaultDistance, cameraCollision)) {
			Debug.DrawLine(target.position, hit.point);
			curDis = (hit.point - target.position).magnitude - 0.1f;
		} else {
			curDis = defaultDistance;
		}
	}

	void FixedUpdate() {
		UpdatePosition ();
	}

	void UpdatePosition() {
		Vector3 targetPos = target.TransformPoint(relativeDir * curDis);
		transform.position = Vector3.Lerp (transform.position, targetPos, followSpeed * Time.deltaTime);
		transform.LookAt (target.position);
	}
}
