using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

using MySqlConnector;

using Configs;

using static Retakes.Commands;
using static Retakes.Events;
using static Retakes.Functions;

using Spawns;

namespace Retakes;

public class Core : BasePlugin
{
    public class Config
    {
        public string PREFIX;
        public string PREFIX_CON;

        public bool use_db = false;

        public bool DEBUG;

        public Config(MainConfig config)
        {
            PREFIX = config.prefixs.PREFIX;
            PREFIX_CON = config.prefixs.PREFIX_CON;
            use_db = config.use_db;
            DEBUG = config.DEBUG;
        }
    }

    public override string ModuleName => "Retakes Plugin";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Ravid";
    public override string ModuleDescription => "Retakes Plugin";

    public Database db = null!;   
    public static Config main_config = null!;

    public static SpawnPoints spawnPoints = null!;

    public override void Load(bool hotReload)
    {
        LoadConfig();

        Database.Connect(SQL_ConnectCallback, LoadDBConfig());

        RegisterCommands();
        RegisterEvents();

        RegisterListener<Listeners.OnMapStart>(OnMapStart);

        if(hotReload)
        {
            OnMapStart(Server.MapName);
        }
    }

    public override void Unload(bool hotReload)
    {
        // Unregister the command
        RemoveCommand("css_test", TestCommand);

        if(db != null!)
        {
            db.CloseConnection();
        }
    }

    public void OnMapStart(string mapName)
    {
        PrintToServer($"Map started: {mapName}");

        LoadSpawns(mapName);
    }

    private void RegisterCommands()
    {
        // Register the command
        AddCommand("css_test", "", TestCommand);
    }

    private void RegisterEvents()
    {
        // Register the event
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
    }

    private void SQL_ConnectCallback(MySqlConnection sqlConnection, Exception exception, dynamic data)
    {
        if(sqlConnection == null!)
        {
            ThrowError($"Failed to connect to database: {exception.Message}");
            return;
        }

        db = new Database(sqlConnection);

        PrintToServer($"Connected to database");

        db.Query(SQL_CheckForErrors, "CREATE TABLE IF NOT EXISTS `spawns` (`id` INT NOT NULL AUTO_INCREMENT, `map` VARCHAR(128) NOT NULL, `position` VARCHAR(64) NOT NULL, `angles` VARCHAR(64) NOT NULL, `team` INT NOT NULL, `site` INT NOT NULL, PRIMARY KEY (`id`)) ENGINE = InnoDB;");
    }

    private void SQL_CheckForErrors(MySqlConnection sqlConnection, MySqlDataReader reader, Exception exception, dynamic data)
    {
        if(exception != null!)
        {
            ThrowError($"Databse error, {exception.Message}");
            return;
        }
    } 

    public void CreateConfigsDirectory()
    {
        var configPath = Path.Combine(ModuleDirectory, "configs/");

        if (!Directory.Exists(configPath))
        {
            Directory.CreateDirectory(configPath);
        }
    }

    private void LoadConfig()
    {
        CreateConfigsDirectory();

        var configPath = Path.Combine(ModuleDirectory, "configs/config.json");
        if (!File.Exists(configPath)) CreateConfig(configPath);

        var config = JsonSerializer.Deserialize<MainConfig>(File.ReadAllText(configPath))!;

        main_config = new Config(config);
    }

    private void CreateConfig(string configPath)
    {
        var config = new MainConfig
        {
            prefixs = new PREFIXS
            {
                PREFIX = " \x04[Retakes]]\x01",
                PREFIX_CON = "[Retakes]"
            },
            DEBUG = false,
            use_db = false
        };

        File.WriteAllText(configPath,
            JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
    }

    private DBConfig LoadDBConfig()
    {
        CreateConfigsDirectory();

        var configPath = Path.Combine(ModuleDirectory, "configs/database.json");
        if (!File.Exists(configPath)) return CreateDBConfig(configPath);

        var config = JsonSerializer.Deserialize<DBConfig>(File.ReadAllText(configPath))!;

        return config;
    }

    private DBConfig CreateDBConfig(string configPath)
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

    public void LoadSpawns(string mapName)
    {
        if(spawnPoints == null!)
        {
            spawnPoints = new SpawnPoints();
        }

        spawnPoints.ClearSpawns();

        if(main_config.use_db)
        {
            //Load spawns from database
        }
        else
        {
            LoadSpawnsFromFile(mapName);
        }
    }

    private void LoadSpawnsFromFile(string mapName)
    {
        CreateConfigsDirectory();

        var configDir = Path.Combine(ModuleDirectory, $"configs/spawns");

        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
        }

        var configPath = Path.Combine(ModuleDirectory, $"configs/spawns/{mapName}.json");

        if (!File.Exists(configPath)) 
        {
            CreateSpawnsConfig(configPath).ConvertToSpawnPoints(configPath);
            return;
        }

        var config = JsonSerializer.Deserialize<SpawnsConfig>(File.ReadAllText(configPath))!;
        config.ConvertToSpawnPoints(configPath);
    }

    private SpawnsConfig CreateSpawnsConfig(string configPath)
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
                    site = (int)Site.A
                },
            }
        };

        File.WriteAllText(configPath,
            JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));

        return config;
    }
}
