using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Admin;

using Spawns;

using static Retakes.Core;
using static Retakes.Functions;

using static Weapons.WeaponsMenu;

using static Configs.Configs;
using static Configs.SpawnsConfig;

namespace Retakes;

class CommandsHandlers
{
    public static void RegisterCommands()
    {
        _plugin.AddCommand("css_guns", "Opens the guns menu", GunsCommand);
        _plugin.AddCommand("css_addspawn", "Adds a spawn", AddSpawnCommand);
        _plugin.AddCommand("css_removespawn", "Removes a spawn", RemoveSpawnCommand);
        _plugin.AddCommand("css_spawnlist", "Shows the spawn list", SpawnListCommand);
        _plugin.AddCommand("css_reloadspawns", "Reloads the spawns", ReloadSpawnsCommand);
        _plugin.AddCommand("css_savespawnstofile", "Saves the spawns to file", SaveSpawnsToFileCommand);
    }

    public static void UnRegisterCommands()
    {
        _plugin.RemoveCommand("css_guns", GunsCommand);
        _plugin.RemoveCommand("css_addspawn", AddSpawnCommand);
        _plugin.RemoveCommand("css_removespawn", RemoveSpawnCommand);
        _plugin.RemoveCommand("css_spawnlist", SpawnListCommand);
        _plugin.RemoveCommand("css_reloadspawns", ReloadSpawnsCommand);
        _plugin.RemoveCommand("css_savespawnstofile", SaveSpawnsToFileCommand);
    }

    private static void GunsCommand(CCSPlayerController? player, CommandInfo commandinfo)
    {
        if (player == null)
        {
            ReplyToCommand(commandinfo, $"{PREFIX} This command can only be executed by a player.");
            return;
        }

        if(!player.IsValid)
        {
            ReplyToCommand(commandinfo, $"{PREFIX} This command can only be executed by a valid player.");
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
            ReplyToCommand(info, $"{PREFIX} This command can only be executed by a player.");
            return;
        }

        if(!player.IsValid)
        {
            ReplyToCommand(info, $"{PREFIX} This command can only be executed by a valid player.");
            return;
        }

        if(!AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            ReplyToCommand(info, $"{PREFIX} You do not have permission to execute this command.");
            return;
        }

        if(info.ArgCount != 4)
        {
            ReplyToCommand(info, $"{PREFIX} Usage: {info.GetArg(0)} <team | T/CT> <site | A/B> <isBombsite | 0/1>.");
            return;
        }

        string team_str = info.GetArg(1).ToLower();

        if(team_str != "t" && team_str != "ct")
        {
            ReplyToCommand(info, $"{PREFIX} Usage: {info.GetArg(0)} <team | T/CT> <site | A/B> <isBombsite | 0/1>.");
            return;
        }

        string site_str = info.GetArg(2).ToLower();

        if(site_str != "a" && site_str != "b")
        {
            ReplyToCommand(info, $"{PREFIX} Usage: {info.GetArg(0)} <team | T/CT> <site | A/B> <isBombsite | 0/1>.");
            return;
        }

        string isBombsite_str = info.GetArg(3).ToLower();

        if(isBombsite_str != "0" && isBombsite_str != "1")
        {
            ReplyToCommand(info, $"{PREFIX} Usage: {info.GetArg(0)} <team | T/CT> <site | A/B> <isBombsite | 0/1>.");
            return;
        }

        CsTeam team = team_str == "ct" ? CsTeam.CounterTerrorist : CsTeam.Terrorist;
        Site site = site_str == "a" ? Site.A : Site.B;
        bool isBombsite = isBombsite_str == "1";

        if(team == CsTeam.CounterTerrorist)
        {
            isBombsite = false;
        }

        Vector absPos = player!.PlayerPawn!.Value!.AbsOrigin!;
        QAngle absRot = player!.PlayerPawn!.Value!.AbsRotation!;

        Vector vOrigin = new Vector(absPos.X, absPos.Y, absPos.Z);
        QAngle qAngle = new QAngle(absRot.X, absRot.Y, absRot.Z);

        Spawn spawn = new Spawn(vOrigin, qAngle, team, site, isBombsite);

        spawnPoints.AddSpawn(spawn, true);

        PrintToChat(player, $"{PREFIX} Successfully added a spawn accourding to your current location.");
    }

    private static void RemoveSpawnCommand(CCSPlayerController? player, CommandInfo info)
    {
        if(player == null)
        {
            ReplyToCommand(info, $"{PREFIX} This command can only be executed by a player.");
            return;
        }

        if(!player.IsValid)
        {
            ReplyToCommand(info, $"{PREFIX} This command can only be executed by a valid player.");
            return;
        }

        if(!AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            ReplyToCommand(info, $"{PREFIX} You do not have permission to execute this command.");
            return;
        }

         if(info.ArgCount != 2)
        {
            ReplyToCommand(info, $"{PREFIX} Usage: {info.GetArg(0)} <id>.");
            return;
        }

        string id_str = info.GetArg(1);

        if(!int.TryParse(id_str, out int id))
        {
            ReplyToCommand(info, $"{PREFIX} Usage: {info.GetArg(0)} <id>.");
            return;
        }

        if(id < 0 || id >= spawnPoints.spawns.Count)
        {
            ReplyToCommand(info, $"{PREFIX} Invalid spawn id, spawns id is shown in the spawn list.");
            return;
        }

        spawnPoints.RemoveSpawn(id);
        ReplyToCommand(info, $"{PREFIX} Successfully removed spawn with id: {id}.");
    }

    private static void SpawnListCommand(CCSPlayerController? player, CommandInfo info)
    {
        if(player == null)
        {
            ReplyToCommand(info, $"{PREFIX} This command can only be executed by a player.");
            return;
        }

        if(!player.IsValid)
        {
            ReplyToCommand(info, $"{PREFIX} This command can only be executed by a valid player.");
            return;
        }

        if(!AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            ReplyToCommand(info, $"{PREFIX} You do not have permission to execute this command.");
            return;
        }

        spawnPoints.ShowSpawnsList(player);
    }

    private static void ReloadSpawnsCommand(CCSPlayerController? player, CommandInfo info)
    {
        if(player == null)
        {
            ReplyToCommand(info, $"{PREFIX} This command can only be executed by a player.");
            return;
        }

        if(!player.IsValid)
        {
            ReplyToCommand(info, $"{PREFIX} This command can only be executed by a valid player.");
            return;
        }

        if(!AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            ReplyToCommand(info, $"{PREFIX} You do not have permission to execute this command.");
            return;
        }

        LoadSpawns(Server.MapName);
        ReplyToCommand(info, $"{PREFIX} Successfully reloaded the spawns.");
    }

    private static void SaveSpawnsToFileCommand(CCSPlayerController? player, CommandInfo info)
    {
        if(player == null)
        {
            ReplyToCommand(info, $"{PREFIX} This command can only be executed by a player.");
            return;
        }

        if(!player.IsValid)
        {
            ReplyToCommand(info, $"{PREFIX} This command can only be executed by a valid player.");
            return;
        }

        if(!AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            ReplyToCommand(info, $"{PREFIX} You do not have permission to execute this command.");
            return;
        }

        ConvertFromSpawnPoints();
        ReplyToCommand(info, $"{PREFIX} Successfully saved the spawns to file.");
    }
}
