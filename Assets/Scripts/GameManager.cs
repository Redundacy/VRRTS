using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Matchmaker.Models;

public class GameManager : MonoBehaviour
{
    //General variables I think the game is gonna need.
    public float playerResources;
    public int playerUnitCount;
    //Resources player should start with when they start the game.
    public int playerStartingResources;

    public float enemyResources;
    public int enemyUnitCount;
    //Resources enemy should start with when they start the game.
    public int enemyStartingResources;

    public GameObject playerInfo; //change later

    public GameObject UnitPrefab;

    //Prefabs to spawn in and stuff
    public GameObject playerCommandTower;
    public GameObject enemyCommandTower;
    public UnitData startingUnitsData;

    //Locations. Used to instatiate things and move things.
    public GameObject playerCommandTowerSpawnLocation;
    public GameObject enemyCommandTowerSpawnLocation;

    // Start is called before the first frame update
    void Start()
    {
        playerInfo.transform.Find("Resource Count").GetComponent<TMP_Text>().text = playerResources.ToString();
        List<GameObject> structures = new List<GameObject>(GameObject.FindGameObjectsWithTag("Structure"));
        playerInfo.transform.Find("Structure Count").GetComponent<TMP_Text>().text = structures.FindAll((GameObject obj) => obj.GetComponent<Structures>().structure.GetTeamString() == "AlliedTeam").Count.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Called when the player presses the Start Game button, instantiates all the necessary gameplay stuff. Potentially want different start game functions for different maps?
    void StartTheGame()
    {
        playerInfo.transform.Find("Resource Count").GetComponent<TMP_Text>().text = playerResources.ToString();
        List<GameObject> structures = new List<GameObject>(GameObject.FindGameObjectsWithTag("Structure"));
        playerInfo.transform.Find("Structure Count").GetComponent<TMP_Text>().text = structures.FindAll((GameObject obj) => obj.GetComponent<Structures>().structure.GetTeamString() == "AlliedTeam").Count.ToString();

        //Spawn in AllyCommandTower?
        Instantiate(playerCommandTower, playerCommandTowerSpawnLocation.transform.position, Quaternion.identity);
        //Spawn in EnemyCommandTower?
        Instantiate(enemyCommandTower, enemyCommandTowerSpawnLocation.transform.position, Quaternion.identity);
        //Give player and enemy starting resources?
        playerResources = playerStartingResources;
        enemyResources = enemyStartingResources;
        //Instantiate starting units and structures for both players?
        //Start playing music?
        //Activate enemy AI?
    }

    public void EndTheGame(bool wonTheGame)
    {
        //CURRENTLY accepts a boolean for win or lose.
        //Do some celebratory stuff, play a bunch of confetti particles around the player and applaud if they won or something, wah wah trumpet if they lose.
        //Display a "YOU WIN" or "YOU LOSE" in the player's face.
        //After some time has passed, load the same scene again to reset everything.
    }

    public void GivePlayerResources(float resourceAmount)
    {
        playerResources += resourceAmount;
        playerInfo.transform.Find("Resource Count").GetComponent<TMP_Text>().text = playerResources.ToString();
    }

    public void RequestMakeGuy(string team, UnitData boughtUnit, Vector3 spawnPoint)
    {
        if(playerResources>= boughtUnit.cost)
        {
            playerResources -= boughtUnit.cost;
            GameObject createdGuy = Instantiate(UnitPrefab, spawnPoint, new Quaternion());
            boughtUnit.SetTeam(team);
            createdGuy.GetComponent<Units>().unit = boughtUnit;
            createdGuy.GetComponent<Units>().InitializeData();
            playerInfo.transform.Find("Resource Count").GetComponent<TMP_Text>().text = playerResources.ToString();
        }
    }

    public void RequestMakeStructure(string team, StructureData boughtStructure, Vector3 buildPoint)
    {
        if (playerResources >= boughtStructure.cost)
        {
            playerResources -= boughtStructure.cost;
            GameObject createdGuy = Instantiate(UnitPrefab, buildPoint, new Quaternion());
            boughtStructure.SetTeam(team);
            createdGuy.GetComponent<Structures>().structure = boughtStructure;
            createdGuy.GetComponent<Structures>().InitializeData();
            playerInfo.transform.Find("Resource Count").GetComponent<TMP_Text>().text = playerResources.ToString();
        }
    }
}
