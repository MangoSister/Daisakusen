using UnityEngine;
using System.Collections;


public class DetachablePartCtrl : MonoBehaviour {

	Vector3 preLocalPos;
	Transform preParent;
	Quaternion preLocalRot;

	public bool detaching;
	public bool requireRot = true;

	Rigidbody rb;
	Collider co;
	bool trigger;

	public float backSpd = 10f;

	float thresHold = 0.001f;

	Coroutine cur;

	void Start() {
		co = GetComponent<Collider> ();
		trigger = false;
		if (co) 
			trigger = co.isTrigger;
		LightningMgr.Instance.CreateLightning (transform, transform.parent);
	}

	IEnumerator AttachBack() {
		transform.parent = preParent;
		Destroy (rb);
		rb = null;
		if (co)
			co.isTrigger = trigger;
		while (true) {
			transform.localPosition = Vector3.Lerp(transform.localPosition, preLocalPos, backSpd * Time.deltaTime);
			if (requireRot) {
				transform.localRotation = Quaternion.Lerp(transform.localRotation, preLocalRot, backSpd * Time.deltaTime);
			}
			if ((transform.localPosition - preLocalPos).magnitude < thresHold) {
				transform.localPosition = preLocalPos;
				transform.localRotation = preLocalRot;
				detaching = false;
				break;
			}
			yield return new WaitForFixedUpdate();
		}
	}

	public void Attach() {
		if (cur == null) {
			cur = StartCoroutine (AttachBack ());
		}
	}

	public void Detach() {

		StopAllCoroutines ();
		cur = null;
		if (co)
			co.isTrigger = false;
		if (!detaching) {
			preLocalPos = transform.localPosition;	
			preLocalRot = transform.localRotation;
		}
		if (transform.parent != GameController.Instance.robotTransform) {
			preParent = transform.parent;
			transform.parent = GameController.Instance.robotTransform;
		}
		if (rb == null) {
			rb = gameObject.AddComponent<Rigidbody> ();
		//	rb.velocity = transform.localPosition;
		}
		detaching = true;
	}


}
