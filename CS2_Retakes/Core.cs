using CounterStrikeSharp.API.Core;

using MySqlConnector;

using static Retakes.Commands;
using static Retakes.Events;
using static Retakes.Functions;

namespace Retakes;

public class Core : BasePlugin
{
    public const string PREFIX = "[Retakes]";

    public override string ModuleName => "Retakes Plugin";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Ravid";
    public override string ModuleDescription => "Retakes Plugin";

    public Database db = new Database();

    public override void Load(bool hotReload)
    {
        Database.Connect(ConnectCallback, "Retakes");

        RegisterCommands();
        RegisterEvents();
    }

    public override void Unload(bool hotReload)
    {
        // Unregister the command
        RemoveCommand("css_test", TestCommand);
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

    private void ConnectCallback(MySqlConnection sqlConnection, Exception exception)
    {
        if(sqlConnection == null!)
        {
            ThrowError($"{PREFIX} Failed to connect to database: {exception.Message}");
            return;
        }

        PrintToServer($"{PREFIX} Connected to database");

        db.Query(CB_CreateTable, "CREATE TABLE IF NOT EXISTS `retakes` (`id` INT NOT NULL AUTO_INCREMENT, `steamid` VARCHAR(17) NOT NULL, `name` VARCHAR(32) NOT NULL, `kills` INT NOT NULL, `deaths` INT NOT NULL, `mvps` INT NOT NULL, `wins` INT NOT NULL, `losses` INT NOT NULL, `draws` INT NOT NULL, `last_seen` DATETIME NOT NULL, PRIMARY KEY (`id`)) ENGINE = InnoDB;");
    }

    private void CB_CreateTable(MySqlConnection sqlConnection, MySqlDataReader reader, Exception exception)
    {
        if(exception != null!)
        {
            ThrowError($"{PREFIX} Failed to create table: {exception.Message}");
            return;
        }

        PrintToServer($"{PREFIX} Created table");
    } 
}
