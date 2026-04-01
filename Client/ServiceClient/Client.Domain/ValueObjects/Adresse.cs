namespace Client.Domain.ValueObjects;

public sealed record Adresse
{
    public string LigneComplete { get; init; } = string.Empty;

    public string Ville { get; init; } = string.Empty;

    public string Pays { get; init; } = string.Empty;
}