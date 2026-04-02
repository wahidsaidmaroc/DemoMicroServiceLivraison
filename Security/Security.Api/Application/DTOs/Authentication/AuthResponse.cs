namespace Security.Api.Application.DTOs.Authentication;

public sealed record AuthResponse(
    string AccessToken,
    DateTimeOffset ExpiresAtUtc,
    string TokenType,
    string UserName,
    string Email,
    IReadOnlyCollection<string> Roles);