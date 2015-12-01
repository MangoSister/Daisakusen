using UnityEngine;
using System.Collections;

public class SuperBlastFX : MonoBehaviour 
{

    public GameObject energyCylinder;
    public LineRenderer centerLineRenderer;
    public MeshRenderer rimRenderer;
    public MeshRenderer uvAnimRenderer;
    public MeshRenderer innerRenderer;
    public ParticleSystem outerParticleSys;
    public ParticleSystem contactParticleSys;

    public Vector3 centerLineStart
    {
        get
        { return _centerLineVertices[0]; }
        set
        {

            _centerLineVertices[0] = value;
            centerLineRenderer.SetPosition(0, _centerLineVertices[0]);
        }
    }

	public Vector3 centerLineEnd
    {
        get
        { return _centerLineVertices[1]; }
        set
        {
            _centerLineVertices[1] = value;
            centerLineRenderer.SetPosition(1, _centerLineVertices[1]);
        }
    }

    private Vector3[] _centerLineVertices;
    public float cylinderLength
    {
        set
        {
            energyCylinder.transform.localScale =
               new Vector3(energyCylinder.transform.localScale.x,
                            energyCylinder.transform.localScale.y, 
                            value);
        }
    }
	public float cylinderRadius
    {
        //get
        //{ return energyCylinder.transform.localScale.y; }
        set
        { energyCylinder.transform.localScale = 
                new Vector3(value, value, energyCylinder.transform.localScale.z);
        }
    }

	public float rimIntensity
    {
        get
        {
            return rimRenderer.material.GetFloat("_RimPower");
        }
        set
        { rimRenderer.material.SetFloat("_RimPower", value); }
    }

    public Vector2 uvTileScale
    {
        set
        {
            uvAnimRenderer.material.SetTextureScale("_MainTex", value);
        }
    }

    public Vector2 uvAnimSpeed
    {
        set
        {
            uvAnimRenderer.material.SetFloat("_AnimSpeedX", value.x);
            uvAnimRenderer.material.SetFloat("_AnimSpeedY", value.y);
        }
    }

	public float rimOpaqueness
    {
        get
        {
            return rimRenderer.material.GetColor("_Color").a;
        }
        set
        {
            Color rimCol = rimRenderer.material.GetColor("_Color");
            rimCol.a = value;
            rimRenderer.material.SetColor("_Color", rimCol);
        }
    }

    public float uvAnimOpaqueness
    {
        get
        {
            return uvAnimRenderer.material.GetColor("_Color").a;
        }
        set
        {
            Color uvAnimCol = uvAnimRenderer.material.GetColor("_Color");
            uvAnimCol.a = value;
            uvAnimRenderer.material.SetColor("_Color", uvAnimCol);
        }
    }

    public float innerOpaqueness
    {
        get
        {
           return innerRenderer.material.GetColor("_Color").a;
        }
        set
        {
            Color innerCol = innerRenderer.material.GetColor("_Color");
            innerCol.a = value;
            innerRenderer.material.SetColor("_Color", innerCol);
        }
    }

    private void Awake()
    {
        Restore();
    }

    public void Restore()
    {
        centerLineRenderer.SetVertexCount(2);
        _centerLineVertices = new Vector3[2] { Vector3.zero, Vector3.zero };
        centerLineRenderer.SetPosition(0, _centerLineVertices[0]);
        centerLineRenderer.SetPosition(1, _centerLineVertices[1]);

        centerLineRenderer.gameObject.SetActive(false);
        energyCylinder.SetActive(false);
        outerParticleSys.startLifetime = 0f; 
        outerParticleSys.gameObject.SetActive(false);
        contactParticleSys.startSize = 0f;
        contactParticleSys.gameObject.SetActive(false);
    }
}
