using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Structures : GamePieces
{
    public StructureData structure;

    //General structure stats
    float health;

    public TextMeshProUGUI healthBarText;

    // Start is called before the first frame update
    void Start()
    {
        health = structure.maxHealth;
        healthBarText.text = "Health: " + health + "/" + structure.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

        //TESTING Dealing Damage to Structure.
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    TakeDamage(10f);
        //}
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Damage taken! Health: " + health);
        healthBarText.text = "Health: " + health + "/" + structure.maxHealth; //Kinda not a fan of just having this line here, but making a method feels silly. Idk.
        if (health <= 0)
        {
            DestroyStructure();
        }
    }

    void DestroyStructure()
    {
        //If the reward for destruction is resources (which is the only reward right now as far as I'm aware) call the game manager to award the correct amount of resources to the player.
        if (structure.destructionReward == StructureData.DestructionReward.Resources)
        {
            //Find the gameManager, call it to give resources to the player equal to the rewardAmount.
        }

        //Destroy self when all is done.
        Debug.Log("Structure destroyed! Hopefully I don't exist anymore.");
        Destroy(gameObject);
    }
}
