using UnityEngine;
using System.Collections;

public class Tornado : MonoBehaviour {

	public int number;
	public GameObject stripePrefab;
	GameObject[] stripes;
	public Transform monster;

	public float scale {
		set {
			this.BroadcastMessage("SetScale", value);	
		}
	}

	void Start() {
		stripes = new GameObject[number];
		for (int i = 0; i < number; i++) {
			stripes[i] = GameObject.Instantiate(stripePrefab);
			stripes[i].transform.parent = transform;
            stripes[i].transform.localScale = new Vector3(1, 1, 1);
		}

		this.BroadcastMessage ("GetMonster", monster);

		
		for (int i = 0; i < number; i++) {
			stripes[i].SetActive(false);
		}
	}

	public void BeginWind() {
		for (int i = 0; i < number; i++) {
			stripes[i].SetActive(true);
		}
	}

	public void EndWind() {
		for (int i = 0; i < number; i++) {
			stripes[i].SetActive(false);
		}
	}
}
