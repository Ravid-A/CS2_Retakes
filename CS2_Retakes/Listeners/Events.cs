using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

using static Retakes.Functions;

namespace Retakes;

class Events
{
    public static HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        return HookResult.Continue;
    }
}