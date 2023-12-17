using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

using static Retakes.Functions;

using static Weapons.WeaponsMenu;

namespace Retakes;

class CommandsHandlers
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

        Player player_obj = FindPlayer(player);

        if(player_obj == null!)
        {
            return;
        }

        if(player_obj.inGunMenu)
        {
            ReplyToCommand(commandinfo, $"You are already in the gun menu!");
            return;
        }

        player_obj.inGunMenu = true;

        OpenTPrimaryMenu(player);
    }
}
