using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Services.Authentication;

public class NetworkGameManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;

    [SerializeField] private NetworkList<ulong> playersInGame = new NetworkList<ulong>();
    [SerializeField] private NetworkList<int> resourcesPerPlayer = new NetworkList<int>();

    public NetworkObject UnitPrefab;

    public override void OnNetworkSpawn()
    {
        Debug.Log("me:" + NetworkManager.Singleton.LocalClientId);
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }


    [ServerRpc(RequireOwnership =false)]
    private void SpawnPlayerServerRpc(ulong playerId)
    {
        var spawn = Instantiate(playerPrefab);
        spawn.SpawnWithOwnership(playerId);
        if(!playersInGame.Contains(playerId))
        {
            playersInGame.Add(playerId);
            resourcesPerPlayer.Add(100);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void RequestMakeGuy(string team, UnitData boughtUnit, Vector3 spawnPoint)
    {
        if(!IsHost)
        {
            MakeGuyServerRpc(1, "EnemyTeam", boughtUnit.name, spawnPoint);
        } else
        {
            MakeGuyServerRpc(0, "AlliedTeam", boughtUnit.name, spawnPoint);
        }
        //if (playerResources >= boughtUnit.cost)
        //{
        //    playerResources -= boughtUnit.cost;
            
        //    playerInfo.transform.Find("Resource Count").GetComponent<TMP_Text>().text = playerResources.ToString();
        //}
    }

    [ServerRpc(RequireOwnership =false)]
    private void MakeGuyServerRpc(ulong playerId, string team, string boughtUnit, Vector3 spawnPoint)
    {
        UnitData foundData = Resources.Load<UnitData>($"Units/{boughtUnit}");
        //foreach (ulong player in playersInGame)
        //{
        //    Debug.Log("player " + player);
        //}
        if (resourcesPerPlayer[playersInGame.IndexOf(playerId)] >= foundData.cost)
        {
            resourcesPerPlayer[playersInGame.IndexOf(playerId)] -= foundData.cost;
            Debug.Log("check paid for");
        } else
        {
            Debug.Log("not enough money");
            return;
        }
        
        NetworkObject createdGuy = Instantiate(UnitPrefab, spawnPoint, new Quaternion());
        createdGuy.SpawnWithOwnership(playerId);
        createdGuy.GetComponent<Units>().unit = foundData;
        createdGuy.GetComponent<Units>().SetTeam(team);
        InitGuyClientRpc(boughtUnit, team);
    }

    [ClientRpc]
    private void InitGuyClientRpc(string boughtUnit, string team)
    {
        Debug.Log("is this running");
        UnitData foundData = Resources.Load<UnitData>($"Units/{boughtUnit}");
        foundData.SetTeam(team);//replace later
        Units[] allUnits = GameObject.FindObjectsOfType<Units>();
        Units ourGuy = allUnits[allUnits.Length - 1];
        ourGuy.unit = foundData;
        Debug.Log("is this running " + ourGuy.unit.ToString());
        ourGuy.InitializeData();
    }
}
