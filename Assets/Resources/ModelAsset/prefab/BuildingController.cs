using UnityEngine;
using System.Collections;

public class BuildingController : MonoBehaviour {

	public string originName;
	GameObject[] collapseChildren;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetOriginName(string _name){
		originName = _name;
	}

	void OnTriggerEnter(Collider coll) {
		if (coll.tag == "character") {
			Debug.Log ("Collide!!");
			string cloneBuildingName =  originName + "_Collapse";

		
			GameObject collapseClone = (GameObject)Instantiate (Resources.Load(cloneBuildingName), transform.position, Quaternion.identity);

			BoxCollider[] childs = collapseClone.transform.GetComponentsInChildren<BoxCollider>();
			//collapseChildren = new GameObject[childs.Length];
			//Child Ordering
			for(int i =0; i < childs.Length; ++i){
				childs[i].isTrigger = true;
				
			}

			Destroy (gameObject);
		}
	}

	//GiveAttacked(float )
}
