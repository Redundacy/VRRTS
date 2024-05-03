using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class matchUnitValues : NetworkBehaviour
{
    public enum Team
    {
        AlliedTeam,
        EnemyTeam,
        Hostile,
        Neutral
    }
    public NetworkVariable<Team> team;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTeam(string teamToSet)
    {
        switch (teamToSet)
        {
            case "AlliedTeam":
                team.Value = Team.AlliedTeam;
                break;
            case "EnemyTeam":
                team.Value = Team.EnemyTeam;
                SetTeamClientRpc(teamToSet);
                break;
            case "Hostile":
                team.Value = Team.Hostile;
                break;
            case "Neutral":
                team.Value = Team.Neutral;
                break;
        }
    }

    [ClientRpc]
    public void SetTeamClientRpc(string team)
    {
        GetComponent<Units>().SetTeam(team);
    }
}
