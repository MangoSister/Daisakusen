using UnityEngine;
using System.Collections;

public class JointPositionTracker : MonoBehaviour
{
    [Tooltip("The Kinect joint we want to track.")]
    public KinectInterop.JointType trackedJoint = KinectInterop.JointType.SpineBase;

    [Tooltip("Whether the joint view is mirrored or not.")]
    public bool mirroredView = false;

    [Tooltip("Whether the displayed position is in Kinect coordinates, or offset from the initial position.")]
    public bool displayKinectPos = false;

    //public bool moveTransform = true;

    [Tooltip("Smooth factor used for the joint position smoothing.")]
    public float smoothFactor = 5f;


    private Vector3 initialPosition = Vector3.zero;
    private long calibratedUserId = 0;
    private Vector3 initialUserOffset = Vector3.zero;

    private Vector3 _vPosObject;
    public Vector3 vPosObject { get { return _vPosObject; } }

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        KinectManager manager = KinectManager.Instance;

        if (manager && manager.IsInitialized())
        {
            int iJointIndex = (int)trackedJoint;

            if (manager.IsUserDetected())
            {
                long userId = manager.GetPrimaryUserID();

                if (manager.IsJointTracked(userId, iJointIndex))
                {
                    Vector3 vPosJoint = manager.GetJointPosition(userId, iJointIndex);
                    vPosJoint.z = !mirroredView ? -vPosJoint.z : vPosJoint.z;

                    if (userId != calibratedUserId)
                    {
                        calibratedUserId = userId;
                        initialUserOffset = vPosJoint;
                    }

                    _vPosObject = !displayKinectPos ? (vPosJoint - initialUserOffset) : vPosJoint;
                    _vPosObject = initialPosition + _vPosObject;

                    //if(moveTransform)
                    {
                        if (smoothFactor != 0f)
                            transform.position = Vector3.Lerp(transform.position, _vPosObject, smoothFactor * Time.deltaTime);
                        else
                            transform.position = _vPosObject;
                    }
                }

            }

        }
    }
}
