using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OpponentBrain : MonoBehaviour
{

    bool readyToSpawnGuy;
    bool readyToMaybeAttack;
    public bool gameIsStarted;
    [SerializeField] GameObject gameManagerObject;
    GameManager gameManager;
    List<EnemySpawnPoints> spawnPoints;
    List<UnitData> spawnableUnits;
    int numSpawnPoints;
    [SerializeField] GameObject attackPoint;

    [SerializeField] UnitData BowerData;
    [SerializeField] UnitData PuncherData;

    [SerializeField] float spawnTimer;
    [SerializeField] float attackTimer;

    [SerializeField] int oddsToAttack;

    public static event Action<Vector3> Muster;
    public static event Action<GameObject> AttackPlayer;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize a bunch of bizz
        gameManager = gameManagerObject.GetComponent<GameManager>();
        spawnPoints = new List<EnemySpawnPoints>(FindObjectsOfType<EnemySpawnPoints>());
        Debug.Log("Found enemy spawns: " + spawnPoints.Count);
        numSpawnPoints= spawnPoints.Count;
        spawnableUnits = new List<UnitData>();
        spawnableUnits.Add(BowerData);
        spawnableUnits.Add(PuncherData);
        readyToSpawnGuy= true;
        readyToMaybeAttack= true;
        gameIsStarted = false; ;
    }

    // Update is called once per frame
    void Update()
    {

        if (gameIsStarted)
        {
            //Basically a loop to invoke spawning and attacking on a timer
            if (readyToSpawnGuy)
            {
                readyToSpawnGuy = false;
                Invoke("SpawnGuy", spawnTimer);
            }

            if (readyToMaybeAttack)
            {
                readyToMaybeAttack = false;
                Invoke("MaybeAttack", attackTimer);
            }
        }
    }

    void SpawnGuy()
    {
        //Find a random spawn point and randomize which unit is being spawned, then spawn that unit at that spawn point.
        Debug.Log("Attempting to Spawn guy.");
        int spawnLocationIndex = UnityEngine.Random.Range(0, numSpawnPoints);
        int unitToSpawnIndex = UnityEngine.Random.Range(0, spawnableUnits.Count);
        gameManager.RequestMakeGuyFree("EnemyTeam", spawnableUnits[unitToSpawnIndex], spawnPoints[spawnLocationIndex].gameObject.transform.position);
        readyToSpawnGuy = true;
    }

    void MaybeAttack()
    {
        //If the random int is 0, the AI attacks. It attacks by sending its units towards the player base.
        Debug.Log("Attempting to attack...");
        int attackIfZero = UnityEngine.Random.Range(0, oddsToAttack);
        if (attackIfZero == 0)
        {
            Debug.Log("Attacking!!!");
            //Find half of the enemy units and send them towards an attack point, near the enemy base likely.
            GameObject[] allUnits = GameObject.FindGameObjectsWithTag("Unit");
            List<GameObject> allEnemyUnits = new List<GameObject>();
            foreach (GameObject unit in allUnits)
            {
                if (unit.GetComponent<Units>().unit.GetTeamString() == "EnemyTeam")
                {
                    allEnemyUnits.Add(unit);
                }
            }
            int amountOfUnitsToAttack = allEnemyUnits.Count/2;

            for (int i = 0; i < amountOfUnitsToAttack; i++)
            {
                //Call each of these Units to move towards the attack position
            }
        }
        else
        {
            Debug.Log("Did not attack...");
        }
    }

    public void ActivateAI()
    {
        gameIsStarted= true;
    }
}
