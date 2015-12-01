using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PoseManager : GenericSingleton<PoseManager>
{
    private static Transform robotTransform { get { return GameController.Instance.robotTransform; } }

    public float distScale = 1f;
    public float triggerScale = 1f;
    public Vector3 offset = Vector3.zero;

    public enum PoseTriggerType
    {
        LeftShoulder = 0,
        LeftElbow = 1,
        LeftHand = 2,

        RightShoulder = 3,
        RightElbow = 4,
        RightHand = 5,

        Undefined = 999,
    }
    public enum PoseType
    {
        //hardcode here
        TestOrdered = 0,
        TestUnordered = 1,
        LeftDodge = 2,
        RightDodge = 3,
		SuperBlast = 4,
		SpeedUp = 5,
    }

    public List<Collider> poseTriggers;
    public List<GameObject> posePrefabs;

    private PoseInterface _currAbilityPose;

    public bool infiniteDodge;
    public float dodgeSlowMotionScale = 0.5f;
    private PoseInterface _currDodgePose;

    public SuperBlast superBlastPrefab;
    public float superBlastRange = 20f;
    public float superBlastSize = 5f;

    public GameObject healthUpPrefab;
    public float RecoverHealthAmount;

    private void Start()
    {
        if (infiniteDodge)
            _currDodgePose = CreateNextPose(PoseType.LeftDodge, robotTransform);
    }

	public PoseInterface CreateNextPose(PoseType type, Transform parent)
	{
		GameObject obj = Instantiate(posePrefabs[(int)type], Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.parent = parent;

		obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;// * scale;

        foreach (Transform t in obj.transform)
        {
            t.localPosition *= distScale;
            t.localScale *= triggerScale;
        }

        obj.transform.localPosition = offset;

        PoseInterface pose = obj.GetComponent<PoseInterface>();
		pose.poseId = (int)type;
		pose.PoseFinish += OnPoseFinish;
        pose.StartPoseDetect();
        if (type != PoseType.LeftDodge && type != PoseType.RightDodge)
        {
            //replace old ability pose
            if (_currAbilityPose != null)
                _currAbilityPose.EndPoseDetect();
            _currAbilityPose = pose;
        }
        else
        {
            if (_currDodgePose != null)
                _currDodgePose.EndPoseDetect();
            _currDodgePose = pose;
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.targetTimeScale = dodgeSlowMotionScale;
                if (SoundManager.Instance != null)
                    SoundManager.Instance.EnterSlowMotion();
            }
        }

        return pose;
	}

    public PoseInterface CreateNextPose(PoseType type, Vector3 pos, Quaternion rot)
    {
		GameObject obj = Instantiate(posePrefabs[(int)type], pos, rot) as GameObject;

		PoseInterface pose = obj.GetComponent<PoseInterface>();
        pose.poseId = (int)type;
        pose.PoseFinish += OnPoseFinish;
        pose.StartPoseDetect();
        if (type != PoseType.LeftDodge && type != PoseType.RightDodge)
        {
            //replace old ability pose
            if (_currAbilityPose != null)
                _currAbilityPose.EndPoseDetect();
            _currAbilityPose = pose;
        }
        else
        {
            if (_currDodgePose != null)
                _currDodgePose.EndPoseDetect();
            _currDodgePose = pose;
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.targetTimeScale = dodgeSlowMotionScale;
                if (SoundManager.Instance != null)
                    SoundManager.Instance.EnterSlowMotion();
            }
        }

        return pose;
    }

    public void CancelCurrDodgePose()
    {
        if (_currDodgePose != null)
            _currDodgePose.EndPoseDetect();
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.targetTimeScale = 1.0f;
            if (SoundManager.Instance != null)
                SoundManager.Instance.ExitSlowMotion();
        }
    }

    private void OnPoseFinish(object sender, EventArgs arg)
    {
        PoseInterface pose = (sender as MonoBehaviour).gameObject.GetComponent<PoseInterface>();
        switch ((PoseType)pose.poseId)
        {
            case PoseType.LeftDodge:
                {
                    if (TimeManager.Instance != null)
                    {
                        TimeManager.Instance.targetTimeScale = 1.0f;
                        if (SoundManager.Instance != null)
                            SoundManager.Instance.ExitSlowMotion();
                    }
                    InputProxy.Instance.SetDodgeDirection(Vector3.left);
                    if (infiniteDodge)
                        _currDodgePose = CreateNextPose(PoseType.RightDodge, robotTransform);
                    break;
                }
            case PoseType.RightDodge:
                {
                    if (TimeManager.Instance != null)
                    {
                        TimeManager.Instance.targetTimeScale = 1.0f;
                        if (SoundManager.Instance != null)
                            SoundManager.Instance.ExitSlowMotion();
                    }
                    InputProxy.Instance.SetDodgeDirection(Vector3.right);
                    if (infiniteDodge)
                        _currDodgePose = CreateNextPose(PoseType.LeftDodge, robotTransform);
                    break;
                }
            case PoseType.SuperBlast:
                {
                    var blast = Instantiate(superBlastPrefab, robotTransform.position, Quaternion.identity) as SuperBlast;
                    blast.blastSize = superBlastSize;
                    blast.Launch(robotTransform.position + robotTransform.forward * superBlastRange); 
                    break;
                }
            case PoseType.SpeedUp:
                {
                    if (GameController.Instance != null)
                    {
                        GameController.Instance.robotTransform.gameObject.
                            GetComponent<RobotCtrl>().GetHealth(RecoverHealthAmount);
                        Instantiate(healthUpPrefab, robotTransform.position, robotTransform.rotation);
                    }
                    break;
                }
            default: break;
        };
    }

    public Collider Type2PoseTrigger(PoseTriggerType type)
    {
        if (type == PoseTriggerType.Undefined)
            return null;
        else return poseTriggers[(int)type];
    }

    public PoseTriggerType PoseTrigger2Type(Collider col)
    {
        int idx = poseTriggers.FindIndex((Collider c) => { return ReferenceEquals(c, col); });
        if (idx == -1)
            return PoseTriggerType.Undefined;
        else return (PoseTriggerType)(idx);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (_currDodgePose != null)
                OnPoseFinish(_currDodgePose, null);
           // SoundManager.Instance.sfxPlay(SFXType.ROBOT_POSE_COMPLETE);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (_currAbilityPose != null)
                OnPoseFinish(_currAbilityPose, null);
        }
    }
}
