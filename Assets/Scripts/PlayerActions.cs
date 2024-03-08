using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Valve.VR;
using System;
using TMPro;

/// <summary>
/// This component helps player raycast components tell selected units where to move
/// </summary>
public class PlayerActions : MonoBehaviour
{
    public SteamVR_Action_Boolean isMoveButtonPressed = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");
    public SteamVR_Action_Boolean isCyclePressed = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("cycleactions");
    /* 
     * 
            "parameters": {
              "long_press_delay": 2
            },
     */
    enum ActionTypes
    {
        None,
        Move,
        Attack,
        Interact
    }
    public SteamVR_Input_Sources[] sources;
    public Transform pointingHand;
    public LayerMask collectedLayers;
    public float raycastDistance;
    public TMP_Text currentActionText;

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
                indexFinger = pointingHand.Find("RightRenderModel Slim(Clone)").Find("vr_glove_right_model_slim(Clone)").Find("slim_r").Find("Root").Find("finger_index_r_aux"); //replace with object that replicates that
                /*
                Vector3(-0.0120021198,-0.0422417708,0.0276686773)

                */
            }
            
            Debug.Log(Vector3.Distance(pointingHand.transform.position, indexFinger.transform.position));
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
