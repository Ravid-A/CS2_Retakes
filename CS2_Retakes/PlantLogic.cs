using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

using static Retakes.Core;
using static Retakes.Functions;
using static Retakes.DefuseLogic;

namespace Retakes;

public class PlantLogic
{
    public static void PlantLogic_OnLoad()
    {
        
    }

    public static void PlantLogic_OnRoundFreezeEnd()
    {
        CPlantedC4 planted_c4 = GetPlantedC4();
        if (planted_c4 == null!)
        {
            //CreateNaturalPlantedC4();
        }
    }

    public static void PlantLogic_OnClientDisconnect(CCSPlayerController client)
    {
        if (GetPlanter() != client || !FreezePeriod)
        {
            return;
        }

        //CreateNaturalPlantedC4();
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

    private static bool CreateNaturalPlantedC4()
    {
        if (!main_config.auto_plant)
        {
            return false;
        }

        CCSPlayerController planter = GetPlanter();
        if (planter == null! || !planter.IsValid)
        {
            return false;
        }

        Vector plant_origin = planter.AbsOrigin!;

        CPlantedC4 planted_c4 = Utilities.CreateEntityByName<CPlantedC4>("planted_c4")!;

        if (planted_c4 == null!)
        {
            return false;
        }

        planted_c4.DispatchSpawn();

        _plugin.AddTimer(0.05f, () =>
        {
            planted_c4.Teleport(plant_origin, new(0f, 0f, 0f), new(0f, 0f, 0f));
            planted_c4.BombTicking = true;
        });

        RemoveBombWeapons(planter);

        NotifyBombPlanted(planter);

        return true;
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

    // Removes any 'weapon_c4' entities.
    private static void RemoveBombWeapons(CCSPlayerController player)
    {
        Utilities.RemoveItemByDesignerName(player, "weapon_c4");
    }
}