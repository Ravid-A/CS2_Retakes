using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

using MySqlConnector;

using static Retakes.Core;
using static Retakes.Functions;

namespace Retakes;

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

public class SpawnPoints
{
    public List<Spawn> spawns;

    public SpawnPoints()
    {
        spawns = new List<Spawn>();
    }

    public void LoadSpawnsFromDB(Database db, string mapName)
    {

        string query = $"SELECT * FROM spawns WHERE map = '{mapName}'";
        PrintToServer($"Loading spawns from database, query: {query}");
        db.Query(SQL_LoadSpawnsCallback, query);
    }

    private void SQL_LoadSpawnsCallback(MySqlConnection sqlConnection, MySqlDataReader reader, Exception exception, dynamic data)
    {
        if(exception != null!)
        {
            ThrowError(exception);
            return;
        }

        if(reader.HasRows)
        {
            while(reader.Read())
            {
                string position = reader.GetString(2);
                string angles = reader.GetString(3);
                int team = reader.GetInt32(4);
                int site = reader.GetInt32(5);

                if(!Enum.IsDefined(typeof(CsTeam), team))
                {
                    ThrowError($"[SQL_LoadSpawnsCallback] Invalid team: {team}");
                    continue;
                }

                if(!Enum.IsDefined(typeof(Spawn.Site), site))
                {
                    ThrowError($"[SQL_LoadSpawnsCallback] Invalid site: {site}");
                    continue;
                }

                if(position == null!)
                {
                    ThrowError($"[SQL_LoadSpawnsCallback] Invalid position: {position}");
                    continue;
                }

                if(angles == null!)
                {
                    ThrowError($"[SQL_LoadSpawnsCallback] Invalid angles: {angles}");
                    continue;
                }

                string[] pos_arr = position.Split(' ');
                string[] ang_arr = angles.Split(' ');

                Vector pos = new Vector(float.Parse(pos_arr[0]), float.Parse(pos_arr[1]), float.Parse(pos_arr[2]));
                QAngle ang = new QAngle(float.Parse(ang_arr[0]), float.Parse(ang_arr[1]), float.Parse(ang_arr[2]));

                Spawn spawn = new Spawn(pos, ang, (CsTeam)team, (Spawn.Site)site);

                AddSpawn(spawn);
            }
        }
    }

    public void AddSpawn(Spawn spawn)
    {
        if(main_config.DEBUG)
        {
            PrintToServer($"Adding spawn: {spawn.position} {spawn.angles} {spawn.team} {spawn.site}");
        }

        spawns.Add(spawn);
    }

    public void RemoveSpawn(Spawn spawn)
    {
        if(main_config.DEBUG)
        {
            PrintToServer($"Removing spawn: {spawn.position} {spawn.angles} {spawn.team} {spawn.site}");
        }

        spawns.Remove(spawn);
    }

    public void RemoveSpawn(int index)
    {
        if(index < 0 || index > spawns.Count)
        {
            ThrowError($"Invalid spawn index: {index}");
            return;
        }

        if(main_config.DEBUG)
        {
            PrintToServer($"Removing spawn: {spawns[index].position} {spawns[index].angles} {spawns[index].team} {spawns[index].site}");
        }

        spawns.RemoveAt(index);
    }

    public void ClearSpawns()
    {
        if(main_config.DEBUG)
        {
            PrintToServer($"Clearing spawns");
        }

        spawns.Clear();
    }

    public void TeleportToSpawn(CCSPlayerController player, int index)
    {
        if(index < 0 || index > spawns.Count)
        {
            ThrowError($"Invalid spawn index: {index}");
            return;
        }

        if(main_config.DEBUG)
        {
            PrintToServer($"Teleporting player to spawn {index}: {spawns[index].position} {spawns[index].angles} {spawns[index].team} {spawns[index].site}");
        }

        spawns[index].Teleport(player);
    }

    public void TeleportToSpawn(CCSPlayerController player, Spawn spawn)
    {
        if(main_config.DEBUG)
        {
            PrintToServer($"Teleporting player to spawn: {spawn.position} {spawn.angles} {spawn.team} {spawn.site}");
        }

        spawn.Teleport(player);
    }
}