using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace Spawns;

public class Spawn
{ 
    public Vector position;
    public QAngle angles;
    public CsTeam team;
    public Site site;

    public enum Site
    {
        A,
        B
    }
    
    public Spawn(Vector position, QAngle angles, CsTeam team = CsTeam.None, Site site = Site.A)
    {
        this.position = position;
        this.angles = angles;
        this.team = team;
        this.site = site;
    }

    public void Teleport(CCSPlayerController player)
    {
        player.Teleport(position, angles, new Vector(0, 0, 0));
    }
}