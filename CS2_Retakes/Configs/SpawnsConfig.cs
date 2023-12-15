using System.Text.Json;
using CounterStrikeSharp.API.Modules.Utils;
using Spawns;

using static Retakes.Core;

namespace Configs;

public class SpawnsConfig
{
    public List<SpawnConfig> Spawns {get; set;} = null!;

    public void ConvertToSpawnPoints(string configpath)
    {
        spawnPoints.spawnsPath = configpath;

        foreach (var spawn in Spawns)
        {
            if(spawn.position == string.Empty || spawn.angles == string.Empty)
            {
                Console.WriteLine($"Invalid spawn: Position: \"{spawn.position}\", Angles: \"{spawn.angles}\", Team: {spawn.team}, Site: {spawn.site} | SKIPPING...");
                continue;
            }

            spawnPoints.AddSpawn(new Spawn(spawn.position, spawn.angles, spawn.team, spawn.site));
        }

        Console.WriteLine($"Loaded {spawnPoints.spawns.Count} spawns");
    }

    public static void ConvertFromSpawnPoints()
    {
        List<SpawnConfig> spawns = new List<SpawnConfig>();

        foreach (var spawn in spawnPoints.spawns)
        {
            SpawnConfig spawnConfig = new SpawnConfig
            {
                position = spawn.position.ToString(),
                angles = spawn.angles.ToString(),
                team = (int)spawn.team,
                site = (int)spawn.site
            };

            spawns.Add(spawnConfig);
        }

        SpawnsConfig spawnsConfig = new SpawnsConfig
        {
            Spawns = spawns
        };

        string json = JsonSerializer.Serialize(spawnsConfig);

        File.WriteAllText(spawnPoints.spawnsPath, json);
    }
}

public class SpawnConfig
{
    public string position { get; init;} = "0 5 0";
    public string angles { get; init;} = "0 6 0";
    public int team { get; init;} = (int)CsTeam.None;
    public int site { get; init;} = (int)Site.A;
}