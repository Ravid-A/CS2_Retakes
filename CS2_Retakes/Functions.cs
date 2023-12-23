using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

using static Retakes.Core;

namespace Retakes;

class Functions
{
    public static string PREFIX { get; set; } = main_config.PREFIX;
    public static string PREFIX_CON { get; set; } = main_config.PREFIX_CON;

    public static string PREFIX_MENU { get; set; } = main_config.PREFIX_MENU;

    public static void PrintToChat(CCSPlayerController controller, string msg)
    {
        controller.PrintToChat(msg);
    }

    public static void PrintToChatAll(string msg)
    {
        Server.PrintToChatAll(msg);
    }

    public static void PrintToConsole(CCSPlayerController client, string msg)
    {
        client.PrintToConsole(msg);
    }

    public static void PrintToServer(string msg, ConsoleColor color = ConsoleColor.Cyan)
    {
        Console.ForegroundColor = color;

        msg = $"{PREFIX_CON} {msg}";
        Console.WriteLine(msg);
        Console.ResetColor();
    }

    public static void ThrowError(string msg)
    {
        throw new Exception(msg);
    }

    public static void ThrowError(Exception exception)
    {
        throw exception;
    }

    public static void ReplyToCommand(CommandInfo commandInfo, string msg)
    {
        commandInfo.ReplyToCommand(msg);
    }

    public static Player FindPlayer(CCSPlayerController player)
    {
        foreach(Player player_obj in players)
        {
            if(player_obj.player == player)
            {
                return player_obj;
            }
        }

        return null!;
    }

    public static int FindPlayer(Player player)
    {
        return players.IndexOf(player);
    }

    public static int FindPlayerByPlayerIndex(int playerIndex)
    {
        foreach(Player player in players)
        {
            if(player.player.Index == playerIndex)
            {
                return FindPlayer(player);
            }
        }

        return -1;
    }

    public static void ServerCommand(string command, params object[] args)
    {
        Server.ExecuteCommand(string.Format(command, args));
    }

    public static void StartPausedWarmup() 
    {
        ServerCommand("mp_warmup_start");
        ServerCommand("mp_warmuptime 120");  // this value must be greater than 6 or the warmup countdown will always start
        ServerCommand("mp_warmup_pausetimer 1");
    }

    public static void StartTimedWarmup(int time) 
    {
        ServerCommand("mp_do_warmup_period 1");
        ServerCommand("mp_warmup_pausetimer 0");
        ServerCommand($"mp_warmuptime {time}");
        ServerCommand("mp_warmup_start");
        ServerCommand("mp_warmup_start"); // don't ask.
    }

    public static float GetVectorDistance(Vector vec1, Vector vec2, bool squared=false)
    {
        float distance = (float)Math.Sqrt(Math.Pow(vec1.X - vec2.X, 2) + Math.Pow(vec1.Y - vec2.Y, 2) + Math.Pow(vec1.Z - vec2.Z, 2));
        return (float)Math.Pow(distance, squared ? 2 : 1);
    }

    public static int GetTeamClientCount(CsTeam team, bool alive = false)
    {
        int count = 0;

        ;

        foreach (CCSPlayerController player in Utilities.GetPlayers())
        {
            if (player.TeamNum == (byte)team)
            {
                if (alive)
                {
                    if (player.PawnIsAlive)
                    {
                        count++;
                    }
                }
                else
                {
                    count++;
                }
            }
        }

        return count;
    }

    public static CCSPlayerController GetPlanter()
    {
        foreach (Player player in players)
        {
            if (player.isBomberOwner)
            {
                return player.player;
            }
        }

        return null!;
    }

    public static void StringToFloatVector(string str, out float[] vec)
    {
        if(str == string.Empty)
            ThrowError("Invalid vector string");

        string[] str_array = str.Split(" ");

        if(str_array.Length != 3)
            ThrowError("Invalid vector string");

        vec = new float[3];

        for (int i = 0; i < str_array.Length; i++)
        {
            if(!float.TryParse(str_array[i], out vec[i]))
                ThrowError("Invalid vector string");
        }
    }

    public static bool IsVectorStringValid(string str)
    {
        if(str == string.Empty)
            return false;

        string[] str_array = str.Split(" ");

        if(str_array.Length != 3)
            return false;

        for (int i = 0; i < str_array.Length; i++)
        {
            if(!float.TryParse(str_array[i], out float _))
                return false;
        }

        return true;
    }
}