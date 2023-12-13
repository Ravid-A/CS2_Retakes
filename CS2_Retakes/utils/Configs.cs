using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using MySqlConnector;

using static Retakes.Core;

namespace Retakes;

class Configs
{ 
    public enum Site
    {
        A,
        B
    }

    public class Spawn
    {
        public float[] position = new float[3];
        public float[] angles = new float[3];
        private int team = 0;
        public int Team
        {
            get { return team; }
            set { team = value; }
        }
        public Site site
        {
            get { return site; }
            set { site = value; }
        }
    }

    public struct DBConfig
    {
        public string host;
        public string database;
        public string user;
        public string password;

        public DBConfig(string host, string database, string user, string password)
        {
            this.host = host;
            this.database = database;
            this.user = user;
            this.password = password;
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

    public List<Spawn> spawns = new List<Spawn>();

    public static void Load()
    {
        LoadSpawns();
    }

    private static void LoadSpawns()
    {
    }

    public static DBConfig LoadDBConfig(string entry)
    {
        DBConfig config = new DBConfig("192.168.1.230", "retakes", "konlig", "xBA3XhxQbx52");
        return config;
    }
}