using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

using Retakes;
using static Retakes.Functions;

namespace Spawns;

public class Spawn
{ 
    public int id;
    public Vector position;
    public QAngle angles;
    public CsTeam team;
    public Site site;
    public bool isBombsite;
    
    public Spawn(Vector position, QAngle angles, CsTeam team = CsTeam.None, Site site = Site.A, bool isBombsite = false)
    {
        this.position = position;
        this.angles = angles;
        this.team = team;
        this.site = site;
        this.isBombsite = isBombsite;
    }

    public Spawn(int id, Vector position, QAngle angles, CsTeam team = CsTeam.None, Site site = Site.A, bool isBombsite = false)
    {
        this.id = id;
        this.position = position;
        this.angles = angles;
        this.team = team;
        this.site = site;
        this.isBombsite = isBombsite;
    }

    public Spawn(int id, string position, string angles, int team = (int)CsTeam.None, int site = (int)Site.A, bool isBombsite = false)
    {
        this.id = id;

        if(position == string.Empty || angles == string.Empty)
            ThrowError("Invalid spawn position or angles");

        string[] position_array = position.Split(" ");
        this.position = new Vector(float.Parse(position_array[0]), float.Parse(position_array[1]), float.Parse(position_array[2]));

        string[] angles_array = angles.Split(" ");
        this.angles = new QAngle(float.Parse(angles_array[0]), float.Parse(angles_array[1]), float.Parse(angles_array[2]));

        this.team = (CsTeam)team;
        this.site = (Site)site;
        this.isBombsite = isBombsite;
    }

    public Spawn(string position, string angles, int team = (int)CsTeam.None, int site = (int)Site.A, bool isBombsite = false)
    {
        if(position == string.Empty || angles == string.Empty)
            ThrowError("Invalid spawn position or angles");

        string[] position_array = position.Split(" ");
        this.position = new Vector(float.Parse(position_array[0]), float.Parse(position_array[1]), float.Parse(position_array[2]));

        string[] angles_array = angles.Split(" ");
        this.angles = new QAngle(float.Parse(angles_array[0]), float.Parse(angles_array[1]), float.Parse(angles_array[2]));

        this.team = (CsTeam)team;
        this.site = (Site)site;
        this.isBombsite = isBombsite;
    }

    public void Teleport(CCSPlayerController player)
    {
        player.PlayerPawn!.Value!.Teleport(position, angles, new(0f, 0f, 0f));
    }
}