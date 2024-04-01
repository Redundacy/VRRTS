using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Entity", menuName = "Game/ResourceEntity")]
public class ResourceEntityData : GamePieceData
{
    public int numResourcesGranted;
}
