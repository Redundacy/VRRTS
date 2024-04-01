using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Structure", menuName = "Game/Structure")]
public class StructureData : GamePieceData
{
    public int cost;
    public enum DestructionReward
    {
        None,
        Resources
    }
    public DestructionReward destructionReward;
    public int rewardAmount;

    public int GetCost()
    {
        return cost;
    }
}
