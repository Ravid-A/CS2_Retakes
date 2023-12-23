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

    public void ConvertToSpawnPoints()
    {
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
    public string position { get; init;} = string.Empty;
    public string angles { get; init;} = string.Empty;
    public int team { get; init;} = (int)CsTeam.None;
    public int site { get; init;} = (int)Site.A;
    public bool isBombsite { get; init;} = false;
}