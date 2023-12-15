using System.Linq;

using MySqlConnector;

using Configs;

using static Retakes.Core;
using static Retakes.Functions;

namespace Retakes;

public class Database
{
    private static MySqlConnection _connection = null!;

    private static bool _isConnected = false;

    public delegate void ConnectCallback(MySqlConnection sqlConnection, Exception exception, dynamic data);
    public delegate void QueryCallback(MySqlConnection sqlConnection, MySqlDataReader reader, Exception exception, dynamic data);

    public Database(MySqlConnection connection)
    {
        _connection = connection;
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
            ThrowError("DBConfig is not valid.");
            return;
        }

        string connection_string = config.BuildConnectionString();

        try
        {
            MySqlConnection connection = new MySqlConnection(connection_string);

            connection.Open();

            _connection = connection;
            _isConnected = true;

            _connection.StateChange += (sender, args) =>
            {
                if (args.CurrentState == System.Data.ConnectionState.Closed)
                {
                    _connection = null!;
                    _isConnected = false;
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
        if(_connection != null! && _isConnected)
        {
            _connection.Close();
        }

        _isConnected = false;
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