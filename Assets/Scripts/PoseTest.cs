using UnityEngine;
using System.Collections;

public class PoseTest : MonoBehaviour
{
    public GameObject avatar;
    public Vector3 poseOffset;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var pose = PoseManager.Instance.CreateNextPose
                (PoseManager.PoseType.SpeedUp, avatar.transform);
        }                          
    }
}
