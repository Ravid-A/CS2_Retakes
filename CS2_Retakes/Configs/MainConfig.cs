namespace Configs;

public class MainConfig
{
    public PREFIXS prefixs { get; set; } = null!;
    public required bool DEBUG { get; init; } = false;
    public required bool use_db { get; init; } = false;
    public int WARMUP_TIME { get; init; } = 12;
    public int MAX_PLAYERS { get; init; } = 9;
    public int MIN_PLAYERS { get; init; } = 2;
    public int ROUND_TIME { get; init; } = 12;
    public bool auto_plant { get; init; } = false;
}

public class PREFIXS
{
    public required string PREFIX { get; init; }
    public required string PREFIX_CON { get; init; }
    public required string PREFIX_MENU { get; init; }
}