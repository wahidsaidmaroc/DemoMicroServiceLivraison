using System.ComponentModel.DataAnnotations;

namespace Security.Api.Infrastructure.Authentication;

public sealed record JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Required]
    public string SecretKey { get; init; } = string.Empty;

    [Range(1, 1440)]
    public int AccessTokenExpirationMinutes { get; init; } = 60;
}