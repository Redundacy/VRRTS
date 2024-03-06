using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Use physics raycast hit from mouse click to set agent destination 
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class TestMovement : MonoBehaviour
{
    NavMeshAgent m_Agent;
    RaycastHit m_HitInfo = new RaycastHit();

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        PlayerActions.MoveSelectedUnits += MoveNow;
    }

    void MoveNow(Vector3 newPos)
    {
        m_Agent.destination = newPos;
    }
}