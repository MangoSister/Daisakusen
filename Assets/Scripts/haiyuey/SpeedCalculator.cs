using UnityEngine;
using System.Collections;

public class SpeedCalculator : MonoBehaviour {

	public Vector3 velocity;
	public float acceleration;
	public Vector3 position;
	public bool towardSelf;

	Vector3 oldPos;
	Vector3 rootPos;
	Vector3 oldVelo;

	float scale = 100.0f;

	KinectManager manager;
	
	public KinectInterop.JointType target;
	KinectInterop.JointType root = KinectInterop.JointType.SpineMid;
	public int userIdx = 0;
	public float smoothFactor = 3.0f;
	private long userId;

	// Use this for initialization
	void Start () {
		manager = KinectManager.Instance;
		towardSelf = false;
	}
	
	// Update is called once per frame
	void Update () {
		GetLatestPosition ();
		CalculateVelocity ();
		CalculateAcceleration ();
	}

	void GetLatestPosition() {

		if(manager && manager.IsInitialized())
		{
			int upIdx = (int)target;
			int rootIdx = (int)root;
			
			if(manager.IsUserDetected())
			{
				userId = manager.GetUserIdByIndex(userIdx);
				
				if(manager.IsJointTracked(userId, upIdx))
				{
					Vector3 vPosJoint = manager.GetJointPosition(userId, upIdx) * scale;

					oldPos = position;
					position = Vector3.Lerp(oldPos, vPosJoint, smoothFactor * Time.deltaTime);
				}

				if(manager.IsJointTracked(userId, rootIdx))
				{
					rootPos = manager.GetJointPosition(userId, rootIdx) * scale;
					
				}

			}
		}
	}

	void CalculateVelocity() {
		oldVelo = velocity;
		if (Time.timeScale > 0) {
			velocity = (position - oldPos) / Time.timeScale;
		}
		Vector3 rootDir = rootPos - position;
		if (Vector3.Dot (rootDir, velocity) > 0.1f) {
			towardSelf = true;
		} else {
			towardSelf = false;
		}
	}

	void CalculateAcceleration() {
		acceleration = oldVelo.magnitude - velocity.magnitude;
	}
	
}
