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

    public float enemyResources;
    public int enemyUnitCount;

    public GameObject playerInfo; //change later

    public GameObject UnitPrefab;

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

    void StartTheGame()
    {
        playerInfo.transform.Find("Resource Count").GetComponent<TMP_Text>().text = playerResources.ToString();
        List<GameObject> structures = new List<GameObject>(GameObject.FindGameObjectsWithTag("Structure"));
        playerInfo.transform.Find("Structure Count").GetComponent<TMP_Text>().text = structures.FindAll((GameObject obj) => obj.GetComponent<Structures>().structure.GetTeamString() == "AlliedTeam").Count.ToString();
    
        //Spawn in AllyCommandTower?
        //Spawn in EnemyCommandTower?
        //Give player and enemy starting resources?
        //Instantiate starting units and structures for both players?
        //Start playing music?
        //Activate enemy AI?
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
}
