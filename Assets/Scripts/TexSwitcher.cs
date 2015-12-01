using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TexSwitcher : MonoBehaviour
{
    public List<Texture2D> texes;
    private int _idx;

    private Material _mat
    {
        get
        {
            if (GetComponent<MeshRenderer>() != null)
                return GetComponent<MeshRenderer>().material;
            else return null;
        }
    }

    private void Awake()
    {
        ResetTex();
    }

    public void NextTex()
    {
        if (_mat != null)
        {
            _idx = _idx < texes.Count - 1 ? _idx + 1 : 0;
            if (texes != null && texes.Count > 0)
                _mat.mainTexture = texes[_idx];
        }
    }

    public void ResetTex()
    {
        _idx = 0;
        if (texes != null && texes.Count > 0)
            _mat.mainTexture = texes[_idx];
    }
}
