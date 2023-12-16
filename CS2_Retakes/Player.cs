using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Weapons;

namespace Retakes;

public class Player
{

    public CCSPlayerController player;

    public WeaponsAllocator weaponsAllocator;

    public Player(CCSPlayerController player)
    {
        this.player = player;
        weaponsAllocator = new WeaponsAllocator(player);
    }

    public static void AllocateWeapons(List<Player> players)
    {
        bool giveawp_t = true;
        bool giveawp_ct = true;

        foreach(Player player in players)
        {
            CsTeam team = player.GetTeam();
            if(team == CsTeam.Terrorist)
            {
                giveawp_t = !player.weaponsAllocator.Allocate(giveawp_t);
            } else if (team == CsTeam.CounterTerrorist) {
                giveawp_ct = !player.weaponsAllocator.Allocate(giveawp_ct);
            }
        }
    }

    public CsTeam GetTeam()
    {
        return (CsTeam)player.TeamNum;
    }

    public string GetSteamID2()
    {
        if(player == null! || !player.IsValid)
        {
            return string.Empty;
        }

        if(player.AuthorizedSteamID == null!)
        {
            return string.Empty;
        }

        return player.AuthorizedSteamID.SteamId2;
    }
}