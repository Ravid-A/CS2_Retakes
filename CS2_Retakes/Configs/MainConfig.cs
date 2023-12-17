namespace Configs;

public class MainConfig
{
    public PREFIXS prefixs { get; set; } = null!;
    public required bool DEBUG { get; init; } = false;
    public required bool use_db { get; init; } = false;
}

public class PREFIXS
{
    public required string PREFIX { get; init; }
    public required string PREFIX_CON { get; init; }
    public required string PREFIX_MENU { get; init; }
}