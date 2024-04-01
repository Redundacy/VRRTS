using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Piece", menuName = "Game/GamePiece")]
public class GamePieceData : ScriptableObject
{
    public string gamePieceName;
    public int maxHealth;
    public GameObject model;
    public enum Team
    {
        AlliedTeam,
        EnemyTeam,
        Hostile,
        Neutral
    }
    public Team team;
    public override string ToString()
    {
        return gamePieceName;
    }

    public Team GetTeam()
    {
        return team;
    }
    public string GetTeamString()
    {
        return team.ToString();
    }

    public void SetTeam(string teamToSet)
    {
        switch(teamToSet)
        {
            case "AlliedTeam":
                team = Team.AlliedTeam;
                break;
            case "EnemyTeam":
                team = Team.EnemyTeam;
                break;
            case "Hostile":
                team = Team.Hostile;
                break;
            case "Neutral":
                team = Team.Neutral;
                break;
        }
    }
}
