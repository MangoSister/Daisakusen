using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class Building : MonoBehaviour {
	
	public string originName;
	[SerializeField] Material OuterMtrl; 
	[SerializeField] Material WindowMtrl;
	
	public void Initialzie(string _name, Material _outerMtrl, Material _windowMtrl){
		//For instanciating collapsed building, save orginal name
		originName = _name;
		
		//Give a various type of color;
		OuterMtrl = _outerMtrl;
		WindowMtrl = _windowMtrl;
	
		MeshRenderer meshRender = this.GetComponent<MeshRenderer> ();
		Material[] materials = meshRender.sharedMaterials;

		materials [0] = _outerMtrl;
		materials [1] = _windowMtrl;

		meshRender.sharedMaterials = materials;
	}
	
	public void GiveAttack(float _damage){
		//Connect with Rechard things;;
		string cloneBuildingName =  originName + "_Collapse";
		GameObject collapseClone = (GameObject)Instantiate (Resources.Load(cloneBuildingName), transform.position , Quaternion.identity);
		Transform[] ts = collapseClone.GetComponentsInChildren<Transform>();

        SoundManager.Instance.sfxPlay3D(SFXType.BUILDING_EXPLOSION, transform);

		float height = this.GetComponent<BoxCollider>().size.z;
		
		ts[0].GetComponent<collapsableBuidling>().SetCollapse(0.2f, height);
		ts[0].GetComponent<collapsableBuidling>().Destroy(2.0f);
		ts[1].GetComponent<collapsingPart>().SetFalling(0.4f, OuterMtrl, WindowMtrl);
		ts[2].GetComponent<collapsingPart>().SetFalling(0.6f, OuterMtrl, WindowMtrl);
		ts[3].GetComponent<collapsingPart>().SetFalling(0.8f, OuterMtrl, WindowMtrl);
		
		Destroy (gameObject);
	}

	void OnTriggerEnter(Collider coll) {
		/*
		if (coll.tag == "character") {
			Debug.Log ("Collide!!");
			string cloneBuildingName =  originName + "_Collapse";

		
			GameObject collapseClone = (GameObject)Instantiate (Resources.Load(cloneBuildingName), transform.position , Quaternion.identity);
			Transform[] ts = collapseClone.GetComponentsInChildren<Transform>();
			//collapseChildren = new GameObject[childs.Length];
			//Child Ordering
		//	Array.Sort(ts);

			float height = this.GetComponent<BoxCollider>().size.z;

			ts[0].GetComponent<collapsableBuidling>().SetCollapse(0.2f, height);
			ts[0].GetComponent<collapsableBuidling>().Destroy(2.0f);
			ts[1].GetComponent<collapsingPart>().SetFalling(0.4f, OuterMtrl, WindowMtrl);
			ts[2].GetComponent<collapsingPart>().SetFalling(0.6f, OuterMtrl, WindowMtrl);
			ts[3].GetComponent<collapsingPart>().SetFalling(0.8f, OuterMtrl, WindowMtrl);

			Destroy (gameObject);
		}
		*/

	}

}
