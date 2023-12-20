using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Menu;

using static Retakes.Core;
using static Retakes.Functions;
using Retakes;

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
            ThrowError($"Invalid spawn index: {index}");
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
            ThrowError($"Invalid spawn");
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
        Spawn spawn = spawns[new Random().Next(0, spawns.Count)];;
        
        if(spawn == null!)
        {
            return SelectSpawn(player);
        }

        if(selectedSpawns.Contains(spawn))
        {
            return SelectSpawn(player);
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

        ChatMenu menu = new ChatMenu($"{PREFIX_MENU} Spawns list:\n{PREFIX_MENU} Choose a spawn to teleport to:");

        for(int i = 0; i < spawns.Count; i++)
        {
            Spawn spawn = spawns[i];

            if(spawn == null!)
            {
                continue;
            }

            string team = spawn.team == CsTeam.CounterTerrorist ? "CT" : "T";
            string site = spawn.site == Site.A ? "A" : "B";

            menu.AddMenuOption($"{i+1} - {team} {site}", (player, option) => SpawnsList_SelectSpawn(player, option, spawn));
        }

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