using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface PoseInterface
{
    List<PoseElementInterface> poseElements { get; set; }
    int poseId { get; set; }
    void StartPoseDetect();
	void EndPoseDetect();

    event EventHandler PoseFinish;
}
