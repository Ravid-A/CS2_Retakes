using MySqlConnector;

namespace Configs;

public class DBConfig
{
     public ConnectionConfig Connection { get; set; } = null!;

    public bool IsValid()
    {
        return Connection.Database != string.Empty && Connection.Host != string.Empty && Connection.User != string.Empty && Connection.Password != string.Empty && 0 < Connection.Port && Connection.Port < 65535;
    }

    public string BuildConnectionString()
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Database = Connection.Database,
            UserID = Connection.User,
            Password = Connection.Password,
            Server = Connection.Host,
            Port = Connection.Port,
        };

        return builder.ConnectionString;
    }
}

public class ConnectionConfig
{
    public required string Host { get; init; }
    public required string Database { get; init; }
    public required string User { get; init; }
    public required string Password { get; init; }
    public required uint Port { get; init; }
}