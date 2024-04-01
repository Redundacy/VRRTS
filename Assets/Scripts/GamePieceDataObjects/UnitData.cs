using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Game/Unit")]
public class UnitData : GamePieceData
{
    public int damage;
    public float attackRange;
    public float timeBetweenAttacks;
    public float moveSpeed;
    public float visionRange;
    public int cost;
    public float speed;
    public GameObject hat;

    public int GetCost()
    {
        return cost;
    }
}
