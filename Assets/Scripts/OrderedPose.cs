using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class OrderedPose : MonoBehaviour, PoseInterface
{
    public List<PoseElementInterface> poseElements { get; set; }
    public int poseId { get; set; }
    public event EventHandler PoseFinish;
    private bool _isEnded;
    private int _currPhase;

    //temporary walk around: add all children objects PoseElementInterface to array
    //better solution: custom interface inspector?
    private void Awake()
    {
        poseElements = new List<PoseElementInterface>();
        foreach (Transform child in transform)
            if (child.GetComponent<PoseElementInterface>() != null)
                poseElements.Add(child.GetComponent<PoseElementInterface>());
        _isEnded = false;
        //debug purpose
        //StartPoseDetect();
    }

	public void EndPoseDetect()
	{
        if (!_isEnded)
        {
            _isEnded = true;
            Destroy(gameObject);
        }
    }

    public void StartPoseDetect()
    {
        Debug.Assert(poseElements != null && poseElements.Count > 0);
        _currPhase = 0;
        foreach (var pose in poseElements)
        {
            pose.PoseElementFinish += OnPoseElementFinish;
            pose.PoseElementInterrupt += OnPoseElementInterrupt;
        }
        poseElements[_currPhase].StartDetect();
    }

    private void OnPoseElementFinish(object sender, EventArgs arg)
    {
        PoseElementInterface pose = sender as PoseElementInterface;
        if (!ReferenceEquals(pose, poseElements[_currPhase]))
            return;
            //throw new UnityException("unexpected current pose");

        if (!pose.requireHold)
            pose.EndDetect();

        Debug.Log(gameObject.name + " phase " + _currPhase + " finished");
        _currPhase++;
        if (_currPhase < poseElements.Count)
            poseElements[_currPhase].StartDetect();
        else
        {
            Debug.Log("ordered pose finished");
            //unregister
            foreach (var p in poseElements)
            {
                p.PoseElementFinish -= OnPoseElementFinish;
                p.PoseElementInterrupt -= OnPoseElementInterrupt;
            }

            if (PoseFinish != null)
                PoseFinish(this, null);

            EndPoseDetect();

            //sound
            SoundManager.Instance.sfxPlay(SFXType.ROBOT_POSE_COMPLETE);
        }
        
    }

    private void OnPoseElementInterrupt(object sender, EventArgs arg)
    {
        PoseElementInterface pose = sender as PoseElementInterface;
        //retrieve interruption source
        int interruptPhase = -1;
        for (int i = 0; i < poseElements.Count; i++)
        {
            if (ReferenceEquals(poseElements[i], pose))
            {
                interruptPhase = i;
                break;
            }
        }
        if (interruptPhase == -1)
            throw new UnityException("cannot find interruption source");

        Debug.Log("interrupted by phase " + interruptPhase);

        for (int i = interruptPhase; i <= _currPhase; i++)
        {
            poseElements[i].EndDetect();
            poseElements[i].ResetDetect();
        }

        _currPhase = interruptPhase;
        poseElements[_currPhase].StartDetect();
    }
	
}
