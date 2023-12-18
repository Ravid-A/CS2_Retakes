using System.Text.Json;
using CounterStrikeSharp.API.Modules.Utils;

using Spawns;

using Retakes;
using static Retakes.Core;

namespace Configs;

public class Configs
{
    private static void CreateConfigsDirectory()
    {
        var configPath = Path.Combine(_plugin.ModuleDirectory, "configs/");

        if (!Directory.Exists(configPath))
        {
            Directory.CreateDirectory(configPath);
        }
    }

    public static void LoadConfig()
    {
        CreateConfigsDirectory();

        var configPath = Path.Combine(_plugin.ModuleDirectory, "configs/config.json");
        if (!File.Exists(configPath)) CreateConfig(configPath);

        var config = JsonSerializer.Deserialize<MainConfig>(File.ReadAllText(configPath))!;

        main_config = new Config(config);
    }

    private static void CreateConfig(string configPath)
    {
        var config = new MainConfig
        {
            prefixs = new PREFIXS
            {
                PREFIX = " \x04[Retakes]]\x01",
                PREFIX_CON = "[Retakes]",
                PREFIX_MENU = " \x04[Retakes]]\x01"
            },
            DEBUG = false,
            use_db = false
        };

        File.WriteAllText(configPath,
            JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
    }

    public static DBConfig LoadDBConfig()
    {
        CreateConfigsDirectory();

        var configPath = Path.Combine(_plugin.ModuleDirectory, "configs/database.json");
        if (!File.Exists(configPath)) return CreateDBConfig(configPath);

        var config = JsonSerializer.Deserialize<DBConfig>(File.ReadAllText(configPath))!;

        return config;
    }

    private static DBConfig CreateDBConfig(string configPath)
    {
        var config = new DBConfig
        {
            Connection = new ConnectionConfig
            {
                Host = "",
                Database = "",
                User = "",
                Password = "",
                Port = 3306
            }
        };

        File.WriteAllText(configPath,
            JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
        return config;
    }

    public static void LoadSpawns(string mapName)
    {
        if(spawnPoints == null!)
        {
            spawnPoints = new SpawnPoints();
        }

        spawnPoints.ClearSpawns();

        if(main_config.use_db)
        {
            db.Query(SQL_LoadSpawns_CB, $"SELECT * FROM `spawns` WHERE `map` = '{mapName}'");
        }
        else
        {
            LoadSpawnsFromFile(mapName);
        }
    }

    private static void LoadSpawnsFromFile(string mapName)
    {
        CreateConfigsDirectory();

        var configDir = Path.Combine(_plugin.ModuleDirectory, $"configs/spawns");

        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
        }

        var configPath = Path.Combine(_plugin.ModuleDirectory, $"configs/spawns/{mapName}.json");

        if (!File.Exists(configPath)) 
        {
            CreateSpawnsConfig(configPath).ConvertToSpawnPoints(configPath);
            return;
        }

        var config = JsonSerializer.Deserialize<SpawnsConfig>(File.ReadAllText(configPath))!;
        config.ConvertToSpawnPoints(configPath);
    }

    private static SpawnsConfig CreateSpawnsConfig(string configPath)
    {
        var config = new SpawnsConfig
        {
            Spawns = new List<SpawnConfig>
            {
                new SpawnConfig
                {
                    position = "",
                    angles = "",
                    team = (int)CsTeam.None,
                    site = (int)Site.A,
                    isBombsite = false
                },
            }
        };

        File.WriteAllText(configPath,
            JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));

        return config;
    }
}