using UnityEngine;
using System.Collections;

public class EffectManager : GenericSingleton<EffectManager> {

	public CameraFXCtrl cfx;
	public GameObject camParent;

	private PoseFeedBack[] pfbControllers;


	void Start () {
		pfbControllers = camParent.GetComponentsInChildren<PoseFeedBack> ();
	}

	public void CameraEffects(float level) {
		cfx.Shake (level);
	}

	public void PoseFeedback(float flickTime, Color color, float maxOpaque) {
		for (int i = 0; i < pfbControllers.Length; i++) {
			if (pfbControllers[i].enabled) 
				pfbControllers[i].Flick(flickTime, color, maxOpaque);
		}
	}
}
