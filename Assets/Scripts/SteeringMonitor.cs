using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using JointType = KinectInterop.JointType;

public class SteeringMonitor : MonoBehaviour
{
    //should be set according to the angle of kinect
    public Vector3 upVec = Vector3.up;
    public float steerScale = 1.0f;
    public int userIdx;
    private long _userId;
    private Dictionary<JointType, Vector3> _initOffset;
    private Vector3 _initDir;
    private Dictionary<JointType, Vector3> _currPos;
    
    private static KinectManager manager
    { get { return KinectManager.Instance; } }

    private bool _calibrateCompleted = false;
    public bool calibrateCompleted { get { return _calibrateCompleted; } }

    private void Start()
    {
        Debug.Assert(userIdx == 0 || userIdx == 1);
        upVec.Normalize();
        _initOffset = new Dictionary<JointType, Vector3>();
        _initOffset.Add(JointType.SpineShoulder, Vector3.zero);
        _initOffset.Add(JointType.ShoulderLeft, Vector3.zero);
        _initOffset.Add(JointType.ShoulderRight, Vector3.zero);

        _currPos = new Dictionary<JointType, Vector3>();
        _currPos.Add(JointType.SpineShoulder, Vector3.zero);
        _currPos.Add(JointType.ShoulderLeft, Vector3.zero);
        _currPos.Add(JointType.ShoulderRight, Vector3.zero);
    }

    private void Update()
    {
        if (!_calibrateCompleted)
            return;

        if (manager && manager.IsInitialized() && manager.IsUserDetected())
        {
            long currUserId = manager.GetUserIdByIndex(userIdx);
            if (currUserId == 0 || currUserId != _userId)
            {
                _calibrateCompleted = false;
                return;
            }

            List<JointType> keys = new List<JointType>(_currPos.Keys);
            foreach (JointType key in keys)
            {
                if (manager.IsJointTracked(_userId, (int)key))
                    _currPos[key] = manager.GetJointPosition(_userId, (int)key);
                //allow temporarily untracked
            }
        }
    }

    public void ReCalibrate()
    {
        _calibrateCompleted = false;

        if (manager && manager.IsInitialized() && manager.IsUserDetected())
        {
            _userId = manager.GetUserIdByIndex(userIdx);
            if (_userId == 0)
                return; //init not completed
            List<JointType> keys = new List<JointType>(_initOffset.Keys);
            foreach (JointType key in keys)
            {
                if (manager.IsJointTracked(_userId, (int)key))
                    _initOffset[key] = manager.GetJointPosition(_userId, (int)key);
                else return; //init not completed
            }

            //compute init direction
            Vector3 left2Mid = (_initOffset[JointType.SpineShoulder] - _initOffset[JointType.ShoulderLeft]);
            Vector3 left2MidProj = Vector3.ProjectOnPlane(left2Mid, upVec).normalized;
            Vector3 mid2Right = (_initOffset[JointType.ShoulderRight] - _initOffset[JointType.SpineShoulder]);
            Vector3 mid2RightProj = Vector3.ProjectOnPlane(mid2Right, upVec).normalized;
            Vector3 left2Right = (_initOffset[JointType.SpineShoulder] - _initOffset[JointType.ShoulderLeft]);
            Vector3 left2RightProj = Vector3.ProjectOnPlane(left2Right, upVec).normalized;
            _initDir = (left2MidProj + mid2RightProj + left2RightProj).normalized;
            _calibrateCompleted = true;
        }
        else return;

    }

    public float GetSteeringAxis()
    {
        if (!_calibrateCompleted)
            return 0f;
        
        //get perpendicular plane of upVec
        // Plane groundPlane = new Plane(upVec, _currPos[JointType.SpineShoulder]);
        Vector3 left2Mid = (_currPos[JointType.SpineShoulder] - _currPos[JointType.ShoulderLeft]);
        Vector3 left2MidProj = Vector3.ProjectOnPlane(left2Mid, upVec).normalized;
        Vector3 mid2Right = (_currPos[JointType.ShoulderRight] - _currPos[JointType.SpineShoulder]);
        Vector3 mid2RightProj = Vector3.ProjectOnPlane(mid2Right, upVec).normalized;
        Vector3 left2Right = (_currPos[JointType.SpineShoulder] - _currPos[JointType.ShoulderLeft]);
        Vector3 left2RightProj = Vector3.ProjectOnPlane(left2Right, upVec).normalized;
        Vector3 currDir = (left2MidProj + mid2RightProj + left2RightProj).normalized;

        float angle = Vector3.Angle(_initDir, currDir);
        if (Vector3.Cross(_initDir, currDir).y > 0f)
            angle *= -1f;

        return Mathf.Clamp(angle / 90f * steerScale, -1f, 1f);
    }
}
