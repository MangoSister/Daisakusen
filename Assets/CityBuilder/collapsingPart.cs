using UnityEngine;
using System.Collections;

//@ the part of building
public class collapsingPart : MonoBehaviour {

	public void SetFalling(float _time, Material _outerMtrl, Material _windowMtrl){

		MeshRenderer meshRender = this.GetComponent<MeshRenderer> ();
		Material[] materials = meshRender.sharedMaterials;

		if (materials.Length == 2) {
			materials [0] = _outerMtrl;
			materials [1] = _windowMtrl;
		} else if (materials.Length == 1) {
			materials [0] = _outerMtrl;
		} else {
			Debug.Log("Error giving color");
		}
		
		meshRender.sharedMaterials = materials;
		StartCoroutine ("activateTrigger", _time);
	}

	IEnumerator activateTrigger(float _time){
		yield return new WaitForSeconds (_time);
		this.GetComponent<BoxCollider> ().isTrigger = true;
	}


}
