using System.Linq;

using MySqlConnector;

using static Retakes.Core;
using static Retakes.Functions;

namespace Retakes;

public class Database
{
    private static MySqlConnection _connection = null!;

    public delegate void ConnectCallback(MySqlConnection sqlConnection, Exception exception, dynamic data);
    public delegate void QueryCallback(MySqlConnection sqlConnection, MySqlDataReader reader, Exception exception, dynamic data);

    public Database(MySqlConnection connection)
    {
        _connection = connection;
    }

    public static void Connect(ConnectCallback callback, DBConfig config, dynamic data = null!)
    {
        main_config.use_db = config.IsValid();

        if(!main_config.use_db)
        {
            ThrowError("Invalid database config");
            return;
        }

        string connection_string = config.BuildConnectionString();
        
            
        if(main_config.DEBUG)
        {
            PrintToServer($"[SQL] Connection string: {connection_string}");
        }

        try
        {
            MySqlConnection connection = new MySqlConnection(connection_string);

            connection.Open();

            _connection = connection;

            _connection.StateChange += (sender, args) =>
            {
                if (args.CurrentState == System.Data.ConnectionState.Closed)
                {
                    _connection = null!;
                    if(main_config.DEBUG)
                    {
                        PrintToServer("[SQL] Connection closed");
                    }
                    main_config.use_db = false;
                }
            };

            callback(connection, null!, data);
        }
        catch (Exception e)
        {
            callback(null!, e, data);
        }
    }

    public void CloseConnection()
    {
        if(_connection != null!)
        {
            _connection.Close();
        }
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
            
            if(main_config.DEBUG)
            {
                PrintToServer($"[SQL] Query: {query}");
            }

            using (MySqlCommand command = new MySqlCommand(query, _connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    callback(_connection, reader, null!, data);
                }
            }
        }
        catch (Exception e)
        {
            callback(_connection, null!, e, data);
        }
    }

}