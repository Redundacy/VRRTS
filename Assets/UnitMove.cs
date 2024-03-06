using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Valve.VR;

/// <summary>
/// This component helps player raycast components tell selected units where to move
/// </summary>
public class UnitMove : MonoBehaviour
{
    public SteamVR_Action_Boolean isPointing = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");
    public GameObject pointingHand;
    public LayerMask collectedLayers;
    public float raycastDistance;

    private LineRenderer lrend;
    // Start is called before the first frame update
    void Start()
    {
        lrend = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isPointing.state)
        {
            RaycastHit hit;
            if (Physics.Raycast(pointingHand.transform.position, pointingHand.transform.position + pointingHand.transform.forward, out hit, raycastDistance)) //currently pointing in the wrong direction (and also I would like to change the angle
            {
                lrend.SetPosition(0, pointingHand.transform.position);
                lrend.SetPosition(1, hit.point);

                //Selecting stuff

            } else
            {
                lrend.SetPosition(0, pointingHand.transform.position);
                lrend.SetPosition(1, pointingHand.transform.position + pointingHand.transform.forward);
            }
            Debug.Log("pointing? " + hit.collider);
        } else if (isPointing.stateUp)
        {
            Debug.Log("not pointing anymore");
        }
    }
}
