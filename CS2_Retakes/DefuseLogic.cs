using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using static Retakes.Core;
using static Retakes.Functions;

namespace Retakes;

public class DefuseLogic
{
    private static CCSPlayerController defuser = null!;

    static ConVar inferno_max_range = null!;
    static ConVar mp_round_restart_delay = null!;
    static ConVar mp_friendlyfire = null!;

    public static void DefuseLogic_OnLoad()
    {
        if ((inferno_max_range = ConVar.Find("inferno_max_range")!) == null!)
        {
            ThrowError("Failed to find convar 'inferno_max_range'");
        }

        if ((mp_round_restart_delay = ConVar.Find("mp_round_restart_delay")!) == null!)
        {
            ThrowError("Failed to find convar 'mp_round_restart_delay'");
        }

        if ((mp_friendlyfire = ConVar.Find("mp_friendlyfire")!) == null!)
        {
            ThrowError("Failed to find convar 'mp_friendlyfire'");
        }
    }

    public static void DefuseLogic_OnBeginDefuse(CCSPlayerController player)
    {
        if (!main_config.insta_defuse)
        {
            return;
        }

        defuser = player;
        
        InstaDefuseAttemptEx();
    }

    public static void DefuseLogic_OnBombAbortDefused()
    {
        defuser = null!;
    }

    public static void DefuseLogic_OnBombDefused()
    {
        PrintToChatAll($"{PREFIX}\x07 Attackers\x01 defused the bomb in time!");
    }

    public static void DefuseLogic_OnPlayerDeath(CCSPlayerController player)
    {
        if (!main_config.insta_defuse)
        {
            return;
        }

        if (player.TeamNum != (byte)CsTeam.Terrorist)
        {
            return;
        }

        if(defuser == null! || !defuser.IsValid)
        {
            return;
        }

        InstaDefuseAttemptEx();
    }

    public static void DefuseLogic_OnInfernoExpire()
    {
        if (!main_config.insta_defuse)
        {
            return;
        }

        if (defuser == null! || !defuser.IsValid)
        {
            return;
        }

        InstaDefuseAttemptEx();
    }

    private static void InstaDefuseAttemptEx()
    {
        Server.NextFrame(() =>
        {
            CPlantedC4 c4 = GetPlantedC4();

            if (c4 == null!)
            {
                return;
            }

            //InstaDefuseAttempt(c4);
        });
    }

    private static void DefusePlantedC4(CPlantedC4 planted_c4)
    {
        planted_c4.DefuseCountDown = 0.0f;
    }

    private static CPlantedC4 GetPlantedC4()
    {
        var c4list = Utilities.FindAllEntitiesByDesignerName<CPlantedC4>("planted_c4");

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