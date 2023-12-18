using System.Text.Json;
using CounterStrikeSharp.API.Modules.Utils;
using Spawns;

using Retakes;
using static Retakes.Core;
using static Retakes.Functions;

namespace Configs;

public class SpawnsConfig
{
    public List<SpawnConfig> Spawns {get; set;} = null!;

    public void ConvertToSpawnPoints(string configpath)
    {
        spawnPoints.spawnsPath = configpath;

        int counter = 0;

        foreach (var spawn in Spawns)
        {
            if(spawn.position == string.Empty || spawn.angles == string.Empty)
            {
                PrintToServer($"Invalid spawn: Position: \"{spawn.position}\", Angles: \"{spawn.angles}\", Team: {spawn.team}, Site: {spawn.site}, isBombsite: {spawn.isBombsite} | SKIPPING...", ConsoleColor.Red);
                continue;
            }

            spawnPoints.AddSpawn(new Spawn(counter++, spawn.position, spawn.angles, spawn.team, spawn.site, spawn.isBombsite));
        }

        PrintToServer($"Loaded {spawnPoints.spawns.Count} spawns");
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
                site = (int)spawn.site,
                isBombsite = spawn.isBombsite
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
    public bool isBombsite { get; init;} = false;
}