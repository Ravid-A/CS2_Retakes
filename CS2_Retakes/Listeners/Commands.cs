using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

using static Retakes.Functions;

namespace Retakes;

class Commands
{
    public static void GunsCommand(CCSPlayerController? player, CommandInfo commandinfo)
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

        
    }
}
