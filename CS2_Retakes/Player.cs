using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

using Weapons;

using static Retakes.Core;
using static Retakes.Functions;

namespace Retakes;

public class Player
{
    public int index => FindPlayer(this);

    public CCSPlayerController player;

    public WeaponsAllocator weaponsAllocator;

    public bool inGunMenu = false;

    public int roundPoints = 0;

    public bool isBomberOwner => index == bombOwner;

    public Player(CCSPlayerController player)
    {
        this.player = player;
        weaponsAllocator = new WeaponsAllocator(player);
    }

    public static void SetupPlayers(List<Player> players)
    {
        bool giveawp_t = true;
        bool giveawp_ct = true;

        foreach(Player player in players)
        {
            CsTeam team = player.GetTeam();
            bool giveawp = player.weaponsAllocator.SetupGiveAwp();

            player.weaponsAllocator.give_awp = false;

            if(giveawp)
            {
                if(team == CsTeam.Terrorist && giveawp_t)
                {
                    player.weaponsAllocator.give_awp = true;
                    giveawp_t = false;
                }

                if(team == CsTeam.CounterTerrorist && giveawp_ct)
                {
                    player.weaponsAllocator.give_awp = true;
                    giveawp_ct = false;
                }
            }
        }
    }

    public CsTeam GetTeam()
    {
        return (CsTeam)player.TeamNum;
    }

    public SteamID? GetSteamID()
    {
        if(player == null! || !player.IsValid)
        {
            return null!;
        }

        if(player.AuthorizedSteamID == null!)
        {
            return null!;
        }

        return player.AuthorizedSteamID;
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

    public string GetName()
    {
        if(player == null! || !player.IsValid)
        {
            return string.Empty;
        }

        return player.PlayerName;
    }

    public bool IsValid()
    {
        return !(player == null! || !player.IsValid);
    }

    public void CreateSpawnDelay()
    {
        _plugin.AddTimer(1f, Timer_StartPlant);
        _plugin.AddTimer(.05f, Timer_GiveWeapons);
    }

    private void Timer_StartPlant()
    {
        if(!isLive())
        {
            return;
        }

        isBombPlantSignal = true;
    }

    private void Timer_GiveWeapons()
    {
        if(!isLive())
        {
            return;
        }

        weaponsAllocator.Allocate(isBomberOwner);
    }
}