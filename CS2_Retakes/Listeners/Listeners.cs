using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

using static Retakes.Core;
using static Retakes.Functions;

namespace Retakes;

class ListenersHandlers
{
    public static void OnClientConnected(int playerSlot)
    {
        CCSPlayerController player = Utilities.GetPlayerFromSlot(playerSlot);
        AddPlayerToList(player);
    }

    public static void AddPlayerToList(CCSPlayerController player)
    {
        if(player is null || !player.IsValid || player.IsBot)
                return;

        Player player_obj = new Player(player);

        players.Add(player_obj);

        int index = players.IndexOf(player_obj);

        db.Query(SQL_FetchUser_CB, $"SELECT * FROM `weapons` WHERE `auth` = '{player_obj.GetSteamID2()}'", index);
    }

    public static void OnClientDisconnect(int playerSlot)
    {
        CCSPlayerController player = Utilities.GetPlayerFromSlot(playerSlot);
        RemovePlayerFromList(player);
    }

    private static void RemovePlayerFromList(CCSPlayerController player)
    {
        if(player is null || !player.IsValid || player.IsBot)
                return;

        Player player_obj = FindPlayer(player);

        if(player_obj == null!)
        {
            return;
        }

        db.Query(SQL_CheckForErrors, $"UPDATE `weapons` SET `t_primary` = '{player_obj.weaponsAllocator.primaryWeapon_t}', `ct_primary` = '{player_obj.weaponsAllocator.primaryWeapon_ct}', `secondary` = '{player_obj.weaponsAllocator.secondaryWeapon}', `give_awp` = '{(int)player_obj.weaponsAllocator.giveAWP}' WHERE `auth` = '{player_obj.GetSteamID2()}'");

        players.Remove(player_obj);
    }
}