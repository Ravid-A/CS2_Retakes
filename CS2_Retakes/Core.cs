using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

using MySqlConnector;

using Configs;
using static Configs.Configs;

using static Retakes.CommandsHandlers;
using static Retakes.EventsHandlers;
using static Retakes.ListenersHandlers;
using static Retakes.Functions;
using static Retakes.Database;

using Spawns;
using Weapons;

namespace Retakes;

public enum Site
{
    A,
    B
}

public class Core : BasePlugin
{
    public class Config
    {
        public string PREFIX;
        public string PREFIX_CON;

        public string PREFIX_MENU;

        public bool use_db = false;

        public int WARMUP_TIME = 12;
        public int MAX_PLAYERS = 9;

        public bool DEBUG;

        public Config(MainConfig config)
        {
            PREFIX = config.prefixs.PREFIX;
            PREFIX_CON = config.prefixs.PREFIX_CON;
            PREFIX_MENU = config.prefixs.PREFIX_MENU;
            use_db = config.use_db;
            DEBUG = config.DEBUG;
            WARMUP_TIME = config.WARMUP_TIME;
            MAX_PLAYERS = config.MAX_PLAYERS;
        }
    }

    public static Core _plugin = null!;

    public override string ModuleName => "Retakes Plugin";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Ravid";
    public override string ModuleDescription => "Retakes Plugin";

    static CCSGameRules? _gameRules = null;
    static void SetGameRules()
    {
        var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");

        if (gameRulesEntities.Any())
        {
            _gameRules = gameRulesEntities.First().GameRules!;
        }
    }

    public static Database db = null!;   
    public static Config main_config = null!;
    public static List<Player> players = new List<Player>();
    public static SpawnPoints spawnPoints = null!;
    public static List<Spawn> selectedSpawns = new List<Spawn>();
    public static Site currentSite = Site.A;

    private static bool WarmupRunning
    {
        get
        {
            if (_gameRules is null)
                SetGameRules();

            return _gameRules is not null && _gameRules.WarmupPeriod;
        }
    }

    public static bool isBombPlanted = false;
    public static bool isBombPlantSignal = false;
    public static int bombOwner = -1;

    public override void Load(bool hotReload)
    {
        _plugin = this;

        LoadConfig();

        Connect(SQL_ConnectCallback, LoadDBConfig());

        RegisterCommands();
        RegisterEvents();
        RegisterListeners();

        if(hotReload)
        {
            OnMapStart(Server.MapName);

            Utilities.GetPlayers().ForEach(AddPlayerToList);
        }
    }

    public override void Unload(bool hotReload)
    {
        UnRegisterCommands();
    }

    public static bool isLive()
    {
        return !WarmupRunning;
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

    public static void SQL_LoadSpawns_CB(MySqlDataReader reader, Exception exception, dynamic data)
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
                bool isBombsite = reader.GetBoolean("is_bombsite");

                spawnPoints.AddSpawn(new Spawn(id, position, angles, team, site, isBombsite));
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
}
