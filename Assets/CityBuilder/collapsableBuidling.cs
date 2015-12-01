using UnityEngine;
using System.Collections;

public class collapsableBuidling : MonoBehaviour {

	float height = 0.0f;

	public void SetCollapse(float _time, float _height){
		StartCoroutine ("activateDistruction", _time);
		height = _height;
	}

	public void Destroy(float _time){
		StartCoroutine ("Destory", _time);
	}
	
	IEnumerator activateDistruction(float _time){
		yield return new WaitForSeconds (_time);
		GameObject InstantEffect =  (GameObject)Instantiate(Resources.Load("ModelAsset/prefab/smoke effect"), new Vector3(transform.position.x, height * 0.9f , transform.position.z), Quaternion.identity );

		Quaternion buldingRot = Quaternion.AngleAxis(90, Vector3.left);
		int randomNum_rot = UnityEngine.Random.Range(0,8);
		buldingRot *= Quaternion.Euler(0, 0, 90 * randomNum_rot); // this add a 90 degrees Z rotation
		GameObject InstantPrefab =  (GameObject)Instantiate(Resources.Load("ModelAsset/prefab/Debris"), transform.position, buldingRot );

	}
	IEnumerator Destory(float _time){
		yield return new WaitForSeconds (_time);

		Destroy (gameObject);
	}
}
