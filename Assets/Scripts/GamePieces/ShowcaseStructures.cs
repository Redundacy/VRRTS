using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowcaseStructures : GamePieces
{
    public StructureData structure;

    //General structure stats
    float health;

    public void InitializeData()
    {
        health = structure.maxHealth;
    }
}
