using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

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

    public static void ServerCommand(string command, params object[] args)
    {
        Server.ExecuteCommand(string.Format(command, args));
    }

    public static void StartTimedWarmup(int time) 
    {
        ServerCommand("mp_do_warmup_period 1");
        ServerCommand("mp_warmup_pausetimer 0");
        ServerCommand($"mp_warmuptime {time}");
        ServerCommand("mp_warmup_start");
        ServerCommand("mp_warmup_start"); // don't ask.
    }
}