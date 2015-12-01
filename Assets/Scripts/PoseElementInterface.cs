using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface PoseElementInterface
{
    bool isFinished { get; }
    bool requireHold { get; }

    //after calling StartDetect(), PoseElement should execute detection coroutine
    void StartDetect();
    //after calling StartDetect(), PoseElement should not execute detection coroutine (no computing resources)
    void EndDetect();
    //reset all internal states of a pose element (mainly for re-detection)
    void ResetDetect();

    event EventHandler PoseElementFinish;
    event EventHandler PoseElementInterrupt;
    event EventHandler PoseElementReset;
}
