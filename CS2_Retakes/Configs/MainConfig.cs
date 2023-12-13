namespace Retakes;

class MainConfig
{
    public PREFIXS prefixs { get; set; } = null!;
    public required bool DEBUG { get; init; } = false;
}

class PREFIXS
{
    public required string PREFIX { get; init; }
    public required string PREFIX_CON { get; init; }
}