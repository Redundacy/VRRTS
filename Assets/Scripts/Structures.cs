using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Structures : GamePieces
{
    //Game Manager to refer to. I doubt this is the cleanest way to do it but we can change later.
    public GameManager gameManager;

    //General structure stats
    float health;
    public float maxHealth;
    
    public string structureType; //May want to specify what kind of structure it is within this script? Would maybe be used to determine what the structure is doing for the player. Adding this here for now.

    public string destroyReward;
    public float rewardAmount;

    public TextMeshProUGUI healthBarText;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthBarText.text = "Health: " + health + "/" + maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

        //TESTING Dealing Damage to Structure.
        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(10f);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Damage taken! Health: " + health);
        healthBarText.text = "Health: " + health + "/" + maxHealth; //Kinda not a fan of just having this line here, but making  method feels silly. Idk.
        if (health <= 0)
        {
            DestroyStructure();
        }
    }

    void DestroyStructure()
    {
        //If the reward for destruction is resources (which is the only reward right now as far as I'm aware) call the game manager to award the correct amount of resources to the player.
        if (destroyReward == "Resources")
        {
            gameManager.GetComponent<GameManager>().GivePlayerResources(rewardAmount);
        }

        //Destroy self when all is done.
        Debug.Log("Structure destroyed! Hopefully I don't exist anymore.");
        Destroy(gameObject);
    }
}
