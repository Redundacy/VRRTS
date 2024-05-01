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

    public NetworkObject AlliedUnitPrefab;
    public NetworkObject EnemyUnitPrefab;

    public override void OnNetworkSpawn()
    {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        if(IsHost)
        {
            foreach (ulong player in NetworkManager.Singleton.ConnectedClientsIds)
            {
                playersInGame.Add(player);
                resourcesPerPlayer.Add(100);
            }
        }
    }


    [ServerRpc(RequireOwnership =false)]
    private void SpawnPlayerServerRpc(ulong playerId)
    {
        var spawn = Instantiate(playerPrefab);
        spawn.SpawnWithOwnership(playerId);
        Debug.Log(NetworkManager.Singleton.ConnectedClientsIds.Count);
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
        if(IsClient)
        {
            MakeGuyClientRpc(1, "EnemyTeam", boughtUnit.name, spawnPoint);
        } else
        {
            MakeGuyClientRpc(OwnerClientId, team, boughtUnit.name, spawnPoint);
        }
        //if (playerResources >= boughtUnit.cost)
        //{
        //    playerResources -= boughtUnit.cost;
            
        //    playerInfo.transform.Find("Resource Count").GetComponent<TMP_Text>().text = playerResources.ToString();
        //}
    }

    [ClientRpc]
    private void MakeGuyClientRpc(ulong playerId, string team, string boughtUnit, Vector3 spawnPoint)
    {
        UnitData foundData = Resources.Load<UnitData>($"Units/{boughtUnit}");
        foreach (ulong player in playersInGame)
        {
            Debug.Log("player " + player);
        }
        if (resourcesPerPlayer[playersInGame.IndexOf(playerId)] >= foundData.cost)
        {
            if (IsHost)
            {
                resourcesPerPlayer[playersInGame.IndexOf(playerId)] -= foundData.cost;
            }
            Debug.Log("check paid for");
        }
        if(playerId == playersInGame[0])
        {
            //allied team
            NetworkObject createdGuy = Instantiate(AlliedUnitPrefab, spawnPoint, new Quaternion());
            createdGuy.SpawnWithOwnership(playerId);
            foundData.SetTeam(team);
            createdGuy.GetComponent<Units>().unit = foundData;
            createdGuy.GetComponent<Units>().InitializeData();
        } else if (playerId == playersInGame[1])
        {
            //enemy team
            NetworkObject createdGuy = Instantiate(EnemyUnitPrefab, spawnPoint, new Quaternion());
            createdGuy.SpawnWithOwnership(playerId);
            foundData.SetTeam(team);
            createdGuy.GetComponent<Units>().unit = foundData;
            createdGuy.GetComponent<Units>().InitializeData();
        }
    }
}
