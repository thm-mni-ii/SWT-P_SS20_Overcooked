using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Team : ScriptableObject
{
    [SerializeField] int teamID;
    [SerializeField] Color teamColor;



    private static Dictionary<int, Team> registeredTeams = new Dictionary<int, Team>();
    private static int nextTeamToJoin = 0;


    public static Team GetNextTeamToJoin()
    {
        Dictionary<int, Team>.Enumerator enumerator = Team.registeredTeams.GetEnumerator();
        int currentIndex = 0;
        while (enumerator.MoveNext())
        {
            if (currentIndex == (Team.nextTeamToJoin) % Team.registeredTeams.Count)
            {
                Team.nextTeamToJoin++;
                return enumerator.Current.Value;
            }
            currentIndex++;
        }

        return null;
    }
    public static Team GetByID(int teamID)
    {
        Team val;
        if (Team.registeredTeams.TryGetValue(teamID, out val))
            return val;
        else
            Debug.LogWarning($"No team found with ID {teamID}.");

        return null;
    }

    private static void RegisterTeam(Team team)
    {
        if (!Team.registeredTeams.ContainsKey(team.TeamID))
            Team.registeredTeams.Add(team.TeamID, team);
    }


    public int TeamID { get => this.teamID; }
    public TeamBase TeamBase { get; private set; }
    public Color TeamColor { get => this.teamColor; }


    public void Register(TeamBase teamBase)
    {
        this.TeamBase = teamBase;
        Team.RegisterTeam(this);
    }
}
