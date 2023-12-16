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
        if(!main_config.use_db)
        {
            return;
        }

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

    //Format function for queries, escapes the string and replaces the placeholders with the values
    public string EscapeString(string buffer)
    {
        return MySqlHelper.EscapeString(buffer);
    }

    public void Query(QueryCallback callback, string query, dynamic data = null!)
    {
        if(!main_config.use_db)
        {
            return;
        }

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

        string query = $"INSERT INTO `spawns` (`map`, `position`, `angles`, `team`, `site`) VALUES ('{mapName}', '{spawn.position}', '{spawn.angles}', '{(int)spawn.team}', '{(int)spawn.site}');";
        query += "SELECT LAST_INSERT_ID() as id;";

        Query(SQL_InserRow_CB, query);
    }

    private void SQL_InserRow_CB(MySqlDataReader reader, Exception exception, dynamic data)
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

                spawnPoints.spawns[data].id = id;
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
}