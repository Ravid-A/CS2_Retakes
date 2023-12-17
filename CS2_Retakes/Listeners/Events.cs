using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

using static Retakes.Core;
using static Retakes.Functions;

using static Retakes.Player;

namespace Retakes;

class EventsHandlers
{

    public static HookResult OnRoundPreStart(EventRoundPrestart @event, GameEventInfo info)
    {
        if(WarmupRunning)
        {
            return HookResult.Continue;
        }

        Server.ExecuteCommand("mp_buy_anywhere 0");
        Server.ExecuteCommand("mp_buytime 0");
        Server.ExecuteCommand("mp_startmoney 0");

        return HookResult.Continue;
    }

    public static HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(!WarmupRunning)
        {
            AllocateWeapons(players);
        }

        return HookResult.Continue;
    }
}