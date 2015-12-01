using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public delegate void ShakeHandler(CameraShake sender, Vector3 offset);
    public ShakeHandler ShakeInProgress;
    private Vector3 _beforeShakeLocalPos;
    private bool _isShaking;
    private Coroutine _currShakingCoroutine;
    private void Start()
    {
        _isShaking = false;
    }

    public void Shake(float intensity, float freq, float time)
    {
        time = Mathf.Max(0f, time);
        if (!_isShaking)
            _beforeShakeLocalPos = transform.localPosition;
        if (_currShakingCoroutine != null)
            StopCoroutine(_currShakingCoroutine);
        _currShakingCoroutine = StartCoroutine(ShakeCoroutine(intensity, freq, time));
    }

    private IEnumerator ShakeCoroutine(float intensity, float freq, float time)
    {
        _isShaking = true;
        float startTime = Time.time;
        float currTime = startTime;

        Vector3 seed = new Vector3(Random.value, Random.value, Random.value);
        while (currTime - startTime < time)
        {
            //cos damper
            float damper = Mathf.Cos(0.5f * Mathf.PI * Mathf.Clamp01((currTime - startTime) / time));
            Vector3 offset = new Vector3
                (
                    2f * Mathf.PerlinNoise(seed.x, seed.x) - 1f,
                    2f * Mathf.PerlinNoise(seed.y, seed.y) - 1f,
                    2f * Mathf.PerlinNoise(seed.z, seed.z) - 1f
                );
            offset = offset.normalized * intensity * damper;
            transform.localPosition = _beforeShakeLocalPos + offset;
            seed += Vector3.one * Time.deltaTime * freq;
            currTime = Time.time;

            if (ShakeInProgress != null)
                ShakeInProgress(this, offset); 

            yield return null;
        }

        transform.localPosition = _beforeShakeLocalPos;
        if (ShakeInProgress != null)
            ShakeInProgress(this, Vector3.zero);
        _isShaking = false;
    }
}
