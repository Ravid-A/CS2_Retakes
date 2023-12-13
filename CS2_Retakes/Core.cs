using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

using MySqlConnector;

using static Retakes.Commands;
using static Retakes.Events;
using static Retakes.Functions;

namespace Retakes;

public class Core : BasePlugin
{
    public class Config
    {
        public string PREFIX;
        public string PREFIX_CON;

        public bool use_db = false;

        public bool DEBUG;

        public Config(string PREFIX, string PREFIX_CON, bool DEBUG = false)
        {
            this.PREFIX = PREFIX;
            this.PREFIX_CON = PREFIX_CON;
            this.DEBUG = DEBUG;
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

        db.CloseConnection();
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

    private void LoadConfig()
    {
        var configPath = Path.Combine(ModuleDirectory, "config.json");
        if (!File.Exists(configPath)) CreateConfig(configPath);

        var config = JsonSerializer.Deserialize<MainConfig>(File.ReadAllText(configPath))!;

        main_config = new Config(config.prefixs.PREFIX, config.prefixs.PREFIX_CON, config.DEBUG);
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
            DEBUG = false
        };

        File.WriteAllText(configPath,
            JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
    }

    private DBConfig LoadDBConfig()
    {
        var configPath = Path.Combine(ModuleDirectory, "database.json");
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
            spawnPoints.LoadSpawnsFromDB(db, mapName);
        }
        else
        {
            //spawnPoints.LoadSpawnsFromFile(mapName);
        }
    }
}
