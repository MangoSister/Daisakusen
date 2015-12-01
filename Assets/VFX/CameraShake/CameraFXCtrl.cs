using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CameraShake))]
public class CameraFXCtrl : MonoBehaviour
{
    public List<Camera> cameras;

    public float shakeIntensity;
    public float shakeFreq;
    public float shakeTime;

    public bool rgbSplitOn;
    public float rgbSplitIntensity = 0.02f;
    public Shader rgbSplitShader;

    private float _shakeScale;
   // private RGBSplitImgFX _split { get { return GetComponent<RGBSplitImgFX>(); } }
    private CameraShake _shake { get { return GetComponent<CameraShake>(); } }

    public void Shake(float scale)
    {
        _shakeScale = Mathf.Clamp01(scale);
        _shake.Shake(shakeIntensity * _shakeScale, shakeFreq * _shakeScale, shakeTime * _shakeScale);
    }

    private void Start()
    {
        foreach (var cam in cameras)
        {
            if (cam.GetComponent<RGBSplitImgFX>() == null)
            {
                var rgb = cam.gameObject.AddComponent<RGBSplitImgFX>();
                rgb.rgbSplitShader = rgbSplitShader;
            }
        }
        _shake.ShakeInProgress += OnShakeInProgress;
    }

    private void OnShakeInProgress(CameraShake shake, Vector3 offset)
    {
        if (rgbSplitOn)
        {
            foreach (var cam in cameras)
            {
                RGBSplitImgFX split = cam.GetComponent<RGBSplitImgFX>();
                if (split != null)
                    split.splitOffset = new Vector2(offset.x, offset.y) * rgbSplitIntensity * _shakeScale;
            }
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        Shake(0.5f);
    //    }
    //}
}
