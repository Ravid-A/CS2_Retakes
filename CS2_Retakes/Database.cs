using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

using static Retakes.Configs;
using static Retakes.Functions;

namespace Retakes;

public class Database
{
    private static MySqlConnection _connection = null!;

    public delegate void ConnectCallback(MySqlConnection sqlConnection, Exception exception);
    public delegate void QueryCallback(MySqlConnection sqlConnection, MySqlDataReader reader, Exception exception);

    public static void Connect(ConnectCallback callback, string entry)
    {
        string connection_string = LoadDBConfig(entry).BuildConnectionString();
        
        MySqlConnection connection = new MySqlConnection(connection_string);
        try
        {
            connection.Open();

            _connection = connection;

            _connection.StateChange += (sender, args) =>
            {
                if (args.CurrentState == System.Data.ConnectionState.Closed)
                {
                    _connection = null!;
                    ThrowError("Connection to database closed");
                }
            };

            callback(connection, null!);
        }
        catch (Exception e)
        {
            callback(null!, e);
        }
    }

    //Format function for queries, escapes the string and replaces the placeholders with the values
    public void Format(string buffer, string query, params object[] args)
    {
        buffer = string.Format(query, args);
        buffer = MySqlHelper.EscapeString(buffer);
    }

    public void Query(QueryCallback callback, string query, params object[] args)
    {
        query = string.Format(query, args);

        try 
        {
            using (MySqlCommand command = new MySqlCommand(query, _connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    callback(_connection, reader, null!);
                }
            }
        }
        catch (Exception e)
        {
            callback(_connection, null!, e);
        }
    }
}