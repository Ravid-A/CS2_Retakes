using System.Text.Json;
using CounterStrikeSharp.API.Modules.Utils;

using Spawns;

using Retakes;
using static Retakes.Core;

namespace Configs;

public class Config
{
    public string PREFIX;
    public string PREFIX_CON;
    public string PREFIX_MENU;

    public bool use_db = true;

    public int WARMUP_TIME = 12;
    public int MAX_PLAYERS = 9;
    public int MIN_PLAYERS = 2;
    public int ROUND_TIME = 12;
    public bool DEBUG;

    public bool auto_plant = true;
    public bool insta_plant = true;
    public bool insta_defuse = true;
    public bool explode_no_time = true;

    public float TerrroristRatio = 0.45f;

    public Config(MainConfig config)
    {
        PREFIX = config.prefixs.PREFIX;
        PREFIX_CON = config.prefixs.PREFIX_CON;
        PREFIX_MENU = config.prefixs.PREFIX_MENU;
        use_db = config.use_db;
        DEBUG = config.DEBUG;
        WARMUP_TIME = config.WARMUP_TIME;
        MAX_PLAYERS = config.MAX_PLAYERS;
        MIN_PLAYERS = config.MIN_PLAYERS;
        ROUND_TIME = config.ROUND_TIME;
        auto_plant = config.auto_plant;
        insta_plant = config.insta_plant;
        insta_defuse = config.insta_defuse;
        explode_no_time = config.explode_no_time;
        TerrroristRatio = config.TerrroristRatio;
    }
}

public class Configs
{
    public static void CreateConfigsDirectory()
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
        var config = new MainConfig(new PREFIXS());

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
            Connection = new ConnectionConfig()
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

        spawnPoints.BuildPath();
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
        string configPath = spawnPoints.spawnsPath;       

        if (!File.Exists(configPath)) 
        {
            CreateSpawnsConfig(configPath).ConvertToSpawnPoints();
            return;
        }

        var config = JsonSerializer.Deserialize<SpawnsConfig>(File.ReadAllText(configPath))!;
        config.ConvertToSpawnPoints();
    }

    private static SpawnsConfig CreateSpawnsConfig(string configPath)
    {
        var config = new SpawnsConfig
        {
            Spawns = new List<SpawnConfig>
            {
                new SpawnConfig(),
            }
        };

        File.WriteAllText(configPath,
            JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));

        return config;
    }
}