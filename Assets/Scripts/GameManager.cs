using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Matchmaker.Models;
using Valve.VR;
using UnityEngine.SceneManagement;

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
    public GameObject playerCommandTowerPrefab;
    public GameObject enemyCommandTowerPrefab;
    public UnitData startingUnitsData;

    //Locations. Used to instatiate things and move things.
    public GameObject playerCommandTowerSpawnLocation;
    public GameObject enemyCommandTowerSpawnLocation;

    //List of starting spawn locations
    List<PlayerSpawnPointsStarting> playerSpawnPointsStarting;
    List<EnemySpawnPointsStarting> enemySpawnPointsStarting;

    [SerializeField] GameObject opponentBrainObject;

    // Start is called before the first frame update
    void Start()
    {
        playerInfo.transform.Find("Resource Count").GetComponent<TMP_Text>().text = playerResources.ToString();
        List<GameObject> structures = new List<GameObject>(GameObject.FindGameObjectsWithTag("Structure"));
        playerInfo.transform.Find("Structure Count").GetComponent<TMP_Text>().text = structures.FindAll((GameObject obj) => obj.GetComponent<Structures>().structure.GetTeamString() == "AlliedTeam").Count.ToString();

        //Get a list of all starting spawn points to initialize the guys for both teams. Will need to be adjusted if more than one on one.
        playerSpawnPointsStarting = new List<PlayerSpawnPointsStarting>(FindObjectsOfType<PlayerSpawnPointsStarting>());
        enemySpawnPointsStarting = new List<EnemySpawnPointsStarting>(FindObjectsOfType<EnemySpawnPointsStarting>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Called when the player presses the Start Game button, instantiates all the necessary gameplay stuff. Potentially want different start game functions for different maps?
    public void StartTheGame()
    {
        playerInfo.transform.Find("Resource Count").GetComponent<TMP_Text>().text = playerResources.ToString();
        List<GameObject> structures = new List<GameObject>(GameObject.FindGameObjectsWithTag("Structure"));
        playerInfo.transform.Find("Structure Count").GetComponent<TMP_Text>().text = structures.FindAll((GameObject obj) => obj.GetComponent<Structures>().structure.GetTeamString() == "AlliedTeam").Count.ToString();

        //TEST: Spawn in AllyCommandTower?
        Instantiate(playerCommandTowerPrefab, playerCommandTowerSpawnLocation.transform.position, Quaternion.identity);
        //TEST: Spawn in EnemyCommandTower?
        //Instantiate(enemyCommandTowerPrefab, enemyCommandTowerSpawnLocation.transform.position, Quaternion.identity);
        //TEST: Give player and enemy starting resources?
        playerResources = playerStartingResources;
        enemyResources = enemyStartingResources;
        //TEST: Instantiate starting units and structures for both players?
        foreach(PlayerSpawnPointsStarting spawnPoint in playerSpawnPointsStarting)
        {
            RequestMakeGuy("AlliedTeam", startingUnitsData, spawnPoint.gameObject.transform.position);
        }

        foreach (EnemySpawnPointsStarting spawnPoint in enemySpawnPointsStarting)
        {
            RequestMakeGuy("EnemyTeam", startingUnitsData, spawnPoint.gameObject.transform.position);
        }
        //Start playing music? (POLISH)
        //Activate enemy AI? (Maybe not doing? Instead we make it a two player experience?)
        opponentBrainObject.GetComponent<OpponentBrain>().ActivateAI();
    }

    public void EndTheGame(bool wonTheGame)
    {
        //CURRENTLY accepts a boolean for win or lose.
        //Do some celebratory stuff, play a bunch of confetti particles around the player and applaud if they won or something, wah wah trumpet if they lose. (POLISH)
        //Display a "YOU WIN" or "YOU LOSE" in the player's face.
        //TEST: After some time has passed, load the same scene again to reset everything.
        Invoke("FadeOutToResetGame", 5.0f);
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
            GameObject createdGuy = Instantiate(boughtStructure.model, buildPoint, new Quaternion());
            boughtStructure.SetTeam(team);
            createdGuy.GetComponent<Structures>().structure = boughtStructure;
            createdGuy.GetComponent<Structures>().InitializeData();
            playerInfo.transform.Find("Resource Count").GetComponent<TMP_Text>().text = playerResources.ToString();
        }
    }

    void FadeOutToResetGame()
    {
        SteamVR_Fade.View(Color.black, 0.5f);
        Invoke("ResetGame", 0.5f);
    }

    void ResetGame()
    {
        //Replace thing with either index or name of scene that is being played in.
        //SceneManager.LoadScene(1);
    }
}
