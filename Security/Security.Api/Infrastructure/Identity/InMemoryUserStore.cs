using Microsoft.AspNetCore.Identity;
using Security.Api.Application.Abstractions.Identity;
using Security.Api.Domain.Authorization;
using Security.Api.Domain.Identity;

namespace Security.Api.Infrastructure.Identity;

public sealed class InMemoryUserStore(IPasswordHasher<AppUser> passwordHasher) : IUserStore
{
    private readonly IReadOnlyDictionary<string, AppUser> _users = CreateSeedUsers(passwordHasher);

    public Task<AppUser?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        _users.TryGetValue(email.Trim().ToUpperInvariant(), out var user);
        return Task.FromResult(user);
    }

    private static IReadOnlyDictionary<string, AppUser> CreateSeedUsers(IPasswordHasher<AppUser> passwordHasher)
    {
        var users = new[]
        {
            CreateUser(
                passwordHasher,
                "admin@security.local",
                "admin",
                "Admin@12345",
                [ProfileNames.Administrator, ProfileNames.Operator]),
            CreateUser(
                passwordHasher,
                "operator@security.local",
                "operator",
                "Operator@12345",
                [ProfileNames.Operator])
        };

        return users.ToDictionary(user => user.Email.ToUpperInvariant(), StringComparer.Ordinal);
    }

    private static AppUser CreateUser(
        IPasswordHasher<AppUser> passwordHasher,
        string email,
        string userName,
        string password,
        IReadOnlyCollection<string> roles)
    {
        var user = new AppUser(Guid.NewGuid(), email, userName, string.Empty, roles);
        var passwordHash = passwordHasher.HashPassword(user, password);
        return user with { PasswordHash = passwordHash };
    }
}