using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

using static Retakes.Core;
using static Retakes.Functions;

namespace Retakes;

class Commands
{
    public static void TestCommand(CCSPlayerController? player, CommandInfo commandinfo)
    {
        if (player == null)
        {
            PrintToServer($"{PREFIX} This command can only be executed by a player");
            return;
        }

        if(!player.IsValid)
        {
            PrintToServer($"{PREFIX} This command can only be executed by a valid player");
            return;
        }

        PrintToChat(player, $"{PREFIX} Test command");
    }
}
