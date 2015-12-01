using UnityEngine;
using System.Collections;

public class InputProxy : GenericSingleton<InputProxy> {
	InputProxy() {}

	public bool usingKinect = false;
	public bool isEnable = true;
	
	public BodyJoystick body0;
	public BodyJoystick body1;
	public SteeringMonitor steerMoniter0;
	public SteeringMonitor steerMoniter1;

	public AttackButton[] abs;

	public float detachAngle = 60.0f;

	private bool attacking;

	private bool dodging = false;
	private Vector3 dodgeDirection;


	void Start() {
		attacking = false;
		dodging = false;
		dodgeDirection = Vector3.right;
	}

	void Update() {
		UpdateRecalibrate ();
	}

	public bool GetBoneLocalPosition (KinectInterop.JointType joint, int playerIdx, out Vector3 localPosition) {
		if (isEnable) {
			localPosition = BodyWrapper.Instance.GetBoneLocalPosition(joint, playerIdx);
			return true;
		} else {
			localPosition = Vector3.zero;
			return false;
		}
	}

	public bool GetBoneLocalRotation (KinectInterop.JointType joint, int playerIdx, out Quaternion localRotation) {
		if (isEnable) {
			localRotation =  BodyWrapper.Instance.GetBoneLocalRotation (joint, playerIdx);
			return true;
		} else {
			localRotation = Quaternion.identity;
			return false;
		}
	}

	public float GetMoveAxis(string name) {
		if (isEnable) {
			if (!usingKinect) {
				return Input.GetAxis (name);
			}
			else {
				float value = 0;
				int tot = 0;
				if ((body0) && (body0.IsTracked)) {
					tot ++;
					value += body0.GetAxis(name);
				}
				if ((body1) && (body1.IsTracked)) {
					tot ++;
					value += body1.GetAxis(name);
				}

				if (tot == 0) {
					return 0;
				}
				else {
					return value / tot;
				}
			}
		}
		return 0;
	}

	public float GetSteerAxis() {
		if (isEnable) {
			if (!usingKinect) {
				return Input.GetAxis ("Rotate");
			}
			else {
				float value = 0;
				int tot = 0;
				if ((steerMoniter0) && (steerMoniter0.calibrateCompleted)) {
					tot ++;
					value += steerMoniter0.GetSteeringAxis();
				}
				if ((steerMoniter1) && (steerMoniter1.calibrateCompleted)) {
					tot ++;
					value += steerMoniter0.GetSteeringAxis();
				}
				
				if (tot == 0) {
					return 0;
				}
				else {
					return value / tot;
				}
			}
		}
		return 0;
	}

	public bool GetAttackButton(string name) {
		if (isEnable) {
			for (int i = 0; i < abs.Length; i++) {
				if (name.Equals (abs [i].buttonName)) {
					return abs [i].IsAttacking ();
				}
			}
		}
		return false;
	}

	public bool GetAttacking() {
		attacking = false;
		if (isEnable) {
			for (int i = 0; i < abs.Length; i++) {
				if (abs [i].IsAttacking ()) {
					attacking = true;
				}
			}
		}
		return attacking;		
	}

	void UpdateRecalibrate() {
		if (Input.GetButtonDown("Recalibrate")) {
			body0.ReCalibrate();
			body1.ReCalibrate();
			steerMoniter0.ReCalibrate();
			steerMoniter1.ReCalibrate();
		}
	}

	public bool GetDodge(out Vector3 dodgeDir) {
		dodgeDir = Vector3.zero;
		if (!usingKinect) {
			if (Input.GetButtonDown ("Dodge")) {
				dodgeDir = Vector3.right;
				return true;
			}
		} else {
			if (dodging) {
				dodging = false;
				dodgeDir = dodgeDirection;
				return true;
			}
		}
		return false;
	}

	public bool GetDetach() {
		if (isEnable) {
			if ((body0) && (body0.IsTracked)) {
				if ((body1) && (body1.IsTracked)) {
					Vector3 stick0 = body0.GetVector();
					Vector3 stick1 = body1.GetVector();
					if (Vector3.Angle(stick0, stick1) > detachAngle) {
						return true;
					}
				}
			}
			//return Input.GetButtonDown("Detach");
		}
		return false;
	}

	public bool GetAttach() {
		if (isEnable) {
			return Input.GetButtonDown("Attach");
		}
		return false;
	}

	public void SetDodgeDirection(Vector3 dir) {
		dodging = true;
		dodgeDirection = dir;
	}
}
