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
    public static void RegisterCommands()
    {
        _plugin.AddCommand("css_guns", "Opens the guns menu", GunsCommand);
        _plugin.AddCommand("css_addspawn", "Adds a spawn", AddSpawnCommand);
        _plugin.AddCommand("css_spawnlist", "Shows the spawn list", SpawnListCommand);
        _plugin.AddCommand("css_testspawn", "Test a spawn", TestSpawnCommand);
    }

    public static void UnRegisterCommands()
    {
        _plugin.RemoveCommand("css_guns", GunsCommand);
        _plugin.RemoveCommand("css_addspawn", AddSpawnCommand);
        _plugin.RemoveCommand("css_spawnlist", SpawnListCommand);
        _plugin.RemoveCommand("css_testspawn", TestSpawnCommand);
    }

    private static void GunsCommand(CCSPlayerController? player, CommandInfo commandinfo)
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

    private static void AddSpawnCommand(CCSPlayerController? player, CommandInfo info)
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

        if(info.ArgCount != 4)
        {
            ReplyToCommand(info, $"{PREFIX} Usage: {info.GetArg(0)} <team | T/CT> <site | A/B> <isBombsite | 0/1>");
            return;
        }

        string team_str = info.GetArg(1).ToLower();

        if(team_str != "t" && team_str != "ct")
        {
            ReplyToCommand(info, $"{PREFIX} Usage: {info.GetArg(0)} <team | T/CT> <site | A/B> <isBombsite | 0/1>");
            return;
        }

        string site_str = info.GetArg(2).ToLower();

        if(site_str != "a" && site_str != "b")
        {
            ReplyToCommand(info, $"{PREFIX} Usage: {info.GetArg(0)} <team | T/CT> <site | A/B> <isBombsite | 0/1>");
            return;
        }

        string isBombsite_str = info.GetArg(3).ToLower();

        if(isBombsite_str != "0" && isBombsite_str != "1")
        {
            ReplyToCommand(info, $"{PREFIX} Usage: {info.GetArg(0)} <team | T/CT> <site | A/B> <isBombsite | 0/1>");
            return;
        }

        CsTeam team = team_str == "ct" ? CsTeam.CounterTerrorist : CsTeam.Terrorist;
        Site site = site_str == "a" ? Site.A : Site.B;
        bool isBombsite = isBombsite_str == "1";

        Vector vOrigin = player!.Pawn!.Value!.AbsOrigin!;
        QAngle qAngle = player!.Pawn!.Value!.AbsRotation!;

        Spawn spawn = new Spawn(vOrigin, qAngle, team, site, isBombsite);

        spawnPoints.AddSpawn(spawn, true);

        PrintToChat(player, $"{PREFIX} Successfully added a spawn accourding to your current location.");
    }

    private static void SpawnListCommand(CCSPlayerController? player, CommandInfo info)
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

        spawnPoints.ShowSpawnsList(player);
    }

    private static void TestSpawnCommand(CCSPlayerController? player, CommandInfo info)
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

        if(info.ArgCount != 2)
        {
            ReplyToCommand(info, $"{PREFIX} Usage: {info.GetArg(0)} <spawn index>");
            return;
        }

        int spawn_index = int.Parse(info.GetArg(1));

        if(spawn_index < 0 || spawn_index > spawnPoints.spawns.Count)
        {
            ReplyToCommand(info, $"{PREFIX} Invalid spawn index");
            return;
        }

        spawnPoints.TeleportToSpawn(player, spawn_index);
    }
}
