using UnityEngine;
using System.Collections;

public class LightningMgr : GenericSingleton<LightningMgr> {
	public void CreateLightning(Transform startPos, Transform endPos) {
		GameObject temp = new GameObject ();
		temp.name = "Lightning";
		temp.transform.parent = transform;
		temp.transform.localPosition = Vector3.zero;
		temp.transform.localRotation = Quaternion.identity;
		Lightning l = temp.AddComponent<Lightning> ();
		l.start = startPos;
		l.end = endPos;
	}
}
