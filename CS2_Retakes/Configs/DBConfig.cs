using System.Text.Json;
using MySqlConnector;

namespace Retakes;

public class DBConfig
{
    public string host;
    public string database;
    public string user;
    public string password;

    public DBConfig(string entry)
    {
        this.host = "127.0.0.1";
        this.database = "retakes";
        this.user = "konlig";
        this.password = "xBA3XhxQbx52";

        //TODO load from config by entry
    }

    public string BuildConnectionString()
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Database = this.database,
            UserID = this.user,
            Password = this.password,
            Server = this.host,
            Port = 3306,
        };

        return builder.ConnectionString;
    }
}