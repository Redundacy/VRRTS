using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //General variables I think the game is gonna need.
    public float playerResources;
    public int playerUnitCount;

    public float enemyResources;
    public int enemyUnitCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GivePlayerResources(float resourceAmount)
    {
        playerResources += resourceAmount;
    }
}
