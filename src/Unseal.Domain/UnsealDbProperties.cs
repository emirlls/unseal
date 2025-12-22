namespace Unseal;

public static class UnsealDbProperties
{
    public static string DbTablePrefix { get; set; } = "Unseal";

    public static string? DbSchema { get; set; } = null;

    public const string ConnectionStringName = "Unseal";
}
