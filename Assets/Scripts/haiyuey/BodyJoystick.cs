using UnityEngine;
using System.Collections;

public class BodyJoystick : MonoBehaviour {
	public KinectInterop.JointType stickHead;
	public KinectInterop.JointType stickFoot;

	public float thresHold = 5.0f;
	public float maxAngle = 20.0f;

	public float smoothFactor = 3.0f;
	public int userIdx = 0;
	
	private bool tracked;
	public bool IsTracked { get{ return tracked; } }

	private long userId = 0;
	private KinectManager manager;

	private Vector3 axisDir;
	private Vector3 upPos;
	private Vector3 downPos;
	private Vector3 initialUp;




	// Use this for initialization
	void Start () {
		manager = KinectManager.Instance;
		initialUp = Vector3.up;
		axisDir = Vector3.zero; 
		tracked = false;
	}
	
	// Update is called once per frame
	void Update () {
		tracked = false;
		GetLatestPosition ();
		CalculateDir ();
	}

	void GetLatestPosition() {
		bool trackedUp = false;
		bool trackedDown = false;
		if(manager && manager.IsInitialized())
		{
			int upIdx = (int)stickHead;
			int downIdx = (int)stickFoot;
			
			if(manager.IsUserDetected())
			{
				userId = manager.GetUserIdByIndex(userIdx);
				if (userId == 0) {
					tracked = false;
				}
				else {
					tracked = true;
				}

				if(manager.IsJointTracked(userId, upIdx))
				{
					Vector3 vPosJoint = manager.GetJointPosition(userId, upIdx);

					trackedUp = true;
					upPos = Vector3.Lerp(upPos, vPosJoint, smoothFactor * Time.deltaTime);
				}
				
				if(manager.IsJointTracked(userId, downIdx))
				{
					Vector3 vPosJoint = manager.GetJointPosition(userId, downIdx);

					trackedDown = true;
					downPos = Vector3.Lerp(downPos, vPosJoint, smoothFactor * Time.deltaTime);
				}
			}
		}

		if (trackedUp && trackedDown) {
			tracked = true;
		}
	}

	public Vector3 GetVector() {
		return (upPos - downPos).normalized;
	}

	void CalculateDir() {
		if (tracked) {
			Vector3 dirNow = upPos - downPos;
			float angle = Vector3.Angle (dirNow, initialUp);
			float rate = 0;
			if (angle > thresHold) {
				rate = Mathf.Clamp01 (angle / maxAngle);
			}

			dirNow.y = 0;
			if (rate == 0) {
				axisDir = Vector3.zero;
			} else {
				axisDir = dirNow.normalized * rate;
			}
		} else {
			axisDir = Vector3.zero;
		}
	}

	public void ReCalibrate() {
		initialUp = upPos - downPos;
	}

	public float GetAxis(string name) {
		if (name.Equals ("Horizontal")) {
			return axisDir.x;
		}

		if (name.Equals ("Vertical")) {
			return -axisDir.z;
		}

		return 0;
	}
}
