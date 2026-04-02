using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Security.Api.Application.Abstractions.Authentication;
using Security.Api.Application.Abstractions.Identity;
using Security.Api.Application.DTOs.Authentication;
using Security.Api.Domain.Identity;

namespace Security.Api.Infrastructure.Authentication;

public sealed class AuthService(
    IOptions<JwtOptions> jwtOptions,
    IUserStore userStore,
    IPasswordHasher<AppUser> passwordHasher,
    TimeProvider timeProvider,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();
        var user = await userStore.FindByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("Authentication failed for {Email}: user not found.", request.Email);
            return null;
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult is PasswordVerificationResult.Failed)
        {
            logger.LogWarning("Authentication failed for {Email}: invalid password.", request.Email);
            return null;
        }

        return GenerateToken(user);
    }

    private AuthResponse GenerateToken(AppUser user)
    {
        var expiresAtUtc = timeProvider.GetUtcNow().AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtClaimNames.Email, user.Email),
            new(JwtClaimNames.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtClaimNames.NameIdentifier, user.Id.ToString())
        };

        claims.AddRange(user.Roles.Select(role => new Claim(JwtClaimNames.Role, role)));

        var signingCredentials = new SigningCredentials(
            JwtSigningKeyFactory.Create(_jwtOptions.SecretKey),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAtUtc.UtcDateTime,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthResponse(
            tokenHandler.WriteToken(token),
            expiresAtUtc,
            "Bearer",
            user.UserName,
            user.Email,
            user.Roles);
    }
}