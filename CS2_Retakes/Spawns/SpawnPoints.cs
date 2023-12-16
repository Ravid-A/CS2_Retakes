using CounterStrikeSharp.API.Core;

using static Retakes.Core;
using static Retakes.Functions;

using static Configs.SpawnsConfig;

namespace Spawns;

public class SpawnPoints
{
    public List<Spawn> spawns;

    public string spawnsPath = string.Empty;

    public SpawnPoints()
    {
        spawns = new List<Spawn>();
    }

    public int Length => spawns.Count;

    public void AddSpawn(Spawn spawn, bool insert = false)
    {
        if(main_config.DEBUG)
        {
            PrintToServer($"Adding spawn: {spawn.position} {spawn.angles} {spawn.team} {spawn.site}");
        }

        spawns.Add(spawn);

        if(insert)
        {
            int index = spawns.IndexOf(spawn);

            if(main_config.use_db)
            {
                db.InsertSpawn(spawn, index);
                return;
            }

            ConvertFromSpawnPoints();
        }
    }

    public void RemoveSpawn(Spawn spawn)
    {
        if(main_config.DEBUG)
        {
            PrintToServer($"Removing spawn: {spawn.position} {spawn.angles} {spawn.team} {spawn.site}");
        }

        spawns.Remove(spawn);

        if(main_config.use_db)
        {
            db.DeleteSpawn(spawn);
            return;
        }

        ConvertFromSpawnPoints();
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

        Spawn spawn = spawns[index];

        spawns.RemoveAt(index);

        if(main_config.use_db)
        {
            db.DeleteSpawn(spawn);
            return;
        }

        ConvertFromSpawnPoints();
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