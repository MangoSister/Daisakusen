using UnityEngine;
using System.Collections;

public class WallShower : MonoBehaviour {

	public float maxAlpha = 0.7f;
	public float fadeSpd = 3f;
	float targetAlpha = 0.0f;
	Renderer r;

	void Start() {
		r = GetComponent<Renderer> ();
	}


	void Update() {
		Color temp = r.material.color;
		temp.a = Mathf.Lerp (temp.a, targetAlpha, fadeSpd * Time.deltaTime);
		r.material.color = temp;
	}

	public void Show() {
		targetAlpha = maxAlpha;
	}

	public void Disappear() {
		targetAlpha = 0f;
	}
}
