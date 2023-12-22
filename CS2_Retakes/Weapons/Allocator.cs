using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace Weapons;

using static Retakes.Core;

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
    public enum WeaponType
    {
        PRIMARY_T,
        PRIMARY_CT,
        SECONDARY
    };

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
    public bool give_awp = false;

    public char nades = '\0';

    public WeaponsAllocator(CCSPlayerController player)
    {
        this.player = player;
    }

    public static int GetWeaponIndex(string weapon, WeaponType type)
    {
        switch(type)
        {
            case WeaponType.PRIMARY_T:
            {
                for(int i = 0; i < primary_t.Length; i++)
                {
                    if(primary_t[i].display_name == weapon)
                    {
                        return i;
                    }
                }
                break;
            }
            case WeaponType.PRIMARY_CT:
            {
                for(int i = 0; i < primary_ct.Length; i++)
                {
                    if(primary_ct[i].display_name == weapon)
                    {
                        return i;
                    }
                }
                break;
            }
            case WeaponType.SECONDARY:
            {
                for(int i = 0; i < pistols.Length; i++)
                {
                    if(pistols[i].display_name == weapon)
                    {
                        return i;
                    }
                }
                break;
            }
        }

        return -1;
    }

    public bool SetupGiveAwp()
    {
        bool give_awp;
        if (giveAWP == GiveAWP.ALWAYS)
        {
            give_awp = true;
        }
        else
        {
            give_awp = new Random().Next(0, 2) == 1;
        }

        return give_awp;
    }

    public void Allocate(bool bombOwner = false)
    {
        if (player == null || !player.IsValid)
        {
            return;
        }

        if(!player.PawnIsAlive)
        {
            return;
        }

        if(player.TeamNum < (byte)CsTeam.Terrorist || player.TeamNum > (byte)CsTeam.CounterTerrorist)
        {
            return;
        }

        string primary;
        if (give_awp )
        {
            primary = "weapon_awp";
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
        player.GiveNamedItem("weapon_knife");

        string nade = SelectNade();

        player.GiveNamedItem(nade);

        if(bombOwner)
        {
            player.GiveNamedItem("weapon_c4");
            player.ExecuteClientCommand("slot5");
            player.PlayerPawn!.Value!.CanMoveDuringFreezePeriod = true;
        }
    }

    private string SelectNade()
    {
        string nade = string.Empty;

        int rand = new Random().Next(0,3);

        switch(rand)
        {
            case 0: 
                nade = "weapon_hegrenade";
                break;
            case 1:
                nade = "weapon_flashbang";
                break;
            case 2:
                nade = "weapon_smokegrenade";
                break;
        }

        return nade;
    }
}