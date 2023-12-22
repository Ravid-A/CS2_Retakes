using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

using static Retakes.Core;

namespace Retakes;

public class PlantLogic
{
    public static void PlantLogic_OnLoad()
    {
        
    }

    public static void PlantLogic_OnBeginPlant(CCSPlayerController player)
    {
        CC4 c4 = GetWeaponC4();

        if(c4 == null!)
        {
            return;
        }

        ForceC4Plant(c4);
    }

    public static void PlantLogic_OnBombPlanted()
    {
        SetFreezePeriod(false);
    }

    private static void SetFreezePeriod(bool value)
    {
        if (FreezePeriod == value)
        {
            return;
        }

        FreezePeriod = value;

        if (!value)
        {
            NotifyRoundFreezeEnd();
        }
    }

    private static void ForceC4Plant(CC4 weapon_c4)
    {
        if (!main_config.insta_plant)
        {
            return;
        }

        weapon_c4.BombPlacedAnimation = false;
        weapon_c4.ArmedTime = 0.0f;
    }

    private static void NotifyRoundFreezeEnd()
    {
        EventRoundFreezeEnd freezeEnd = new EventRoundFreezeEnd(true);
        
        if(freezeEnd == null!)
        {
            return;
        }

        freezeEnd.FireEvent(false);
    }

    private static void NotifyBombPlanted(CCSPlayerController player)
    {
        EventBombPlanted bombPlanted = new EventBombPlanted(true);

        if(bombPlanted == null!)
        {
            return;
        }

        bombPlanted.Userid = player;
        bombPlanted.Site = (int)currentSite;
        bombPlanted.FireEvent(false);
    }

    public static CC4 GetWeaponC4()
    {
        var c4list = Utilities.FindAllEntitiesByDesignerName<CC4>("weapon_c4");

        if (!c4list.Any())
        {
            return null!;
        }

        var c4 = c4list.FirstOrDefault();

        if (c4 == null)
        {
            return null!;
        }

        return c4;
    }
}