namespace Security.Api.Domain.Identity;

public sealed record AppUser(
    Guid Id,
    string Email,
    string UserName,
    string PasswordHash,
    IReadOnlyCollection<string> Roles);