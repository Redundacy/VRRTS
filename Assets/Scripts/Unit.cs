using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Game/Unit")]
public class Unit : ScriptableObject
{
    public string unitType;
    public int maxHealth;
    public int damage;
    public float attackRange;
    public float moveSpeed;
    public int cost;
    public GameObject hat;

	public override string ToString()
	{
        return unitType;
	}

    public int GetCost()
    {
        return cost;
    }
}
