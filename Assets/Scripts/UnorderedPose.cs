using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UnorderedPose: MonoBehaviour, PoseInterface
{
    public List<PoseElementInterface> poseElements { get; set; }
    public int poseId { get; set; }
    public event EventHandler PoseFinish;
    private bool _isEnded;
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

    public void StartPoseDetect()
    {
        foreach (var pose in poseElements)
            pose.StartDetect();
        StartCoroutine(CheckFinishCoroutine());
    }

	public void EndPoseDetect()
	{
        if (!_isEnded)
        {
            _isEnded = true;
            Destroy(gameObject);
        }
    }


    private IEnumerator CheckFinishCoroutine()
    {
        while (true)
        {
            bool finish = true;
            foreach (PoseElementInterface pose in poseElements)
            {
                if (pose.isFinished)
                    pose.EndDetect();
                finish = pose.isFinished;
                if (!finish)
                    break;
            }

            if (finish)
                break;
            yield return null;
        }

        //finish
        Debug.Log(gameObject.name + " unordered pose finished");
        if (PoseFinish != null)
            PoseFinish(this, null);
		EndPoseDetect();

        //sound
        SoundManager.Instance.sfxPlay(SFXType.ROBOT_POSE_COMPLETE);
    }
}
