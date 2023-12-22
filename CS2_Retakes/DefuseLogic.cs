using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities.Constants;
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

    static bool SentNotificiation = false;

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
        _plugin.AddTimer(0.05f, () =>
        {
            CPlantedC4 c4 = GetPlantedC4();

            if (c4 == null!)
            {
                return;
            }

            InstaDefuseAttempt(c4);
        });
    }

    static void InstaDefuseAttempt(CPlantedC4 c4)
    {
        CCSPlayerController client = defuser;
        
        if(client == null! || !client.IsValid)
        {
            return;
        }

        int alive_defenders = GetTeamClientCount(CsTeam.Terrorist, true);
        if (alive_defenders >= 1)
        {
            return;
        }

        // Time left till the bomb explosion. In seconds.
        float remaining_time = c4.C4Blow - Server.GameFrameTime;

        // Time taken for the defuser to successfully defuse the bomb. (accounts for defuse kits)
        float defuse_time = c4.DefuseLength;

        // Note: Enough time to defuse would be: [remaining_time >= defuse_time]
        if (remaining_time >= defuse_time && IsInfernoNearC4(c4, client))
        {
            if (!SentNotificiation)
            {
                PrintToChatAll($"{PREFIX}\x07 Inferno\x01 is close to the C4, be careful defusing!");
                SentNotificiation = true;
            }

            return;
        }
        else
        {
            SentNotificiation = false;
        }

        if (remaining_time < defuse_time)
        {
            if (!SentNotificiation)
            {
                PrintToChatAll($"{PREFIX}\x07 Attackers\x01 did not defuse in time!\x08 {remaining_time}s\x01 remaining.");

                if (main_config.explode_no_time)
                {
                    c4.C4Blow = 1.0f;
                }

               
                float restart_delay = mp_round_restart_delay.GetPrimitiveValue<float>();

                // TODO: Check if round termination is needed even if 'retakes_explode_no_time' is enabled.
                TerminateRound(restart_delay - 1.0f, RoundEndReason.TerroristsPlanned);

                SentNotificiation = true;
            }

            return;
        }

        DefusePlantedC4(c4);
    }

    private static void DefusePlantedC4(CPlantedC4 planted_c4)
    {
        planted_c4.DefuseCountDown = 0.0f;
    }

    private static bool IsInfernoNearC4(CPlantedC4 planted_c4, CCSPlayerController defuser)
    {
        // Retrieve the planted c4 origin.
        Vector c4_origin = planted_c4.AbsOrigin!;

        if(c4_origin == null!)
        {
            return false;
        }

        var inferno_list = Utilities.FindAllEntitiesByDesignerName<CInferno>("inferno");

        if (!inferno_list.Any())
        {
            return false;
        }

        // Loop through all infernos.
        foreach(CInferno ent in inferno_list)
        {
            if(ent == null!)
            {
                continue;
            }

            // Exclude friendly inferno.
            // Exclude only if Owner
            CBaseEntity inferno_owner = ent.OwnerEntity!.Value!;

            if(inferno_owner != null! && inferno_owner.IsValid)
            {
                bool friendly_fire = mp_friendlyfire.GetPrimitiveValue<bool>();

                if (defuser != inferno_owner && !friendly_fire && inferno_owner.TeamNum == (byte)CsTeam.CounterTerrorist)
                {
                    continue;
                }
            }

            // Retrieve the inferno center origin and compare it to the planted c4 origin.
            Vector inferno_origin = ent.AbsOrigin!;

            if (inferno_origin == null!)
            {
                continue;
            }

            float max_range = inferno_max_range.GetPrimitiveValue<float>();

            if (GetVectorDistance(c4_origin, inferno_origin) <= max_range)
            {
                // Inferno is close enough to the planted c4, return true!
                return true;
            }
        }

        return false;
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