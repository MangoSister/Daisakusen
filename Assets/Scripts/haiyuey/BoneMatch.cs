using UnityEngine;
using System.Collections;

public class BoneMatch : MonoBehaviour {
	public KinectInterop.JointType joint;
	public int playerIdx;
		
	// Update is called once per frame
	void Update () {
		Quaternion tempRot;
		if (InputProxy.Instance.GetBoneLocalRotation(joint, playerIdx, out tempRot)) {
			transform.localRotation = tempRot;
		}
	}
}
