using UnityEngine;
using System.Collections;

[RequireComponent (typeof (LineRenderer))]
public class Lightning : MonoBehaviour {
	LineRenderer lr;
	public Transform start;
	public Transform end;
	public int maxPoint = 50;
	float disScaler = 50.0f;
	Vector3[] vertices;
	public float randomScale = 0.05f;
	float maxCooldown = 0.05f;
	bool waitForLightning = true;
	public float disappearThres = 10.0f;
	public bool working = false;

	// Use this for initialization
	void Start () {
		lr = GetComponent<LineRenderer> ();
		lr.material = GameController.Instance.lightningMat;
		lr.SetWidth (0.1f, 0.1f);
		vertices = new Vector3[maxPoint];
		lr.SetVertexCount (vertices.Length);
		vertices [0] = start.position;
		vertices [vertices.Length - 1] = end.position;
		RecursiveStrike (0, vertices.Length - 1, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (working) {
			float dis = (end.position - start.position).magnitude;
			if (lr.enabled) {
					vertices [0] = start.position;
					vertices [vertices.Length - 1] = end.position;
					RecursiveStrike (0, vertices.Length - 1, Random.value);
					UpdateLR ();
			
				if (dis > disappearThres) {
					lr.enabled = false;
				}
			}
		} else {
			if (lr.enabled) {
				lr.enabled = false;
			}
		}
	}

	public void Detach() {
		StopAllCoroutines ();
		working = true;
		lr.enabled = true;
		StartCoroutine (WaitTime ());
	}

	public void Attach() {
		StopAllCoroutines ();
		working = true;
		lr.enabled = true;
		StartCoroutine (WaitTime ());
	}

	IEnumerator WaitTime() {
		yield return new WaitForSeconds (1.0f);
		working = false;
	}

	IEnumerator CoolDown() {
		yield return new WaitForSeconds (Random.Range (0.0f, maxCooldown));
		waitForLightning = true;
	}

	void RecursiveStrike(int start, int end, float spd) {
		int mid = (start + end) >> 1;
		if (mid <= start) {
			return;
		}
		Vector3 temp = Vector3.zero;
		temp.x += Random.Range (-1.0f, 1.0f) * randomScale * Mathf.Pow((float)((end - start)), 1.0f);
		temp.y += Random.Range (-1.0f, 1.0f) * randomScale * Mathf.Pow((float)((end - start)), 1.0f);
		temp.z += Random.Range (-1.0f, 1.0f) * randomScale * Mathf.Pow((float)((end - start)), 1.0f);
		vertices [mid] = temp + (vertices [start] + vertices [end]) * 0.5f;
		RecursiveStrike (start, mid, spd);
		RecursiveStrike (mid, end, spd);
	}

	void UpdateLR() {
		for (int i = 0; i < vertices.Length; i++) {
			lr.SetPosition(i, vertices[i]);
		}
	}

}
