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
    public NetworkObject UnitPrefab;

    public override void OnNetworkSpawn()
    {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        foreach(ulong player in NetworkManager.Singleton.ConnectedClientsIds)
        {
            playersInGame.Add(player);
        }
    }


    [ServerRpc(RequireOwnership =false)]
    private void SpawnPlayerServerRpc(ulong playerId)
    {
        var spawn = Instantiate(playerPrefab);
        spawn.SpawnWithOwnership(playerId);
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
        //if (playerResources >= boughtUnit.cost)
        //{
        //    playerResources -= boughtUnit.cost;
            MakeGuyServerRpc(OwnerClientId, team/*, boughtUnit*/, spawnPoint);
        //    playerInfo.transform.Find("Resource Count").GetComponent<TMP_Text>().text = playerResources.ToString();
        //}
    }

    [ServerRpc(RequireOwnership =false)]
    private void MakeGuyServerRpc(ulong playerId, string team/*, UnitData boughtUnit*/, Vector3 spawnPoint)
    {
        NetworkObject createdGuy = Instantiate(UnitPrefab, spawnPoint, new Quaternion());
        createdGuy.SpawnWithOwnership(playerId);
        //boughtUnit.SetTeam(team);
        //createdGuy.GetComponent<Units>().unit = boughtUnit;
        //createdGuy.GetComponent<Units>().InitializeData();
    }
}
