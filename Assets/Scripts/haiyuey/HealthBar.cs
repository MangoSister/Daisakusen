using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour {

	Slider bar;
	
	public float targetHealth;
	public float speed = 0.1f;

	// Use this for initialization
	void Start () {
		bar = GetComponent<Slider> ();	
		bar.value = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		bar.value = Mathf.Lerp (bar.value, targetHealth, speed);
	}
}
