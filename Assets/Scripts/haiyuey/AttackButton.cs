using UnityEngine;
using System.Collections;

public class AttackButton : MonoBehaviour {
	public string buttonName;
	public float velocityThresHold = 2.0f;
	public float attackThresHold = 2.0f;
	public float deadTime = 0.3f;

	public SpeedCalculator sc;

	bool attackPrepare = false;
	bool attacking = false;

	float lastAttackingTime = 0.0f;

	// Use this for initialization
	void Start () {
		attackPrepare = false;
		attacking = false;
		if (!sc) {
			sc = GetComponent<SpeedCalculator>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (attacking) {
			if (Time.time - lastAttackingTime > deadTime) {
				attackPrepare = false;
				attacking = false;
			}
		}

		if (sc) {
			if (sc.acceleration > attackThresHold) {
			//	SoundManager.Instance.sfxPlay(SFXType.ROBOT_PUNCH_FAKE);
				attackPrepare = true;
			}
			if (sc.acceleration < -attackThresHold) {
				attackPrepare = false;
			}
		}

		if (attackPrepare) {
			if (sc) {
				if (sc.velocity.magnitude > velocityThresHold) {
					if (!sc.towardSelf) {
						attacking = true;
						lastAttackingTime = Time.time;
					}
				}
			}
		}
        /*
		if (buttonName == "LeftHandAttack") {
			if (attackPrepare) {
				if (sc.velocity.magnitude > velocityThresHold) {
					if (!sc.towardSelf) {
						//Debug.Log("Attacking");
					}
				}
			}
		}
        */
	}

	public bool IsAttacking() {
		return attacking;
	}
}
