
using Retakes;
using static Retakes.Core;
using static Retakes.Functions;

namespace Managers;

public class QueueManager
{
    private int max_players => main_config.MAX_PLAYERS;
    private float terroristRatio => main_config.TerrroristRatio;

    private readonly Queue<Player> activePlayers = new();
    private readonly Queue<Player> waitingPlayers = new();
}