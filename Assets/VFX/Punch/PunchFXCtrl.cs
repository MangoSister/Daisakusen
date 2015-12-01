using UnityEngine;
using System.Collections;

public class PunchFXCtrl : MonoBehaviour
{
    private ParticleSystem _particleSys { get { return GetComponent<ParticleSystem>(); } }
    public void PlayFX()
    {
        if (_particleSys.isPlaying)
            _particleSys.Stop();
        _particleSys.Play();
    }
}
