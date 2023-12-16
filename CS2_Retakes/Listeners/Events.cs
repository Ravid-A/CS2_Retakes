using CounterStrikeSharp.API.Core;

using MySqlConnector;

using static Retakes.Core;

namespace Retakes;

class Events
{
    public static HookResult OnPlayerConnect(EventPlayerConnect @event, GameEventInfo info)
    {
        CCSPlayerController player_controller = @event.Userid;

        if(!player_controller.IsValid)
        {
            return HookResult.Continue;
        }

        Player player = new Player(player_controller);

        players.Add(player);

        int index = players.IndexOf(player);

        db.Query(SQL_FetchUser_CB, $"SELECT * FROM `weapon` WHERE `steamid` = '{player.GetSteamID2()}'", index);
        return HookResult.Continue;
    }

    public static HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        CCSPlayerController player = @event.Userid;

        if(!player.IsValid)
        {
            return HookResult.Continue;
        }

        Player player_obj = FindPlayer(player);

        if(player_obj == null!)
        {
            return HookResult.Continue;
        }

        players.Remove(player_obj);

        db.Query(SQL_CheckForErrors, $"UPDATE `weapon` SET `t_primary` = '{player_obj.weaponsAllocator.primaryWeapon_t}', `ct_primary` = '{player_obj.weaponsAllocator.primaryWeapon_ct}', `secondary` = '{player_obj.weaponsAllocator.secondaryWeapon}', `give_awp` = '{(int)player_obj.weaponsAllocator.giveAWP}' WHERE `steamid` = '{player_obj.GetSteamID2()}'");

        return HookResult.Continue;
    }

    public static HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        CCSPlayerController player = @event.Userid;

        if(!player.IsValid)
        {
            return HookResult.Continue;
        }

        

        return HookResult.Continue;
    }
}