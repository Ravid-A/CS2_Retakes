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
            CreateNaturalPlantedC4();
        }
    }

    public static void PlantLogic_OnClientDisconnect(CCSPlayerController client)
    {
        if (GetPlanter() != client || !FreezePeriod)
        {
            return;
        }

        CreateNaturalPlantedC4();
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

    public static void PlantLogic_OnBombPlanted(CCSPlayerController planter)
    {
        SetFreezePeriod(false); 

        PrintToChatAll($"{PREFIX} \x07{planter.PlayerName}\x01 has planted the bomb!");
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

        Vector playerOrigin = planter.PlayerPawn!.Value!.AbsOrigin!;

        if(playerOrigin == null!)
        {
            return false;
        }

        playerOrigin.Z -= planter.PlayerPawn.Value.Collision.Mins.Z;

        CBaseModelEntity prop = Utilities.CreateEntityByName<CBaseModelEntity>("planted_c4")!;

        if (prop == null!) 
        {
            return false;
        }

        prop.DispatchSpawn();

        CPlantedC4 plantedC4 = new CPlantedC4(prop.Handle);

        Server.NextFrame(() =>
        {
            prop.Teleport(playerOrigin, new QAngle(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero), new Vector(0, 0, 0));

            BombPlanted = true;

            plantedC4.BombTicking = true;

            NotifyBombPlanted(planter);
            RemoveC4FromPlayer(planter);
        });
        return true;
    }

    //A cheaty to remove the c4 from the player
    private static bool RemoveC4FromPlayer(CCSPlayerController player)
    {
        if(player == null! || !player.IsValid)
        {
            return false;
        }

        player.ExecuteClientCommand("slot5");

        var active_weapon = player.PlayerPawn.Value!.WeaponServices!.ActiveWeapon.Value;

        if(active_weapon == null)
        {
            return false;
        }

        player.DropActiveWeapon();
        active_weapon.Remove();
        
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
}