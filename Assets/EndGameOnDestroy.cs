using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameOnDestroy : MonoBehaviour
{
    private void OnDestroy()
    {
        if (gameObject.name == "AllyCommandTower")   {
            GameObject.Find("GameManager").GetComponent<GameManager>().EndTheGame(false);
        }
        else if (gameObject.name == "EnemyCommandTower")
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().EndTheGame(true);
        }
    }
}
