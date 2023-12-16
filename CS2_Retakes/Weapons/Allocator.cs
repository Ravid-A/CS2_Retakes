using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Entities;

namespace Weapons;

public enum GiveAWP
{
    NEVER,
    SOMETIMES,
    ALWAYS
}

public class Weapon
{
    public string name = string.Empty;
    public string display_name = string.Empty;

    public Weapon(string name, string display_name)
    {
        this.name = name;
        this.display_name = display_name;
    }
}



public class WeaponsAllocator
{
    public static Weapon[] primary_t =
    {
        new Weapon("weapon_ak47", "AK-47"),
        new Weapon("weapon_sg556", "SG 553")
    };

    public static Weapon[] primary_ct =
    {
        new Weapon("weapon_m4a1", "M4A4"),
        new Weapon("weapon_m4a1_silencer", "M4A1-S"),
        new Weapon("weapon_aug", "AUG")
    };

    public static Weapon[] pistols = 
    {
        new Weapon("weapon_glock", "Glock-18"),
        new Weapon("weapon_hkp2000", "P2000"),
        new Weapon("weapon_usp_silencer", "USP-S"),
        new Weapon("weapon_p250", "P250"),
        new Weapon("weapon_tec9", "Tec-9"),
        new Weapon("weapon_fiveseven", "Five-Seven"),
        new Weapon("weapon_deagle", "Desert Eagle")
    };

    public CCSPlayerController player;

    public int primaryWeapon_t = 0;
    public int primaryWeapon_ct = 0;
    public int secondaryWeapon = 0;

    public GiveAWP giveAWP = GiveAWP.NEVER;

    public WeaponsAllocator(CCSPlayerController player)
    {
        this.player = player;
    }

    public bool Allocate(bool give_awp)
    {
        bool gave_awp = false;

        if (player == null || !player.IsValid)
        {
            return gave_awp;
        }

        if(!player.PawnIsAlive)
        {
            return gave_awp;
        }

        if(player.TeamNum < (byte)CsTeam.Terrorist || player.TeamNum > (byte)CsTeam.CounterTerrorist)
        {
            return gave_awp;
        }

        string primary = string.Empty;

        if(give_awp && giveAWP != GiveAWP.NEVER)
        {
            if(giveAWP == GiveAWP.ALWAYS)
            {
                gave_awp = true;
            }
            else
            {
                gave_awp = new Random().Next(0, 1) == 1;
            }

            if(gave_awp)
            {
                primary = "weapon_awp";
            }
        }
        else
        {
            if(player.TeamNum == (byte)CsTeam.Terrorist)
            {
                primary = primary_t[primaryWeapon_t].name;
            }else{
                primary = primary_ct[primaryWeapon_ct].name;
            }
        }

        string secondary = pistols[secondaryWeapon].name;

        player.RemoveWeapons();
        player.GiveNamedItem(primary);
        player.GiveNamedItem(secondary);

        return gave_awp;
    }
}