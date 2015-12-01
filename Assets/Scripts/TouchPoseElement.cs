using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PoseTriggerType = PoseManager.PoseTriggerType;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class TouchPoseElement : MonoBehaviour, PoseElementInterface
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

    public List<PoseTriggerType> possibleTriggers;

    private void Awake()
    {
        Debug.Assert(possibleTriggers != null && possibleTriggers.Count > 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_enableDetection)
            return;

        foreach (PoseTriggerType type in possibleTriggers)
        {
            if (PoseManager.Instance.Type2PoseTrigger(type) == other)
            {
                _isFinished = true;
                if (PoseElementFinish != null)
                    PoseElementFinish(this, null);
                //sound
                SoundManager.Instance.sfxPlay(SFXType.ROBOT_POSE_HIT);
                //visuals
                EffectManager.Instance.PoseFeedback(0.1f, Color.blue, 0.5f);
                break;
            }
            else continue;
        }
    }

    //PoseElementInterface
    public bool isFinished { get { return _isFinished; } }
    public bool requireHold { get { return false; } }
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
        if (PoseElementReset != null)
            PoseElementReset(this, null);
        gameObject.SetActive(true);
    }

    public event EventHandler PoseElementFinish;
    public event EventHandler PoseElementInterrupt;
    public event EventHandler PoseElementReset;
}
