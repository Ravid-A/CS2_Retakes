using System.Linq;

using MySqlConnector;

using Configs;

using static Retakes.Core;
using static Retakes.Functions;
using Spawns;
using CounterStrikeSharp.API;

namespace Retakes;

public class Database
{
    private static string _connectionString = string.Empty;

    public delegate void ConnectCallback(string connectionString, Exception exception, dynamic data);
    public delegate void QueryCallback(MySqlDataReader reader, Exception exception, dynamic data);

    public Database(string connectionString)
    {
        _connectionString = connectionString;
    }

    public static void Connect(ConnectCallback callback, DBConfig config, dynamic data = null!)
    {
        if(config == null!)
        {
            ThrowError("DBConfig cannot be null.");
            return;
        }

        if(!config.IsValid())
        {
            ThrowError("DBConfig is invalid.");
            return;
        }

        _connectionString = config.BuildConnectionString();

        try
        {
            MySqlConnection connection = new MySqlConnection(_connectionString);

            connection.Open();

            callback(_connectionString, null!, data);

            connection.Close();
        }
        catch (Exception e)
        {
            callback(null!, e, data);
        }
    }

    public void CreateTables()
    {
        string query = string.Empty;
        if(main_config.use_db)
        {
            query += "CREATE TABLE IF NOT EXISTS `spawns` ( `id` INT NOT NULL AUTO_INCREMENT , `map` VARCHAR(128) NOT NULL , `position` VARCHAR(64) NOT NULL , `angles` VARCHAR(64) NOT NULL , `team` INT NOT NULL , `site` INT NOT NULL, `is_bombsite` INT NOT NULL, PRIMARY KEY (`id`)) ENGINE = InnoDB;";
            return;
        }

        query += "CREATE TABLE IF NOT EXISTS `weapons` ( `id` INT NOT NULL AUTO_INCREMENT , `auth` VARCHAR(128) NOT NULL , `name` VARCHAR(128) NOT NULL , `t_primary` INT NOT NULL , `ct_primary` INT NOT NULL , `secondary` INT NOT NULL, `give_awp` INT NOT NULL , PRIMARY KEY (`id`), UNIQUE (`auth`)) ENGINE = InnoDB;";

        Query(SQL_CheckForErrors, query);
    }

    //Format function for queries, escapes the string and replaces the placeholders with the values
    public string EscapeString(string buffer)
    {
        return MySqlHelper.EscapeString(buffer);
    }

    public void Query(QueryCallback callback, string query, dynamic data = null!)
    {
        try 
        {
            if (string.IsNullOrEmpty(query))
            {
                ThrowError("Query cannot be null or empty.");
            }

            using(MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using(MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using(MySqlDataReader reader = command.ExecuteReader())
                    {
                        callback(reader, null!, data);
                    }
                }

                connection.Close();
            }
        }
        catch (Exception e)
        {
            callback(null!, e, data);
        }
    }


    public void InsertSpawn(Spawn spawn, int index)
    {
        if(!main_config.use_db)
        {
            return;
        }

        if(index < 0 || index > spawnPoints.spawns.Count)
        {
            ThrowError($"Invalid spawn index: {index}");
            return;
        }

        string mapName = Server.MapName;

        string query = $"INSERT INTO `spawns` (`map`, `position`, `angles`, `team`, `site`, `is_bombsite`) VALUES ('{mapName}', '{spawn.position}', '{spawn.angles}', {(int)spawn.team}, {(int)spawn.site}, {spawn.isBombsite});";
        query += "SELECT LAST_INSERT_ID() as id;";

        Query(SQL_InserRow_CB, query, index);
    }

    private void SQL_InserRow_CB(MySqlDataReader reader, Exception exception, dynamic data)
    {
        if(exception != null!)
        {
            ThrowError($"Databse error, {exception.Message}");
            return;
        }

        int index = data;

        if(index < 0 || index > spawnPoints.spawns.Count)
        {
            ThrowError($"Invalid spawn index: {index}");
            return;
        }

        if(reader.HasRows)
        {
            while(reader.Read())
            {
                int id = reader.GetInt32("id");

                spawnPoints.SetSpawnId(index, id);
            }
        }
    }

    public void DeleteSpawn(Spawn spawn)
    {
        if(!main_config.use_db)
        {
            return;
        }

        string mapName = Server.MapName;

        string query = $"DELETE FROM `spawns` WHERE `id` = '{spawn.id}' AND `map` = '{mapName}'";

        Query(SQL_CheckForErrors, query);
    }

    public static void SQL_CheckForErrors(MySqlDataReader reader, Exception exception, dynamic data)
    {
        if(exception != null!)
        {
            ThrowError($"Databse error, {exception.Message}");
            return;
        }
    } 
}