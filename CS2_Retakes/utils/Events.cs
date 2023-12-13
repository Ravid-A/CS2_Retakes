using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

using static Retakes.Core;
using static Retakes.Functions;

namespace Retakes;

class Events
{
    public static HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        CCSPlayerController player = @event.Userid;
        CCSPlayerController attacker = @event.Attacker;

        PrintToChatAll($"{PREFIX} {player.PlayerName} was killed by {attacker.PlayerName}");
        return HookResult.Continue;
    }
}