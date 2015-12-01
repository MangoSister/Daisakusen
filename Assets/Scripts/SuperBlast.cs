using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuperBlast : MonoBehaviour 
{
	//stage 1
	public float launchSpeed;
	public float intervalTime;

    //stage 2
	public float blastSize;
	public float blastTime;
    public Vector3 blastPID = new Vector3(0.3f, 2f, 0.003f);
    public Vector2 texTileFactor;
    public Vector2 animSpeed;
    public float startScaleCorrection;
    public float startOffset;
    //stage 3
    public float fadeTime;

    private bool _causeDamage;
    private List<ParticleSystem> _contactParticles;

    private SuperBlastFX _fx
    {
        get { return GetComponent<SuperBlastFX>(); }
    }

    public Collider blastCollider;

    // Use this for initialization
    private void Awake()
    {
        _causeDamage = false;
        _contactParticles = new List<ParticleSystem>();
    }

	public void Launch(Vector3 target)
	{
        StartCoroutine(LaunchCoroutine(target));
	}

    public IEnumerator LaunchCoroutine(Vector3 target)
    {
		//sound
		if(SoundManager.Instance != null)
			SoundManager.Instance.sfxPlay(SFXType.ROBOT_SPECIAL_ATK);

        _causeDamage = true;

        _fx.centerLineStart = transform.position;
        _fx.centerLineEnd = transform.position;

        transform.LookAt(target);
        float dist = Vector3.Distance(transform.position, target);

        //stage 1: launch
        _fx.centerLineRenderer.gameObject.SetActive(true);
        _fx.outerParticleSys.gameObject.SetActive(true);
        _fx.outerParticleSys.startLifetime = dist / 100f; //hard code
        _fx.outerParticleSys.gameObject.transform.localScale =
            new Vector3(blastSize, _fx.outerParticleSys.gameObject.transform.localScale.y, blastSize);
        //_fx.startParticleSys.sh
        if(!_fx.outerParticleSys.isPlaying)
            _fx.outerParticleSys.Play();
        float timer = 0f;
        while (timer < dist / launchSpeed)
        {
            timer += Time.deltaTime;
            _fx.centerLineEnd = Vector3.Lerp(transform.position, target,
                Mathf.Clamp01(timer / (dist / launchSpeed)));
            yield return null;
        }

        //stage 1: wait
        yield return new WaitForSeconds(intervalTime);

        //stage 2
        //call effectmanager if possible
        if (EffectManager.Instance != null)
            EffectManager.Instance.CameraEffects(1.0f);

        _fx.centerLineRenderer.gameObject.SetActive(false);
        _fx.energyCylinder.transform.position = 0.5f * (transform.position + target);
        _fx.cylinderLength = dist * 0.5f;
        _fx.cylinderRadius = 0f;
        _fx.uvTileScale = new Vector2(blastSize / texTileFactor.x, dist / texTileFactor.y);
        _fx.uvAnimSpeed = animSpeed;
        _fx.energyCylinder.SetActive(true);
        _fx.contactParticleSys.gameObject.transform.position += (target - transform.position).normalized * startOffset;
        _fx.contactParticleSys.startSize = blastSize * startScaleCorrection;
        _fx.contactParticleSys.gameObject.SetActive(true);
        if (!_fx.contactParticleSys.isPlaying)
            _fx.contactParticleSys.Play();

        GenerateContacts(target);

        float prevError = 0f;
        float integral = 0f;
        float radius = 0f;
        float kp = blastPID.x, ki = blastPID.y, kd = blastPID.z;
        timer = 0f;
        while (timer < blastTime)
        {
            float error = blastSize - radius;
            integral += error * Time.deltaTime;
            float derivative = (error - prevError) / Time.deltaTime;
            radius = kp * error + ki * integral + kd * derivative;
            prevError = error;
            _fx.cylinderRadius = Mathf.Max(0f, radius);
           
            timer += Time.deltaTime;
            yield return null;
        }
        //previous_error = 0
        //integral = 0
        //start:
        //    error = setpoint - measured_value
        //    integral = integral + error * dt
        //    derivative = (error - previous_error) / dt
        //    output = Kp * error + Ki * integral + Kd * derivative
        //    previous_error = error
        //    wait(dt)
        //    goto start

        //stage 3 : fade/disappear

        _causeDamage = false;
        if (_contactParticles != null)
        {
            foreach (var par in _contactParticles)
                Destroy(par.gameObject);
            _contactParticles.Clear();
        }
        _fx.outerParticleSys.gameObject.SetActive(false);
        _fx.contactParticleSys.gameObject.SetActive(false);
        _fx.centerLineRenderer.gameObject.SetActive(false);
        timer = 0f;
        float rimOpaque = _fx.rimOpaqueness;
        float rimIntensity = _fx.rimIntensity;
        float uvAnimOpaque = _fx.uvAnimOpaqueness;
        float innerOpaque = _fx.innerOpaqueness;
        while (timer < fadeTime)
        {
            float percent = Mathf.Clamp01(timer / fadeTime);
            _fx.cylinderRadius = Mathf.Lerp(radius, 0, percent);
            _fx.rimOpaqueness = Mathf.Lerp(rimOpaque, 0, percent);
            _fx.uvAnimOpaqueness = Mathf.Lerp(uvAnimOpaque, 0, percent);
            _fx.innerOpaqueness = Mathf.Lerp(innerOpaque, 0, percent);
            _fx.rimIntensity = Mathf.Lerp(rimIntensity, 8.0f, percent);
            timer += Time.deltaTime;
            yield return null;
        }

        _fx.energyCylinder.SetActive(false);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            Launch(Vector3.forward * 100);
    }

    private void GenerateContacts(Vector3 target)
    {
        //hit fx
        RaycastHit[] hits = Physics.RaycastAll(transform.position, (target - transform.position).normalized, (target - transform.position).magnitude);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject != blastCollider.gameObject)
            {
                var par = Instantiate(_fx.contactParticleSys, hit.point, Quaternion.identity) as ParticleSystem;
                _contactParticles.Add(par);
                par.transform.parent = transform;
            }
        }

        //interact with building
        hits = Physics.CapsuleCastAll(transform.position, target, blastSize, 
            (target - transform.position).normalized, 
            (target - transform.position).magnitude);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("building"))
            {
                hit.collider.gameObject.SendMessage("GiveAttack", 10.0f);
            }
        }
    }
}
