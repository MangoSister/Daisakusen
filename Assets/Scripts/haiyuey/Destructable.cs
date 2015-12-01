using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour {

	public float hp = 1.0f;

	public void GiveAttack(float damage) {
		hp -= damage;
		if (hp < 0) {
			hp = 0;
			SoundManager.Instance.sfxPlay(SFXType.BUILDING_EXPLOSION);
			Die ();
		}
	}

	void Die() {
		Destroy (gameObject);
	}

}
