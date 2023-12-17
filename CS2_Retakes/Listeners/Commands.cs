using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Admin;

using Spawns;

using static Retakes.Core;
using static Retakes.Functions;


using static Weapons.WeaponsMenu;

namespace Retakes;

class CommandsHandlers
{
    public static void GunsCommand(CCSPlayerController? player, CommandInfo commandinfo)
    {
        if (player == null)
        {
            ReplyToCommand(commandinfo, $"{PREFIX} This command can only be executed by a player");
            return;
        }

        if(!player.IsValid)
        {
            ReplyToCommand(commandinfo, $"{PREFIX} This command can only be executed by a valid player");
            return;
        }

        Player player_obj = FindPlayer(player);

        if(player_obj == null!)
        {
            return;
        }

        if(player_obj.inGunMenu)
        {
            ReplyToCommand(commandinfo, $"{PREFIX} You are already in the gun menu!");
            return;
        }

        player_obj.inGunMenu = true;

        OpenTPrimaryMenu(player);
    }

    public static void AddSpawnCommand(CCSPlayerController? player, CommandInfo info)
    {
        if(player == null)
        {
            ReplyToCommand(info, $"{PREFIX} This command can only be executed by a player");
            return;
        }

        if(!player.IsValid)
        {
            ReplyToCommand(info, $"{PREFIX} This command can only be executed by a valid player");
            return;
        }

        if(!AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            ReplyToCommand(info, $"{PREFIX} You do not have permission to execute this command");
            return;
        }

        if(info.ArgCount != 3)
        {
            ReplyToCommand(info, $"{PREFIX} Usage: {info.GetArg(0)} <team | T/CT> <site | A/B>");
            return;
        }

        string team_str = info.GetArg(1).ToLower();

        if(team_str != "t" && team_str != "ct")
        {
            ReplyToCommand(info, $"{PREFIX} Usage: {info.GetArg(0)} <team | T/CT> <site | A/B>");
            return;
        }

        string site_str = info.GetArg(2).ToLower();

        if(site_str != "a" && site_str != "b")
        {
            ReplyToCommand(info, $"{PREFIX} Usage: {info.GetArg(0)} <team | T/CT> <site | A/B>");
            return;
        }

        CsTeam team = team_str == "ct" ? CsTeam.CounterTerrorist : CsTeam.Terrorist;
        Site site = site_str == "a" ? Site.A : Site.B;

        Vector vOrigin = player!.Pawn!.Value!.AbsOrigin!;
        QAngle qAngle = player!.Pawn!.Value!.AbsRotation!;

        Spawn spawn = new Spawn(vOrigin, qAngle, team, site);

        spawnPoints.AddSpawn(spawn, true);

        PrintToChat(player, $"{PREFIX} Successfully added a spawn accourding to your current location.");
    }
}
