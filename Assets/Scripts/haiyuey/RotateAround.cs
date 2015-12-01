using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
public class RotateAround : MonoBehaviour {

	public Transform body;
	public float scale = 6;
	float angle = 0;
	float height = 0;
	float range = 0;
	float spd = 1.0f;

	// Use this for initialization
	void Start () {
		TrailRenderer tr = GetComponent<TrailRenderer> ();
		tr.startWidth = 0.7f;
		tr.endWidth = 0.1f;
		tr.time = 0.5f;
		tr.material = GameController.Instance.windMat;
		angle = Random.Range (0f, 360.0f);
		height = Random.Range (0.0f, 16.0f);
		range = Random.Range (1.0f, 1.2f) * scale;
		spd = Random.Range (500f, 1000f);
		Vector3 pos = Quaternion.Euler(0, angle, 0) * Vector3.forward;
		pos = pos.normalized * range;
		pos.y = height;
		if (body) 
			transform.position = body.position + pos;
		else 
			transform.position = pos;
	}

	public void SetScale(float newScale) {
		scale = newScale;
		range = Random.Range (1.0f, 1.2f) * scale;
	}

	// Update is called once per frame
	void Update () {
		angle = Mathf.Repeat (angle + spd * Time.deltaTime, 360.0f);
	}

	void FixedUpdate() {
		Vector3 pos = Quaternion.Euler(0, angle, 0) * Vector3.forward;
		pos = pos.normalized * range;
		pos.y = height;
		if (body) 
			transform.position = body.position + pos;
		else 
			transform.position = pos;
	}

	public void GetMonster(Transform monster) {
		body = monster;
	}
}
