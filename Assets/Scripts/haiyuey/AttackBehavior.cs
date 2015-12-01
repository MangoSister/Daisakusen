using UnityEngine;
using System.Collections;

public class AttackBehavior : MonoBehaviour {

	public string buttonName;

	Renderer rder;


	// Use this for initialization
	void Start () {
		rder = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (InputProxy.Instance.GetAttackButton (buttonName)) {
			if (rder) {
				rder.material.color = Color.red;
			}
		} else {
			if (rder) {
				rder.material.color = Color.white;
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (InputProxy.Instance.GetAttackButton (buttonName)) {
			if (other.CompareTag ("building")) {
				other.SendMessage ("GiveAttack", GameController.Instance.attackPower * GameController.Instance.attackMultiplier);
				EffectManager.Instance.CameraEffects(0.1f);
				SoundManager.Instance.sfxPlay(SFXType.ROBOT_PUNCH);
			}
			if (other.attachedRigidbody) {
				if (other.attachedRigidbody.CompareTag ("monster")) {
					other.attachedRigidbody.SendMessage ("DealDamage", GameController.Instance.attackPower * GameController.Instance.attackMultiplier);
					GameController.Instance.hitFeedback.transform.position = other.bounds.ClosestPoint(transform.position);
					GameController.Instance.hitFeedback.Play();
					EffectManager.Instance.CameraEffects(0.2f);
					SoundManager.Instance.sfxPlay(SFXType.ROBOT_PUNCH);
			}
			}

		}
	}

}
