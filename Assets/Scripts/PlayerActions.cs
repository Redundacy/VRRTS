using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Valve.VR;
using System;

/// <summary>
/// This component helps player raycast components tell selected units where to move
/// </summary>
public class PlayerActions : MonoBehaviour
{
    public SteamVR_Action_Boolean isMoveButtonPressed = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");
    public SteamVR_Input_Sources source;
    public Transform pointingHand;
    public LayerMask collectedLayers;
    public float raycastDistance;

    public static event Action<Vector3> MoveSelectedUnits;

    private LineRenderer lrend;
    private static Transform indexFinger;
    private RaycastHit lastHit;
    // Start is called before the first frame update
    void Start()
    {
        lrend = GetComponent<LineRenderer>();
        indexFinger = pointingHand.Find("RightRenderModel Slim(Clone)").Find("vr_glove_right_model_slim(Clone)").Find("slim_r").Find("Root").Find("finger_index_r_aux");
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoveButtonPressed.state)
        {
            if (indexFinger == null)
            {
                indexFinger = pointingHand.Find("RightRenderModel Slim(Clone)").Find("vr_glove_right_model_slim(Clone)").Find("slim_r").Find("Root").Find("finger_index_r_aux");
                Debug.Log("Do only once pls");
            }
            if (Physics.Raycast(indexFinger.position, indexFinger.right, out lastHit, raycastDistance)) //currently pointing in the wrong direction (and also I would like to change the angle
            {
                lrend.SetPosition(0, indexFinger.position);
                lrend.SetPosition(1, lastHit.point);

                //Selecting stuff

            } else
            {
                lrend.SetPosition(0, indexFinger.position);
                lrend.SetPosition(1, indexFinger.position + indexFinger.right);
            }
            Debug.Log("pointing? " + lastHit.collider);
        } else if (isMoveButtonPressed.stateUp)
        {
            Debug.Log("move to " + lastHit.point);
            MoveSelectedUnits?.Invoke(lastHit.point);
        }
    }

}
