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
    public SteamVR_Action_Boolean isSelectButtonPressed = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");
    public SteamVR_Action_Boolean isCyclePressed = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("cycleactions");
    public SteamVR_Action_Boolean openMenu = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("openactionwheel");
    /* 
     * 
            "parameters": {
              "long_press_delay": 2
            },
     */
    public enum ActionTypes
    {
        None,
        Select,
        Move,
        Attack,
        Interact,
        Buy
    }
    public List<SteamVR_Input_Sources> sources;
    public List<Transform> indexFingers;
    public LayerMask collectedLayers;
    public float raycastDistance;

    public ActionTypes currentAction;
    public TMP_Text currentActionText;
    public GameObject actionWheel;

    public static event Action<Vector3> MoveSelectedUnits;
    public static event Action<GameObject> TryAttackObject;

    private LineRenderer lrend;
    private Transform indexFinger;
    private RaycastHit lastHit;

    private UnitData handUnit = null;
    // Start is called before the first frame update
    void Start()
    {
        lrend = GetComponent<LineRenderer>();
        //indexFinger = pointingHand.Find("RightRenderModel Slim(Clone)").Find("vr_glove_right_model_slim(Clone)").Find("slim_r").Find("Root").Find("finger_index_r_aux");
        //indexFinger = pointingHand.Find("HoverPoint").Find("Cylinder");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (SteamVR_Input_Sources source in sources)
        {
            if (isSelectButtonPressed.GetState(source))
            {
                indexFinger = indexFingers[sources.IndexOf(source)];
                if (Physics.Raycast(indexFinger.position, indexFinger.right, out lastHit, raycastDistance, collectedLayers)) //currently pointing in the wrong direction (and also I would like to change the angle
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
            } else if (isSelectButtonPressed.GetStateUp(source))
            {
                //Debug.Log(lastHit.point);
                //if lastHit was a world thing, do the action on that thing. If it was a UI thing, do the UI procedure

                if (actionWheel.activeSelf)
                {
                    switch (lastHit.collider.name)
                    {
                        case "Move":
                            currentAction = ActionTypes.Move; break;
                        case "Select":
                            currentAction = ActionTypes.Select; break;
                        case "Attack":
                            currentAction = ActionTypes.Attack; break;
                        case "Interact":
                            currentAction = ActionTypes.Interact; break;
						default:
							currentAction = ActionTypes.None; break;
					}
					currentActionText.text = currentAction.ToString();
					actionWheel.SetActive(false);
				}
                switch(currentAction)
                {
                    case ActionTypes.Select:
                        UnitState unit = lastHit.collider.gameObject.GetComponentInParent<UnitState>();
                        if (unit != null)
                        {
                            unit.ToggleSelection();
                            Debug.Log("selected " + unit);
                        }
                            
                        Debug.Log("select " + lastHit.collider.name);
						break;
                    case ActionTypes.Move:
                        MoveSelectedUnits?.Invoke(lastHit.point);
                        break;
                    case ActionTypes.Attack:
						TryAttackObject?.Invoke(lastHit.collider.gameObject);
						break;
                    case ActionTypes.Interact:
                        break;
                    case ActionTypes.Buy:
                        GameObject.Find("GameManager").GetComponent<GameManager>().RequestMakeGuy(handUnit, lastHit.point);
                        Debug.Log("buy " + handUnit);
                        break;
                    default:
                        break;
                }
                lrend.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
            }
        }

        foreach(SteamVR_Input_Sources source in sources)
        {
            if (isCyclePressed.GetStateUp(source))
            {
                Debug.Log("cycle pressed on hand " + source.ToString());
                currentAction++;
                if (currentAction > ActionTypes.Interact)
                    currentAction = ActionTypes.None;
                currentActionText.text = currentAction.ToString();
            }
        }

        foreach (SteamVR_Input_Sources source in sources)
        {
            if (openMenu.GetStateUp(source))
            {
                Debug.Log("open menu");
                actionWheel.SetActive(!actionWheel.activeSelf);
            }
        }
    }

    public void OnShopSelect(ShowcaseUnits heldUnit)
    {
        currentAction = ActionTypes.Buy;
        actionWheel.SetActive(false);
        handUnit = heldUnit.unit;
    }

}
