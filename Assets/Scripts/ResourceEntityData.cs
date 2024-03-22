using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Entity", menuName = "Game/ResourceEntity")]
public class ResourceEntityData : ScriptableObject
{
    public string entityType;
    public int numResourcesGranted;
    public GameObject model;
    public enum Team
    {
        AlliedTeam,
        EnemyTeam,
        Hostile,
        Neutral
    }
    public override string ToString()
    {
        return entityType;
    }
}
