using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Valve.VR;

public class MatchPlayerTransforms : NetworkBehaviour
{
    public Transform head;
    public Transform body;
    public Transform leftHand;
    public Transform rightHand;

    private SteamVR_Behaviour_Pose leftHandPose;
    private SteamVR_Behaviour_Pose rightHandPose;

    // Start is called before the first frame update
    void Start()
    {
        if(IsOwner)
        {
            SteamVR_Behaviour_Pose[] poses = FindObjectsByType<SteamVR_Behaviour_Pose>(FindObjectsSortMode.InstanceID);
            foreach (SteamVR_Behaviour_Pose pose in poses)
            {
                if (pose.inputSource == SteamVR_Input_Sources.LeftHand)
                {
                    leftHandPose = pose;
                } else if (pose.inputSource == SteamVR_Input_Sources.RightHand)
                {
                    rightHandPose = pose;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            head.position = Camera.main.transform.position;
            head.rotation = Camera.main.transform.rotation;

            if (leftHandPose != null)
            {
                leftHand.position = leftHandPose.transform.position;
                leftHand.rotation = leftHandPose.transform.rotation;
                if (leftHand.childCount > 0)
                {
                    leftHand.GetChild(0).gameObject.SetActive(false);
                }
            }
            if (rightHandPose != null)
            {

                rightHand.position = rightHandPose.transform.position;
                rightHand.rotation = rightHandPose.transform.rotation;
                if (rightHand.childCount > 0)
                {
                    rightHand.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }
}
