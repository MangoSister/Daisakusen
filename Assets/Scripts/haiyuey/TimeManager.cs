using UnityEngine;
using System.Collections;

public class TimeManager : GenericSingleton<TimeManager> {
	[Range(0.0f, 2.0f)]
	public float targetTimeScale = 1.0f;
	private float defaultFixedDelta;
	private float jumpThres = 0.005f;

	public float smoothFactor = 0.05f;
	// Use this for initialization
	void Start () {
		defaultFixedDelta = Time.fixedDeltaTime;
	}
	
	void Update() {
		if (Mathf.Abs(Time.timeScale - targetTimeScale) > jumpThres) {
			Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, smoothFactor);
			if (Mathf.Abs(Time.timeScale - targetTimeScale) <= jumpThres) {
				Time.timeScale = targetTimeScale;
			}
			Time.fixedDeltaTime = defaultFixedDelta * Time.timeScale;
		}
	}
}
