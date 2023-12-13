using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using MySqlConnector;

using static Retakes.Core;

namespace Retakes;

class Spawn
{ 
    public float[] position = new float[3];
    public float[] angles = new float[3];
    public CsTeam team
    {
        get { return team; }
        set { team = value; }
    }
    public Site site
    {
        get { return site; }
        set { site = value; }
    }

    public enum Site
    {
        A,
        B
    }
    
    public Spawn(float[] position, float[] angles, CsTeam team = CsTeam.None, Site site = Site.A)
    {
        this.position = position;
        this.angles = angles;
        this.team = team;
        this.site = site;
    }
}

class SpawnsConfig
{
    public Spawn[] spawns;
    public SpawnsConfig(string filepath)
    {
        this.spawns = new Spawn[1];

        this.spawns[0] = new Spawn(new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 });

        //TODO load from config by filepath
    }
}