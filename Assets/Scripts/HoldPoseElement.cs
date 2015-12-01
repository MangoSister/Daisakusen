using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PoseTriggerType = PoseManager.PoseTriggerType;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class HoldPoseElement : MonoBehaviour, PoseElementInterface
{
    public float detectionRadius
    {
        get
        { return GetComponent<SphereCollider>().radius; }
        set
        { GetComponent<SphereCollider>().radius = value; }
    }

    private bool _isFinished = false;
    private bool _enableDetection = false;

    public float holdTime;
    public List<PoseTriggerType> possibleTriggers;
    private List<PoseTriggerType> _currTriggers;
    private Coroutine _currHoldCoroutine;

    public TexSwitcher texSwitcher;// { get { return GetComponent<TexSwitcher>(); } }

    private void Awake()
    {
        Debug.Assert(possibleTriggers != null && possibleTriggers.Count > 0);
        _currTriggers = new List<PoseTriggerType>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_enableDetection)
            return;
        PoseTriggerType otherType = PoseManager.Instance.PoseTrigger2Type(other);
        if (otherType == PoseTriggerType.Undefined)
            return;

        if (possibleTriggers.Find((PoseTriggerType type) => { return type == otherType; }) 
            != PoseTriggerType.Undefined)
        {
            _currTriggers.Add(otherType);
            if (_currTriggers.Count == 1)
            {
                _currHoldCoroutine = StartCoroutine(HoldCoroutine());
            }
        }
    }

    private IEnumerator HoldCoroutine()
    {
        if (texSwitcher != null)
            texSwitcher.NextTex();
        float timer = 0f;
        float repeatedTimer = 0f;
        while (timer < holdTime)
        {
            if (texSwitcher != null && repeatedTimer > Mathf.Repeat(repeatedTimer + Time.deltaTime, holdTime / (float)texSwitcher.texes.Count))
                texSwitcher.NextTex();
            timer += Time.deltaTime;
            repeatedTimer = Mathf.Repeat(repeatedTimer + Time.deltaTime, holdTime / (float)texSwitcher.texes.Count);
            yield return null;
        }

        _isFinished = true;
        if (PoseElementFinish != null)
            PoseElementFinish(this, null);

        //sound
        SoundManager.Instance.sfxPlay(SFXType.ROBOT_POSE_HIT);
        //visuals
        EffectManager.Instance.PoseFeedback(0.1f, Color.yellow, 0.5f);
    }

    private void OnTriggerExit(Collider other)
    {
        PoseTriggerType otherType = PoseManager.Instance.PoseTrigger2Type(other);
        if (_currTriggers.Remove(otherType))
        {
            if (_currTriggers.Count == 0)
            {
                StopCoroutine(_currHoldCoroutine);
                if (texSwitcher != null)
                    texSwitcher.ResetTex();
                if (PoseElementInterrupt != null)
                    PoseElementInterrupt(this, null);
            }
        }
    }

    //PoseElementInterface
    public bool isFinished { get { return _isFinished; } }
    public bool requireHold { get { return true; } }
    public void StartDetect()
    {
        _enableDetection = true;
        gameObject.SetActive(true);
    }

    public void EndDetect()
    {
        _enableDetection = false;
        gameObject.SetActive(false);
    }

    public void ResetDetect()
    {
        _isFinished = false;
        _currTriggers.Clear();
        if (PoseElementReset != null)
            PoseElementReset(this, null);
        gameObject.SetActive(true);
    }

    public event EventHandler PoseElementFinish;
    public event EventHandler PoseElementInterrupt;
    public event EventHandler PoseElementReset;
}
