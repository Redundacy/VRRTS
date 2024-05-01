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
    private NetworkVariable<NetworkObject> createdGuy;

    public override void OnNetworkSpawn()
    {
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
        
        createdGuy.Value = Instantiate(UnitPrefab, spawnPoint, new Quaternion());
        createdGuy.Value.SpawnWithOwnership(playerId);
        InitGuyClientRpc(boughtUnit, team);
    }

    [ClientRpc]
    private void InitGuyClientRpc(string boughtUnit, string team)
    {
        UnitData foundData = Resources.Load<UnitData>($"Units/{boughtUnit}");
        foundData.SetTeam(team);
        createdGuy.Value.GetComponent<Units>().unit = foundData;
        createdGuy.Value.GetComponent<Units>().InitializeData();
    }
}
