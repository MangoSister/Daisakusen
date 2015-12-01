using UnityEngine;
using System.Collections;

public class destorableParticle : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine ("Destroy");
	}

	private IEnumerator Destroy()
	{
		yield return new WaitForSeconds(GetComponent<ParticleSystem>().duration);
		Destroy(gameObject); 
	}

}
