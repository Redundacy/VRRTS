using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Game/Unit")]
public class UnitData : ScriptableObject
{
    public string unitType;
    public int maxHealth;
    public int damage;
    public float attackRange;
    public float timeBetweenAttacks;
    public float moveSpeed;
    public int cost;
    public GameObject model;
    public GameObject hat;
    public LayerMask hostileTargets;
    public enum Team
    {
        AlliedTeam,
        EnemyTeam,
        Hostile,
        Neutral
    }

    public override string ToString()
	{
        return unitType;
	}

    public int GetCost()
    {
        return cost;
    }
}
