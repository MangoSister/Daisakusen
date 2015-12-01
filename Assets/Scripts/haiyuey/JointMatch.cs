using UnityEngine;
using System.Collections;

public class JointMatch : MonoBehaviour {
	
	public KinectInterop.JointType target;
	public KinectInterop.JointType parent;
	public float scale = 2.0f;
	public float smoothFactor = 5.0f;
	public int userIdx = 0;
	
	private long userId;
	KinectManager manager;

	Vector3 posNow;
	Quaternion rotNow;

	// Use this for initialization
	void Start () {
		manager = KinectManager.Instance;
		rotNow = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update () {
		UpdatePos();
        if (Input.GetKeyDown(KeyCode.L))
        {
            Flip();
        }
	}

    void Flip()
    {
        userIdx = (userIdx + 1) & 1;
    }

	void FixedUpdate() {
		//transform.localPosition = Vector3.Lerp (transform.localPosition, posNow, smoothFactor * Time.deltaTime);
		transform.localRotation = Quaternion.Lerp(transform.localRotation, rotNow, smoothFactor * Time.deltaTime);		
	}

	void UpdatePos () {
		if (InputProxy.Instance.isEnable) {
			Quaternion targetRot = Quaternion.identity;
			Quaternion parentRot = Quaternion.identity;

			if (manager && manager.IsInitialized ()) {
				int upIdx = (int)target;
				int downIdx = (int)parent;
			
				if (manager.IsUserDetected ()) {
					userId = manager.GetUserIdByIndex (userIdx);
					if (manager.IsJointTracked (userId, upIdx)) {
						targetRot = manager.GetJointOrientation (userId, upIdx, true);
					}
				
					if (manager.IsJointTracked (userId, downIdx)) {
						parentRot = manager.GetJointOrientation (userId, downIdx, true);
					}
				}
			}
			rotNow = Quaternion.Inverse (parentRot) * targetRot;
		}
	}
}
