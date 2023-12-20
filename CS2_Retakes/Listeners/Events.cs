using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

using static Spawns.SpawnPoints;

using static Retakes.Core;
using static Retakes.Functions;
using static Retakes.Player;
using CounterStrikeSharp.API;

namespace Retakes;

class EventsHandlers
{
    public static void RegisterEvents()
    {
        _plugin.RegisterEventHandler<EventRoundPrestart>(OnRoundPreStart);
        _plugin.RegisterEventHandler<EventRoundPoststart>(OnRoundPostStart);
        _plugin.RegisterEventHandler<EventRoundFreezeEnd>(OnRoundFreezeEnd);

        _plugin.RegisterEventHandler<EventPlayerTeam>(OnPlayerTeam);
        _plugin.RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
    }

    private static HookResult OnRoundPreStart(EventRoundPrestart @event, GameEventInfo info)
    {
        if(!isLive())
        {
            return HookResult.Continue;
        }

        currentSite = (Site)new Random().Next(0, 2);

        selectedSpawns.Clear();

        List<int> ts_players = new List<int>();
        List<int> ct_players = new List<int>();

        for(int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
             if(player == null! || !player.IsValid())
            {
                continue;
            }

            if(player.GetTeam() == CsTeam.Terrorist)
            {
                ts_players.Add(i);
            }

            if(player.GetTeam() == CsTeam.CounterTerrorist)
            {
                ct_players.Add(i);
            }
        }

        if(ts_players.Count >= 1)
        {
            int player_index = ts_players[new Random().Next(0, ts_players.Count)];
            bombOwner = player_index;
        }

        numT = ts_players.Count;
        numCT = ct_players.Count;

        activePlayers = numCT + numT;

        SetupPlayers(players);

        return HookResult.Continue;
    }

    private static HookResult OnRoundPostStart(EventRoundPoststart @event, GameEventInfo info)
    {
        if(!isLive())
        {
            return HookResult.Continue;
        }

        RoundTime = main_config.ROUND_TIME;
        PrintToChatAll($"{PREFIX} Retake \x04{(currentSite == Site.A ? "A" : "B")}\x01: \x07{numT} Ts \x01vs \x0E{numCT} CTs");

        isBombPlanted = false;
        return HookResult.Continue;
    }

    private static HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
    {
        foreach(Player player in players)
        {
            player.roundPoints = 0;
        }

        return HookResult.Continue;
    }

    private static HookResult OnPlayerTeam(EventPlayerTeam @event, GameEventInfo info)
    {
        @event.Silent = true;
        return HookResult.Continue;
    }

    private static HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        if(!isLive())
        {
            return HookResult.Continue;
        }

        CCSPlayerController player_controller = @event.Userid;

        if(player_controller == null! || !player_controller.IsValid)
        {
            return HookResult.Continue;
        }

        Player player = FindPlayer(player_controller);

        if(player == null! || !player.IsValid())
        {
            return HookResult.Continue;
        }

        player.selectSpawnCallCount = 0;
        spawnPoints.TeleportToSpawn(player_controller, spawnPoints.SelectSpawn(player));
        player.CreateSpawnDelay();

        return HookResult.Continue;
    }
}