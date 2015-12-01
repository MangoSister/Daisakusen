using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RGBSplitImgFX : MonoBehaviour
{
    public Shader rgbSplitShader;
    public Vector2 splitOffset;

    private Material _mat;
    public Material material
    {
        get
        {
            if (_mat == null)
            {
                _mat = new Material(rgbSplitShader);
                _mat.hideFlags = HideFlags.HideAndDontSave;
            }
            return _mat;
        }
    }

    private void Start()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
        if (!rgbSplitShader || !rgbSplitShader.isSupported)
            enabled = false;
    }

    
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (rgbSplitShader != null)
        {
            material.SetVector("_SplitOffset", new Vector4(splitOffset.x, splitOffset.y, 0f, 0f));
            Graphics.Blit(src, dest, material);
        }
        else Graphics.Blit(src, dest);
    }

    private void OnDisable()
    {
        if (_mat)
            DestroyImmediate(_mat);
    }
}
