using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class ShowcaseUnits : GamePieces
{

    public UnitData unit;

    int health;

    //Objects to interact with.
    GameObject targetedObject;
    Vector3 locationToMoveTo;

    //Booleans for determining actions.
    public bool isSelected;
    bool isAttacking;
    bool isMoving;

    //The nav mesh agent.
    NavMeshAgent m_Agent;

    //overhead text
    public TextMeshProUGUI selectedText; //Unit text needs to look at camera otherwse its weird.

    void Start()
    {
        //health = unit.maxHealth;
    }
}
