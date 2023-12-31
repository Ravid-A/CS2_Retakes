using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Menu;

using static Retakes.Core;
using static Retakes.Functions;
using Retakes;

using static Configs.Configs;
using static Configs.SpawnsConfig;

namespace Spawns;

public class SpawnPoints
{
    public List<Spawn> spawns;

    public string spawnsPath = string.Empty;

    public SpawnPoints()
    {
        spawns = new List<Spawn>();
    }

    public int Length => spawns.Count;

    public void BuildPath()
    {
        CreateConfigsDirectory();

        var configDir = Path.Combine(_plugin.ModuleDirectory, $"configs/spawns");

        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
        }

        spawnsPath = Path.Combine(_plugin.ModuleDirectory, $"configs/spawns/{Server.MapName}.json");
    }

    public void SetSpawnId(int index, int id)
    {
        if(index < 0 || index > spawns.Count)
        {
            ThrowError($"Invalid spawn index: {index}");
            return;
        }

        spawns[index].id = id;
    }

    public void AddSpawn(Spawn spawn, bool insert = false)
    {
        if(main_config.DEBUG)
        {
            PrintToServer($"Adding spawn: {spawn.position} {spawn.angles} {spawn.team} {spawn.site} {spawn.isBombsite}");
        }

        spawns.Add(spawn);

        if(insert)
        {
            int index = spawns.IndexOf(spawn);

            if(main_config.use_db)
            {
                db.InsertSpawn(spawn, index);
                return;
            }

            ConvertFromSpawnPoints();
        }
    }

    public void RemoveSpawn(Spawn spawn)
    {
        if(main_config.DEBUG)
        {
            PrintToServer($"Removing spawn: {spawn.position} {spawn.angles} {spawn.team} {spawn.site}");
        }

        spawns.Remove(spawn);

        if(main_config.use_db)
        {
            db.DeleteSpawn(spawn);
            return;
        }

        ConvertFromSpawnPoints();
    }

    public void RemoveSpawn(int index)
    {
        if(index < 0 || index > spawns.Count)
        {
            ThrowError($"Invalid spawn index: {index}");
            return;
        }

        if(main_config.DEBUG)
        {
            PrintToServer($"Removing spawn: {spawns[index].position} {spawns[index].angles} {spawns[index].team} {spawns[index].site} {spawns[index].isBombsite}");
        }

        Spawn spawn = spawns[index];

        spawns.RemoveAt(index);

        if(main_config.use_db)
        {
            db.DeleteSpawn(spawn);
            return;
        }

        ConvertFromSpawnPoints();
    }

    public void ClearSpawns()
    {
        if(main_config.DEBUG)
        {
            PrintToServer($"Clearing spawns");
        }

        spawns.Clear();
    }

    public void TeleportToSpawn(CCSPlayerController player, int index)
    {
        if(index < 0 || index > spawns.Count)
        {
            ThrowError($"Invalid spawn index: {index}");
            return;
        }

        Spawn spawn = spawns[index];

        if(spawn == null!)
        {
            return;
        }

        if(main_config.DEBUG)
        {
            PrintToServer($"Teleporting player to spawn {index}: {spawns[index].position} {spawns[index].angles} {spawns[index].team} {spawns[index].site}");
        }

        spawns[index].Teleport(player);
    }

    public void TeleportToSpawn(CCSPlayerController player, Spawn spawn)
    {
        if(spawn == null!)
        {
            return;
        }

        if(main_config.DEBUG)
        {
            PrintToServer($"Teleporting player to spawn: {spawn.position} {spawn.angles} {spawn.team} {spawn.site}");
        }

        spawn.Teleport(player);
    }

    public Spawn SelectSpawn(Player player)
    {
        if(spawns.Count == 0)
        {
            return null!;
        }

        player.selectSpawnCallCount++;

        if(player.selectSpawnCallCount >= 100)
        {
            return null!;
        }

        Spawn spawn = spawns[new Random().Next(0, spawns.Count)];;
        
        if(spawn == null!)
        {
            return SelectSpawn(player);
        }

        if(selectedSpawns.Contains(spawn))
        {
            return SelectSpawn(player);
        }

        if(main_config.DEBUG)
        {
            PrintToServer($"Spawn: {spawn.team} | Player: {player.GetTeam()} | Site: {spawn.site} | BombSite: {spawn.isBombsite} | Player: {player.isBomberOwner}");
        }

        if(spawn.team != player.GetTeam() || spawn.site != currentSite || spawn.isBombsite != player.isBomberOwner)
        {
            return SelectSpawn(player);
        }

        selectedSpawns.Add(spawn);
        return spawn;
    }

    public void ShowSpawnsList(CCSPlayerController player)
    {
        if(spawns.Count == 0)
        {
            PrintToChat(player, $"{PREFIX} No spawns have been set");
            return;
        }

        ChatMenu menu = new ChatMenu($"{PREFIX_MENU} Spawns list - Choose a spawn to teleport to:");

        for(int i = 0; i < spawns.Count; i++)
        {
            Spawn spawn = spawns[i];

            if(spawn == null!)
            {
                continue;
            }

            string team = spawn.team == CsTeam.CounterTerrorist ? "CT" : "T";
            string site = spawn.site == Site.A ? "A" : "B";

            menu.AddMenuOption($"{team} {site} (ID: {i})", (player, option) => SpawnsList_SelectSpawn(player, option, spawn));
        }

        menu.AddMenuOption("Close", (player, option) => {});

        ChatMenus.OpenMenu(player, menu);
    }

    private void SpawnsList_SelectSpawn(CCSPlayerController player, ChatMenuOption option, Spawn spawn)
    {
        if (option == null)
        {
            PrintToChat(player, $"{PREFIX} You did not select a weapon!");
            return;
        }

        Player player_obj = FindPlayer(player);

        if (player_obj == null!)
        {
            return;
        }

        TeleportToSpawn(player, spawn);
    }
}