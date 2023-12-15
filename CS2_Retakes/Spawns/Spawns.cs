using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace Spawns;

public enum Site
{
    A,
    B
}

public class Spawn
{ 
    public Vector position;
    public QAngle angles;
    public CsTeam team;
    public Site site;
    
    public Spawn(Vector position, QAngle angles, CsTeam team = CsTeam.None, Site site = Site.A)
    {
        this.position = position;
        this.angles = angles;
        this.team = team;
        this.site = site;
    }

    public Spawn(string position, string angles, int team = (int)CsTeam.None, int site = (int)Site.A)
    {
        string[] position_array = position.Split(" ");
        Console.WriteLine($"Position (\"{position}\"): {float.Parse(position_array[0])} {float.Parse(position_array[1])} {float.Parse(position_array[2])}");
        this.position = new Vector(float.Parse(position_array[0]), float.Parse(position_array[1]), float.Parse(position_array[2]));

        string[] angles_array = angles.Split(" ");
        this.angles = new QAngle(float.Parse(angles_array[0]), float.Parse(angles_array[1]), float.Parse(angles_array[2]));

        this.team = (CsTeam)team;
        this.site = (Site)site;
    }

    public void Teleport(CCSPlayerController player)
    {
        player.Teleport(position, angles, new Vector(0, 0, 0));
    }
}