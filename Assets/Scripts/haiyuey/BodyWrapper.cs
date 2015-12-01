using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text; 

public class BodyWrapper : GenericSingleton<BodyWrapper> {

	public Animator humanoid0;
	public Animator humanoid1;
	protected Transform[] bones0;
	protected Transform[] bones1;

	const int totalBones = 27;


	// Use this for initialization
	void Start () {
		MapBones ();
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Flip();
        }
    }

	public Vector3 GetBoneLocalPosition (KinectInterop.JointType joint, int playerIdx) {
		int boneIdx = jointMap2boneIndex[joint];
		if (playerIdx == 0) {
			if (boneIndex2MecanimMap.ContainsKey(boneIdx)) {
				return bones0[boneIdx].localPosition;
			}
		}
		if (playerIdx == 1) {
			if (boneIndex2MecanimMap.ContainsKey(boneIdx)) {
				return bones1[boneIdx].localPosition;
			}
		}
		if (playerIdx == 2) {
			Vector3 pos0 = Vector3.zero;
			Vector3 pos1 = Vector3.zero;
			int num = 0;
			bool p0 = (KinectManager.Instance.GetUserIdByIndex(0) != 0);
			bool p1 = (KinectManager.Instance.GetUserIdByIndex(1) != 0);
			if (p0) {
				if (boneIndex2MecanimMap.ContainsKey(boneIdx)) {
					pos0 = bones0[boneIdx].localPosition;
					num++;
				}
			}
			if (p1) {
				if (boneIndex2MecanimMap.ContainsKey(boneIdx)) {
					pos1 = bones1[boneIdx].localPosition;
					num++;
				}
			}
			switch(num) {
			case 0: return Vector3.zero;
			case 1: 
				if (p0)
					return pos0;
				if (p1)
					return pos1;
				break;
			case 2:
				return Vector3.Lerp(pos0, pos1, 0.5f);
			default: break;
			}
			return Vector3.zero;
		}
		return Vector3.zero;
	}

	public Quaternion GetBoneLocalRotation (KinectInterop.JointType joint, int playerIdx) {
		int boneIdx = jointMap2boneIndex[joint];
		if (playerIdx == 0) {
			if (boneIndex2MecanimMap.ContainsKey(boneIdx)) {
				return bones0[boneIdx].localRotation;
			}
		}
		if (playerIdx == 1) {
			if (boneIndex2MecanimMap.ContainsKey(boneIdx)) {
				return bones1[boneIdx].localRotation;
			}
		}
		if (playerIdx == 2) {
			Quaternion rot0 = Quaternion.identity;
			Quaternion rot1 = Quaternion.identity;
			int num = 0;
			bool p0 = (KinectManager.Instance.GetUserIdByIndex(0) != 0);
			bool p1 = (KinectManager.Instance.GetUserIdByIndex(1) != 0);
			if (p0) {
				if (boneIndex2MecanimMap.ContainsKey(boneIdx)) {
					rot0 = bones0[boneIdx].localRotation;
					num++;
				}
			}
			if (p1) {
				if (boneIndex2MecanimMap.ContainsKey(boneIdx)) {
					rot1 = bones1[boneIdx].localRotation;
					num++;
				}
			}
			switch(num) {
			case 0: return Quaternion.identity;
			case 1: 
				if (p0)
					return rot0;
				if (p1)
					return rot1;
				break;
			case 2:
				return Quaternion.Lerp(rot0, rot1, 0.5f);
			default: break;
			}
			return Quaternion.identity;
		}
		return Quaternion.identity;
	}

	void MapBones() {

		bones0 = new Transform[totalBones];

		for (int boneIndex = 0; boneIndex < bones0.Length; boneIndex++)
		{
			if (!boneIndex2MecanimMap.ContainsKey(boneIndex)) 
				continue;
			
			bones0[boneIndex] = humanoid0.GetBoneTransform(boneIndex2MecanimMap[boneIndex]);
		}

		bones1 = new Transform[totalBones];
		
		for (int boneIndex = 0; boneIndex < bones1.Length; boneIndex++)
		{
			if (!boneIndex2MecanimMap.ContainsKey(boneIndex)) 
				continue;
			bones1[boneIndex] = humanoid1.GetBoneTransform(boneIndex2MecanimMap[boneIndex]);
		}
	}

    void Flip()
    {
        Animator temp = humanoid0;
        humanoid0 = humanoid1;
        humanoid1 = temp;
        MapBones();
    }


	private readonly Dictionary<int, HumanBodyBones> boneIndex2MecanimMap = new Dictionary<int, HumanBodyBones>
	{
		{0, HumanBodyBones.Hips},
		{1, HumanBodyBones.Spine},
		{2, HumanBodyBones.Chest},
		{3, HumanBodyBones.Neck},
		{4, HumanBodyBones.Head},
		
		{5, HumanBodyBones.LeftUpperArm},
		{6, HumanBodyBones.LeftLowerArm},
		{7, HumanBodyBones.LeftHand},
		{8, HumanBodyBones.LeftIndexProximal},
		//		{9, HumanBodyBones.LeftIndexIntermediate},
		//		{10, HumanBodyBones.LeftThumbProximal},
		
		{11, HumanBodyBones.RightUpperArm},
		{12, HumanBodyBones.RightLowerArm},
		{13, HumanBodyBones.RightHand},
		{14, HumanBodyBones.RightIndexProximal},
		//		{15, HumanBodyBones.RightIndexIntermediate},
		//		{16, HumanBodyBones.RightThumbProximal},
		
		{17, HumanBodyBones.LeftUpperLeg},
		{18, HumanBodyBones.LeftLowerLeg},
		{19, HumanBodyBones.LeftFoot},
		//		{20, HumanBodyBones.LeftToes},
		
		{21, HumanBodyBones.RightUpperLeg},
		{22, HumanBodyBones.RightLowerLeg},
		{23, HumanBodyBones.RightFoot},
		//		{24, HumanBodyBones.RightToes},
		
		{25, HumanBodyBones.LeftShoulder},
		{26, HumanBodyBones.RightShoulder},
	};

	protected readonly Dictionary<KinectInterop.JointType, int> jointMap2boneIndex = new Dictionary<KinectInterop.JointType, int>
	{
		{KinectInterop.JointType.SpineBase, 0},
		{KinectInterop.JointType.SpineMid, 1},
		{KinectInterop.JointType.SpineShoulder, 2},
		{KinectInterop.JointType.Neck, 3},
		{KinectInterop.JointType.Head, 4},
		
		{KinectInterop.JointType.ShoulderLeft, 5},
		{KinectInterop.JointType.ElbowLeft, 6},
		{KinectInterop.JointType.WristLeft, 7},
		{KinectInterop.JointType.HandLeft, 8},
		
		{KinectInterop.JointType.HandTipLeft, 9},
		{KinectInterop.JointType.ThumbLeft, 10},
		
		{KinectInterop.JointType.ShoulderRight, 11},
		{KinectInterop.JointType.ElbowRight, 12},
		{KinectInterop.JointType.WristRight, 13},
		{KinectInterop.JointType.HandRight, 14},
		
		{KinectInterop.JointType.HandTipRight, 15},
		{KinectInterop.JointType.ThumbRight, 16},
		
		{KinectInterop.JointType.HipLeft, 17},
		{KinectInterop.JointType.KneeLeft, 18},
		{KinectInterop.JointType.AnkleLeft, 19},
		{KinectInterop.JointType.FootLeft, 20},
		
		{KinectInterop.JointType.HipRight, 21},
		{KinectInterop.JointType.KneeRight, 22},
		{KinectInterop.JointType.AnkleRight, 23},
		{KinectInterop.JointType.FootRight, 24},
	};

}
