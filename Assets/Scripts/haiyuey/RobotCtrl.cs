using UnityEngine;
using System.Collections;

public class RobotCtrl : MonoBehaviour {

	public Transform ground;
	public float height;
	public RobotState state;

	public float health = 100.0f;

	public HealthBar bar;

	public float range = 125.0f;

	public float healthDropRate = 3.0f;
	public float detachThreshold = 20.0f;
	public float attachRecovery = 25.0f;

	public float maxSpeed = 30.0f;
	public float acceleration = 30.0f;
	public float dampingSpd = 3.5f;
	public float rotateSpd = 60.0f;
	Rigidbody rb;
	public BodyCtrl robotBody;


	float thresHold = 0.1f;
	float velocityThresHold = 0.5f;
	float rotationThresHold = 0.2f;
	float angleY = 0;

	public Transform focus;
	private float followFocusSpeed = 0.3f;

	public Vector3 dodgeDir;
	public float beginDodgingVelocity = 3000.0f;
	public float dodgeDis = 30.0f;

	public Vector3 damageDir;
	public float damagePower = 1000.0f;
	public float damageDis = 80.0f;

	float curDodgeDis = 0;
	float curDamageDis = 0;
	Vector3 preDodgePos;
	Vector3 preDamagePos;
	float endDodgeThresHold = 0.5f;
	float endDamageThresHold = 0.5f;


	private Vector3 oldVelo;
	private Vector3 newVelo;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		dodgeDir = Vector3.zero;
		damageDir = Vector3.zero;
		preDodgePos = transform.position;
		preDamagePos = transform.position;

		oldVelo = Vector3.zero;
		newVelo = Vector3.zero;
	}

	public void Begin() {
		DetachBehavior ();
	}

	// Update is called once per frame
	void FixedUpdate () {

		if (bar) {
			bar.targetHealth = Mathf.Clamp01(health / 100.0f);
		}

		if (transform.position.y < ground.position.y + height) {
			Vector3 tempPos = transform.position;
			tempPos.y = ground.position.y + height;
			transform.position = tempPos;
			if (rb.velocity.y <=0) {
				Vector3 tempV = rb.velocity;
				tempV.y = 0;
				rb.velocity = tempV;
			}
		}

		if (transform.position.y > ground.position.y + 2 * height) {
			Vector3 tempPos = transform.position;
			tempPos.y = ground.position.y + 2 * height;
			transform.position = tempPos;
			if (rb.velocity.y >=0) {
				Vector3 tempV = rb.velocity;
				tempV.y = 0;
				rb.velocity = tempV;
			}
		}

		if (Mathf.Abs (transform.position.x) >= range) {
			Vector3 tempPos = transform.position;
			tempPos.x = Mathf.Sign(transform.position.x) * range;
			transform.position = tempPos;
			if (rb.velocity.x * Mathf.Sign(transform.position.x) > 0) {
				Vector3 tempV = rb.velocity;
				tempV.x = 0;
				rb.velocity = tempV;
			}
		}
		if (Mathf.Abs (transform.position.z) >= range) {
			Vector3 tempPos = transform.position;
			tempPos.z = Mathf.Sign(transform.position.z) * range;
			transform.position = tempPos;
			if (rb.velocity.z * Mathf.Sign(transform.position.z) > 0) {
				Vector3 tempV = rb.velocity;
				tempV.z = 0;
				rb.velocity = tempV;
			}
		}


		switch (state) {
		case RobotState.idle: 
			Idle ();
			UpdateMovement (); 
			UpdateRotation ();
			UpdateBody ();
			break;
		case RobotState.moving: 
			Moving ();
			UpdateMovement (); 
			UpdateRotation ();
			UpdateBody ();
			break;
		case RobotState.attacking: 
			Attacking ();
			UpdateMovement (); 
			UpdateBody ();
			if (focus) {
				UpdateRotation ();
			}
			break;
		case RobotState.dodging:
			UpdateDodging ();
			UpdateBody ();
			break;
		case RobotState.detached:
			Detaching ();
			break;
		case RobotState.skill:
			UpdateBody ();
			break;
		case RobotState.damaged:
			UpdateDamaged();
			UpdateBody();
			break;
        case RobotState.dead:
            break;
        }
	}

	public void GetHealth(float value) {
		health += value;
		if (health > 100.0f) {
			health = 100.0f;
		}
	}

	void Detaching() {
		if (InputProxy.Instance.GetAttach ()) {
			BroadcastMessage ("Attach");
			SoundManager.Instance.sfxPlay(SFXType.ROBOT_CONNECT);
			GameController.Instance.ma.targetActive = true;
			rb.constraints = RigidbodyConstraints.FreezeRotation;
			if (health <= detachThreshold) {
				health += attachRecovery;
				if (health > 100.0f) {
					health = 100.0f;
				}
			}
			state = RobotState.idle;
		} else {
			if (health <= detachThreshold) {
				health -= healthDropRate * Time.deltaTime;
				if (health <= 0) {
                    state = RobotState.dead;
					GameController.Instance.GameEnding(false);
					health = 0;
				}
			}
		}
	}

	void Idle() {
		if (InputProxy.Instance.GetDodge (out dodgeDir)) {
			preDodgePos = transform.position;
			curDodgeDis = 0;
			state = RobotState.dodging;
			return;
		}
		if (InputProxy.Instance.GetAttacking ()) {
			state = RobotState.attacking;
			return;
		}

		if (rb.velocity.magnitude > velocityThresHold) {
			state =	RobotState.moving;
		}
	}

	void Moving() {
		if (InputProxy.Instance.GetDodge (out dodgeDir)) {
			preDodgePos = transform.position;
			curDodgeDis = 0;
			state = RobotState.dodging;
			return;
		}
		if (InputProxy.Instance.GetAttacking ()) {
			state = RobotState.attacking;
			return;
		}
		
		if (rb.velocity.magnitude <= velocityThresHold) {
			state =	RobotState.idle;
		}
	}

	void Attacking() {
		if (InputProxy.Instance.GetDodge (out dodgeDir)) {
			preDodgePos = transform.position;
			curDodgeDis = 0;
			state = RobotState.dodging;
			return;
		}
		if (!InputProxy.Instance.GetAttacking ()) {
			if (rb.velocity.magnitude > velocityThresHold) {
				state = RobotState.moving;
			}
			else {
				state = RobotState.idle;
			}
		}
	}

	void UpdateDodging() {
		oldVelo = newVelo;
		newVelo = rb.velocity;
		curDodgeDis += (transform.position - preDodgePos).magnitude;
		preDodgePos = transform.position;
		float factor = 1.0f - (curDodgeDis / dodgeDis);
		if (factor < endDodgeThresHold) {
			state = RobotState.idle;
			return;
		}
		Vector3 curVelo = transform.TransformDirection(dodgeDir).normalized * beginDodgingVelocity * Time.deltaTime * factor;
		rb.velocity = curVelo;

		if (focus) {
			Vector3 lookDir = transform.forward;
			lookDir.y = 0;
			lookDir.Normalize();
			Vector3 targetDir = (focus.position - transform.position).normalized;
			targetDir.y = 0;
			lookDir.Normalize();
			Vector3 curDir = Vector3.Lerp(lookDir, targetDir, followFocusSpeed);
			curDir.y = 0;
			curDir.Normalize();
			transform.LookAt(transform.position + curDir);
		}
	}

	void UpdateDamaged() {
		oldVelo = newVelo;
		newVelo = rb.velocity;
		curDamageDis += (transform.position - preDamagePos).magnitude;
		preDamagePos = transform.position;
		float factor = 1.0f - (curDamageDis / damageDis);
		if (factor < endDamageThresHold) {
			state = RobotState.idle;
			return;
		}
		Vector3 curVelo = damageDir.normalized * beginDodgingVelocity * Time.deltaTime * factor;
		rb.velocity = curVelo;
		
		if (focus) {
			Vector3 lookDir = transform.forward;
			lookDir.y = 0;
			lookDir.Normalize();
			Vector3 targetDir = (focus.position - transform.position).normalized;
			targetDir.y = 0;
			lookDir.Normalize();
			Vector3 curDir = Vector3.Lerp(lookDir, targetDir, followFocusSpeed);
			curDir.y = 0;
			curDir.Normalize();
			transform.LookAt(transform.position + curDir);
		}
	}

	void DetachBehavior() {
		state = RobotState.detached;
		SoundManager.Instance.sfxPlay(SFXType.ROBOT_DISCONNECT);
		BroadcastMessage("Detach");
		rb.velocity = Vector3.zero;
		rb.constraints = RigidbodyConstraints.FreezeAll;
		GameController.Instance.ma.targetActive = false;
	}

	void UpdateMovement() {
		
		if (InputProxy.Instance.GetDetach ()) {
			DetachBehavior();
			return;
		}

		oldVelo = newVelo;
		newVelo = rb.velocity;
		float x = InputProxy.Instance.GetMoveAxis ("Horizontal");
		float y = InputProxy.Instance.GetMoveAxis ("Vertical");
		Vector3 force = transform.TransformDirection(new Vector3 (x, 0, y)) * acceleration;


		Vector3 spdNow = rb.velocity;
		spdNow.y = 0;

		if (force.magnitude > thresHold) {
			rb.AddForce (force, ForceMode.Acceleration);
			if ( spdNow.magnitude > maxSpeed) {
				rb.velocity = rb.velocity.normalized * maxSpeed;
			}
		} else {
			rb.AddForce (-rb.velocity * dampingSpd, ForceMode.Acceleration);
		}
	}

	public void SetFocus(Transform newFocus) {
		focus = newFocus;
	}

	public void ReleaseFocus() {
		angleY = Mathf.Repeat (transform.rotation.eulerAngles.y, 360.0f);
		focus = null;
	}

	void UpdateRotation() {
		if (!focus) {
			float steer = InputProxy.Instance.GetSteerAxis ();
			if (Mathf.Abs (steer) > rotationThresHold) {
				angleY = Mathf.Repeat (angleY + steer * rotateSpd * Time.deltaTime, 360.0f);
				Vector3 curRot = transform.rotation.eulerAngles;
				curRot.y = angleY;
				transform.rotation = Quaternion.Euler (curRot);
			}
		} else {
			Vector3 lookDir = transform.forward;
			lookDir.y = 0;
			lookDir.Normalize();
			Vector3 targetDir = (focus.position - transform.position).normalized;
			targetDir.y = 0;
			lookDir.Normalize();
			Vector3 curDir = Vector3.Lerp(lookDir, targetDir, followFocusSpeed).normalized;
			curDir.y = 0;
			curDir.Normalize();
			transform.LookAt(transform.position + curDir);
		}
	}

	void UpdateBody() {
		Vector3 spdNow = rb.velocity;
		spdNow.y = 0;
		
		if (spdNow.magnitude > velocityThresHold) {
			RotateBody (rb.velocity, rb.velocity.magnitude / maxSpeed);
		} else {
			RotateBody (Vector3.zero, 0);
		}
	}

	void RotateBody(Vector3 velo, float vRate) {
		robotBody.UpdateVelocity (Vector3.Angle(transform.forward, velo), velo, vRate);
	}

	public void BeginSkill() {
		state = RobotState.skill;
	}

	public void EndSkill() {
		state = RobotState.idle;
	}

	void OnCollisionEnter(Collision c) {
		if (c.gameObject.layer == LayerMask.NameToLayer ("Wall")) {
			if ((state == RobotState.dodging) || (state == RobotState.damaged)) {
				state = RobotState.idle;
			}
		}

		
		if ((state != RobotState.detached) && (state != RobotState.skill)) {
		if (c.gameObject.CompareTag("building")) {
			c.gameObject.SendMessage ("GiveAttack", GameController.Instance.attackPower * GameController.Instance.attackMultiplier);
			EffectManager.Instance.CameraEffects(0.1f);
			if ((state == RobotState.dodging) || (state == RobotState.damaged)) {
				rb.velocity = oldVelo;
			}
		}

		if (state != RobotState.damaged) {
				if (c.collider.gameObject.CompareTag ("normalAtk")) {
					preDamagePos = transform.position;
					curDamageDis = 0;
					damageDis = 10.0f;
					damageDir = (gameObject.transform.position - c.collider.attachedRigidbody.transform.position).normalized;
					damageDir.y = 0;
					damageDir.Normalize();
					state = RobotState.damaged;
					EffectManager.Instance.CameraEffects(0.3f);
					SoundManager.Instance.sfxPlay(SFXType.ROBOT_HURT);
					DamagePower dp = c.collider.gameObject.GetComponent<DamagePower> ();
					if (PoseManager.Instance != null)
						PoseManager.Instance.CancelCurrDodgePose();
					if (dp) {
						health -= dp.damagePower;
						if (health <= detachThreshold) {
							DetachBehavior ();
						}
					}
				}

				if (c.collider.gameObject.CompareTag ("specialAtk")) {
					preDamagePos = transform.position;
					curDamageDis = 0;
					damageDis = 80.0f;
					damageDir = (gameObject.transform.position - c.collider.attachedRigidbody.transform.position).normalized;
					damageDir.y = 0;
					damageDir.Normalize();
					state = RobotState.damaged;
					EffectManager.Instance.CameraEffects(0.3f);
					SoundManager.Instance.sfxPlay(SFXType.ROBOT_HURT);
					DamagePower dp = c.collider.gameObject.GetComponent<DamagePower> ();
					if (dp) {
						health -= dp.damagePower;
						if (health <= detachThreshold) {
							DetachBehavior ();
						}
					}
				}
			}
		}

	}



}
