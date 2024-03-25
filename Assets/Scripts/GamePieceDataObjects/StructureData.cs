using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Structure", menuName = "Game/Structure")]
public class StructureData : ScriptableObject
{
    public string structureType;
    public int maxHealth;
    public int cost;
    public enum DestructionReward
    {
        None,
        Resources
    }
    public DestructionReward destructionReward;
    public int rewardAmount;
    public GameObject model;
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
        return structureType;
    }

    public int GetCost()
    {
        return cost;
    }
}
