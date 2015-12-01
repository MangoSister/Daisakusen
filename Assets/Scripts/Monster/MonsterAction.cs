using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MonsterAction : MonoBehaviour
{
    public float maxHealth;
    public float rageHealthRatio;
    public float lowHealthRatio;

    public MonsterNormalParam normalParam;
    public MonsterRageParam rageParam;
    public MonsterEscapeParam escapeParam;

    public Vector2 leftBottomBound;
    //{ get { return new Vector2(GameController.Instance.cityLowerBound.x, GameController.Instance.cityLowerBound.z); } }
    public Vector2 rightTopBound;
    //{ get { return new Vector2(GameController.Instance.cityUpperBound.x, GameController.Instance.cityUpperBound.z); } }

    public GameObject target;

	//punch
	//includes 2 ordinary colliders (two hands)
    public GameObject leftPunchObj;
    public GameObject rightPunchObj;

    //tornado
    //ordinary collider
    public float tornadoDamage;
    public float tornadoSize;
    public float tornadoRotSpeed;
    public Tornado tornado;

    //dash
	//ordinary collider
    public GameObject dashObj;

    //explodesion
    public GameObject explosionPrefab;

    //anim
    public MonsterAnimHandler _animHandler
    {
        get { return GetComponent<MonsterAnimHandler>(); }
    }

    //UI
    public HealthBar healthBar;

    private StateEngine<MonsterAction> _stateEngine;
    private bool _init = false;

    //[SerializeField]
    public bool targetActive;
    //[SerializeField]
    private float _health;
    //[SerializeField]
    public int hitCount;

    public void DealDamage(float amount)
    {
        _health -= amount;
        _health = Mathf.Max(_health, 0f);
        hitCount++;
        healthBar.targetHealth = _health / maxHealth;
    }

    public void RestoreHealth(float amount)
    {
        _health += amount;
        _health = Math.Min(_health, maxHealth);
        healthBar.targetHealth = _health / maxHealth;
    }

    public void StartTornado(float tornadoTime)
    {
        _animHandler.TriggerSpinAnim();

        //sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.sfxPlay3D(SFXType.MONSTER_SPATK_SPIN, transform);
        }

        StartCoroutine(TornadoCoroutine(tornadoTime));
        StartCoroutine(TornadoRotCoroutine(tornadoTime));
    }

    private IEnumerator TornadoCoroutine(float tornadoTime)
    {
        //3 - 2 - 5 hard code
        yield return new WaitForSeconds(tornadoTime * 0.3f);

        GameObject physObj = new GameObject();
        physObj.name = "TornadoPhys";
        physObj.tag = "specialAtk";
        physObj.transform.parent = transform;
        physObj.transform.localPosition = Vector3.zero;
        physObj.transform.localRotation = Quaternion.identity;
        physObj.transform.position += Vector3.up * 6f; //hard code
        CapsuleCollider col = physObj.AddComponent<CapsuleCollider>();
        DamagePower pow = physObj.AddComponent<DamagePower>();
        pow.damagePower = tornadoDamage; //hardcode

        col.height = 12f;//hard code
        tornado.BeginWind();
        tornado.scale = 0f;
        col.radius = 0f;
        float timer = 0f;

        while (timer < tornadoTime * 0.2f)
        {
            float r = Mathf.Lerp(0, tornadoSize, Mathf.Clamp01(timer / (tornadoTime * 0.2f)));
            tornado.scale = r;
            col.radius = r;
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f * tornadoTime);
        tornado.EndWind();

        Destroy(physObj);
    }

    private IEnumerator TornadoRotCoroutine(float tornadoTime)
    {
        Quaternion initRot = transform.rotation;
        float rotSpeed = 0f;
        //2-5-2-1 hardcode
        float timer = 0f;
        while (timer < tornadoTime * 0.2f)
        {
            rotSpeed = Mathf.Lerp(0f, tornadoRotSpeed, Mathf.Clamp01(timer / (tornadoTime * 0.2f)));
            transform.Rotate(transform.up, Time.deltaTime * rotSpeed);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;
        while (timer < tornadoTime * 0.5f)
        {
            transform.Rotate(transform.up, Time.deltaTime * tornadoRotSpeed);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;
        while (timer < tornadoTime * 0.2f)
        {
            rotSpeed = Mathf.Lerp(tornadoRotSpeed, 0f, Mathf.Clamp01(timer / (tornadoTime * 0.2f)));
            transform.Rotate(transform.up, Time.deltaTime * rotSpeed);
            timer += Time.deltaTime;
            yield return null;
        }

        //rotate back
        timer = 0f;
        Quaternion afterRot = transform.rotation;
        while (timer < tornadoTime * 0.1f)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(afterRot, initRot, Mathf.Clamp01(timer / (tornadoTime * 0.1f)));
            yield return null;
        }
    }

    public void ActivateDash()
    {
        if (dashObj != null)
            dashObj.SetActive(true);
        //accelertate move anim
        _animHandler.SetMovePlaySpeed(2.0f);

        //sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.sfxPlay3D(SFXType.MONSTER_SPATK, transform);
        }
    }

    public void DeactivateDash()
    {
        if (dashObj != null)
            dashObj.SetActive(false);
        _animHandler.SetMovePlaySpeed(1.0f);
    }

	public void ActivatePunch(PunchType hand) //true: right / false: left
	{
        if (hand == PunchType.Right && rightPunchObj != null)
        {
            rightPunchObj.SetActive(true);
            PunchFXCtrl[] ctrls = rightPunchObj.GetComponentsInChildren<PunchFXCtrl>();
            foreach (var ctrl in ctrls)
                ctrl.PlayFX();
        }
        else if (hand == PunchType.Left && leftPunchObj != null)
        {
            leftPunchObj.SetActive(true);
            PunchFXCtrl[] ctrls = leftPunchObj.GetComponentsInChildren<PunchFXCtrl>();
            foreach (var ctrl in ctrls)
                ctrl.PlayFX();
        }
    }

    public void DeactivatePunch(PunchType hand) //true: right / false: left
    {
        if (hand == PunchType.Right && rightPunchObj != null)
            rightPunchObj.SetActive(false);
        else if (hand == PunchType.Left && leftPunchObj != null)
            leftPunchObj.SetActive(false);
    }

    public void Explode()
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
    }

    public void Init()
    {
        _stateEngine.Init(this, new MonsterStateNormalStage(normalParam));
        _init = true;
    }

    // Use this for initialization
    private void Start()
    {
        _init = false;
        _stateEngine = new StateEngine<MonsterAction>();
        _stateEngine.debug = true;
        //_stateEngine.Init(this, new MonsterStateNormalStage(normalParam));

        //init status
        hitCount = 0;
        targetActive = true;
        _health = maxHealth;

        DeactivatePunch(PunchType.Left);
        DeactivatePunch(PunchType.Right);
        DeactivateDash();

        healthBar.targetHealth = 1.0f;
    }

    // Update is called once per frame
    private void Update ()
    {
        if (_init)
        {
            //state machine update
            _stateEngine.Execute();

            //state machine transisiton
            StateEngineTransition();
        }
	}

    private void StateEngineTransition()
    {
        StateBase<MonsterAction> curr = _stateEngine.currState;

        if (_health <= 0f && !(curr is MonsterStateDie))
        {
            _stateEngine.ChangeState(new MonsterStateDie());
            return;
        }

        if (curr is MonsterStateNormalStage)
        {
            if (_health <= maxHealth * rageHealthRatio)
            {
                _stateEngine.ChangeState(new MonsterStateRageStage(rageParam));
                if (SoundManager.Instance != null)
                    SoundManager.Instance.bgmPlay(BGMStage.FIGHT_STAGE_2);
            }
        }
        else if (curr is MonsterStateRageStage)
        {
            if (_health <= maxHealth * lowHealthRatio)
                _stateEngine.ChangeState(new MonsterStateEscape
                    (
                        leftBottomBound, rightTopBound,
                        escapeParam.escapeSpeed, escapeParam.escapeAngularSpeed,
                        escapeParam.minRDir, escapeParam.maxRDir,
                        target, escapeParam.safeRange
                    ));
        }
        //else if (curr is MonsterEscapeParam)
        //{
        //}
        //dunno what to do when it dies
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("building"))
        {
            other.gameObject.SendMessage("GiveAttack", 10.0f);
        }
    }

    public enum PunchType
    {
        Left, Right,
    }
}
