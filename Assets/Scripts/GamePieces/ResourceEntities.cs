using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceEntities : GamePieces
{
    public ResourceEntityData resourceEntity;

    //General structure stats
    float health;

    public TextMeshProUGUI healthBarText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        Debug.Log("Damage taken! Health: " + health);
        healthBarText.text = "Health: " + health + "/" + resourceEntity.maxHealth; //Kinda not a fan of just having this line here, but making a method feels silly. Idk.
        if (health <= 0)
        {
            DestroyEntity();
        }
    }

    public void DestroyEntity()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().GivePlayerResources(resourceEntity.numResourcesGranted);

        //Destroy self when all is done.
        Debug.Log("Structure destroyed! Hopefully I don't exist anymore.");
        Destroy(gameObject);
    }
}
