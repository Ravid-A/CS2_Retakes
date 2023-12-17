using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

using MySqlConnector;

using Configs;

using static Retakes.CommandsHandlers;
using static Retakes.EventsHandlers;
using static Retakes.ListenersHandlers;
using static Retakes.Functions;

using Spawns;
using Weapons;

namespace Retakes;

public class Core : BasePlugin
{
    public class Config
    {
        public string PREFIX;
        public string PREFIX_CON;

        public string PREFIX_MENU;

        public bool use_db = false;

        public bool DEBUG;

        public Config(MainConfig config)
        {
            PREFIX = config.prefixs.PREFIX;
            PREFIX_CON = config.prefixs.PREFIX_CON;
            PREFIX_MENU = config.prefixs.PREFIX_MENU;
            use_db = config.use_db;
            DEBUG = config.DEBUG;
        }
    }

    public override string ModuleName => "Retakes Plugin";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Ravid";
    public override string ModuleDescription => "Retakes Plugin";

    static CCSGameRules? _gameRules = null;
    static void SetGameRules() => _gameRules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!;

    public static Database db = null!;   
    public static Config main_config = null!;

    public static List<Player> players = new List<Player>();

    public static SpawnPoints spawnPoints = null!;

    public static bool WarmupRunning
    {
        get
        {
            if (_gameRules is null)
                SetGameRules();

            return _gameRules is not null && _gameRules.WarmupPeriod;
        }
    }

    public override void Load(bool hotReload)
    {
        LoadConfig();

        Database.Connect(SQL_ConnectCallback, LoadDBConfig());

        RegisterCommands();
        RegisterEvents();
        RegisterListeners();

        if(hotReload)
        {
            OnMapStart(Server.MapName);

            Utilities.GetPlayers().ForEach(player =>
            {
                AddPlayerToList(player);
            });
        }
    }

    public override void Unload(bool hotReload)
    {
        UnRegisterCommands();
    }

    public void OnMapStart(string mapName)
    {
        PrintToServer($"Map started: {mapName}");

        LoadSpawns(mapName);
    }

    private void RegisterCommands()
    {
        AddCommand("css_guns", "Opens the guns menu", GunsCommand);
    }

    private void UnRegisterCommands()
    {
        RemoveCommand("css_guns", GunsCommand);
    }

    private void RegisterEvents()
    {
        //RegisterEventHandler<EventRoundPrestart>(OnRoundPreStart);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
    }

    private void RegisterListeners()
    {
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterListener<Listeners.OnClientConnected>(OnClientConnected);
        RegisterListener<Listeners.OnClientDisconnect>(OnClientDisconnect);
    }

    private void SQL_ConnectCallback(string connectionString, Exception exception, dynamic data)
    {
        if(connectionString == null!)
        {
            ThrowError($"Failed to connect to database: {exception.Message}");
            return;
        }

        db = new Database(connectionString);

        PrintToServer($"Connected to database");

        db.CreateTables();
    }

    public static void SQL_CheckForErrors(MySqlDataReader reader, Exception exception, dynamic data)
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
                PREFIX_CON = "[Retakes]",
                PREFIX_MENU = " \x04[Retakes]]\x01"
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
            db.Query(SQL_LoadSpawns_CB, $"SELECT * FROM `spawns` WHERE `map` = '{mapName}'");
        }
        else
        {
            LoadSpawnsFromFile(mapName);
        }
    }

    private void SQL_LoadSpawns_CB(MySqlDataReader reader, Exception exception, dynamic data)
    {
        if(exception != null!)
        {
            ThrowError($"Databse error, {exception.Message}");
            return;
        }

        if(reader.HasRows)
        {
            while(reader.Read())
            {
                int id = reader.GetInt32("id");
                string position = reader.GetString("position");
                string angles = reader.GetString("angles");
                int team = reader.GetInt32("team");
                int site = reader.GetInt32("site");

                spawnPoints.AddSpawn(new Spawn(id, position, angles, team, site));
            }
        }

        PrintToServer($"Loaded {spawnPoints.spawns.Count} spawns");
    }

    public static void SQL_FetchUser_CB(MySqlDataReader reader, Exception exception, dynamic data)
    {
        if(exception != null!)
        {
            ThrowError($"Databse error, {exception.Message}");
            return;
        }

        if(reader.HasRows)
        {
            while(reader.Read())
            {
                int t_primary = reader.GetInt32("t_primary");
                int ct_primary = reader.GetInt32("ct_primary");
                int secondary = reader.GetInt32("secondary");
                GiveAWP giveAWP = (GiveAWP)reader.GetInt32("give_awp");

                Player player = players[data];

                player.weaponsAllocator.primaryWeapon_t = t_primary;
                player.weaponsAllocator.primaryWeapon_ct = ct_primary;
                player.weaponsAllocator.secondaryWeapon = secondary;
                player.weaponsAllocator.giveAWP = giveAWP;
            }
        } else{
            Player player = players[data];

            db.Query(SQL_CheckForErrors, $"INSERT INTO `weapons` (`auth`, `name`, `t_primary`, `ct_primary`, `secondary`, `give_awp`) VALUES ('{player.GetSteamID2()}', '{player.GetName()}' , '0', '0', '0', '0')");
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
