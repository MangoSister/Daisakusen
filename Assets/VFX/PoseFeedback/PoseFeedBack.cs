using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PoseFeedBack : MonoBehaviour
{
    public Shader screenSoftFrameShader;
    public Texture2D maskTex;

    private Color _colorTint;
    private float _opaqueness;

    private Material _mat;
    public Material material
    {
        get
        {
            if (_mat == null)
            {
                _mat = new Material(screenSoftFrameShader);
                _mat.SetTexture("_MaskTex", maskTex);
                _mat.hideFlags = HideFlags.HideAndDontSave;
            }
            return _mat;
        }
    }

    private Coroutine _currCoroutine;

    private void Start()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
        if (!screenSoftFrameShader || !screenSoftFrameShader.isSupported)
            enabled = false;

        _opaqueness = 0f;
        _colorTint = Color.white;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (screenSoftFrameShader != null)
        {
            material.SetFloat("_Opaqueness", _opaqueness);
            _mat.SetColor("_Color", _colorTint);
            Graphics.Blit(src, dest, material);
        }
        else Graphics.Blit(src, dest);
    }

    private void OnDisable()
    {
        if (_mat)
            DestroyImmediate(_mat);
    }

    public void Flick(float flickTime, Color color, float maxOpaque)
    {
        if (_currCoroutine != null)
            StopCoroutine(_currCoroutine);
        _currCoroutine = StartCoroutine(FlickCoroutine(flickTime, color, maxOpaque));
    }

    private IEnumerator FlickCoroutine(float flickTime, Color color, float maxOpaque)
    {
        _colorTint = color;
        float timer = 0f;
        while (timer < flickTime)
        {
            _opaqueness = Mathf.Lerp(0, maxOpaque, Mathf.Clamp01(timer / flickTime));
            timer += Time.deltaTime;
            yield return null;
        }
        _opaqueness = maxOpaque;
        timer = 0f;
        while (timer < flickTime)
        {
            _opaqueness = Mathf.Lerp(maxOpaque, 0, Mathf.Clamp01(timer / flickTime));
            timer += Time.deltaTime;
            yield return null;
        }
        _opaqueness = 0f;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.U))
    //    {
    //        Flick(0.1f, Color.blue, 0.5f);
    //    }
    //}
}
