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
    public Canvas healthBar;

    public enum Team
    {
        AlliedTeam,
        EnemyTeam,
        Hostile,
        Neutral
    }

    Team team;

    [SerializeField] GameObject hitEffect;
    [SerializeField] GameObject deathEffect;

    // Start is called before the first frame update
    void Start()
    {
        health = structure.maxHealth;
        healthBarText.text = "Health: " + health + "/" + structure.maxHealth;
    }
    public void InitializeData()
    {
        health = structure.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

        //TESTING Dealing Damage to Structure.
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    TakeDamage(10f);
        //}

        Vector3 lookDirection = healthBar.transform.position - GameObject.Find("VRRTS Player").transform.position;
        healthBar.transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Structure Damage taken! Health: " + health);
        healthBarText.text = "Health: " + health + "/" + structure.maxHealth; //Kinda not a fan of just having this line here, but making a method feels silly. Idk.

        Vector3 hitParticlePosition = gameObject.transform.position;
        hitParticlePosition.y = hitParticlePosition.y + 0.25f;
        Instantiate(hitEffect, hitParticlePosition, Quaternion.identity);

        if (health <= 0)
        {
            DestroyStructure();
        }
    }

    public void SetTeam(string teamToSet)
    {
        switch (teamToSet)
        {
            case "AlliedTeam":
                team = Team.AlliedTeam;
                break;
            case "EnemyTeam":
                team = Team.EnemyTeam;
                break;
            case "Hostile":
                team = Team.Hostile;
                break;
            case "Neutral":
                team = Team.Neutral;
                break;
        }
    }

    public string GetTeamString()
    {
        return team.ToString();
    }

    void DestroyStructure()
    {
        //If the reward for destruction is resources (which is the only reward right now as far as I'm aware) call the game manager to award the correct amount of resources to the player.
        if (structure.destructionReward == StructureData.DestructionReward.Resources)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().GivePlayerResources(structure.rewardAmount);
        }

        Vector3 deathParticlePosition = gameObject.transform.position;
        deathParticlePosition.y = deathParticlePosition.y + 0.5f;
        Instantiate(deathEffect, deathParticlePosition, Quaternion.identity);

        //Destroy self when all is done.
        Debug.Log("Structure destroyed! Hopefully I don't exist anymore.");
        Destroy(gameObject);
    }
}
