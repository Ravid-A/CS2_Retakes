using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

using static Retakes.Functions;

namespace Retakes;

class Commands
{
    public static void TestCommand(CCSPlayerController? player, CommandInfo commandinfo)
    {
        if (player == null)
        {
            ReplyToCommand(commandinfo, $"This command can only be executed by a player");
            return;
        }

        if(!player.IsValid)
        {
            ReplyToCommand(commandinfo, $"This command can only be executed by a valid player");
            return;
        }

        PrintToChat(player, $"{PREFIX} Test command");
    }
}
