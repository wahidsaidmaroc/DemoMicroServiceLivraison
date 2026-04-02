using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Security.Api.Application.Abstractions.Authentication;
using Security.Api.Application.Abstractions.Identity;
using Security.Api.Application.Authorization;
using Security.Api.Domain.Authorization;
using Security.Api.Domain.Identity;
using Security.Api.Infrastructure.Authentication;
using Security.Api.Infrastructure.Identity;
using Security.Api.Presentation.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services
    .AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .ValidateDataAnnotations()
    .Validate(
        static options => !string.IsNullOrWhiteSpace(options.SecretKey) && options.SecretKey.Length >= 32,
        "Jwt:SecretKey must contain at least 32 characters.")
    .ValidateOnStart();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
builder.Services.AddSingleton<IUserStore, InMemoryUserStore>();
builder.Services.AddScoped<IAuthService, AuthService>();

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration section is missing.");

var signingKey = JwtSigningKeyFactory.Create(jwtOptions.SecretKey);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = false;
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Ensure the token was issued by the trusted authority configured for this API.
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            // Ensure the token was created for this API and not replayed against another audience.
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            // Reject tokens whose signature does not match the secret configured by the application.
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            // Enforce strict expiration handling and remove the default clock tolerance.
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = JwtClaimNames.Name,
            RoleClaimType = JwtClaimNames.Role
        };
    });

builder.Services
    .AddAuthorizationBuilder()
    .AddPolicy(AuthorizationPolicies.AdminOnly, policy => policy.RequireRole(ProfileNames.Administrator))
    .AddPolicy(AuthorizationPolicies.BackOffice, policy =>
        policy.RequireRole(ProfileNames.Administrator, ProfileNames.Operator));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();

app.MapGet("/me", (ClaimsPrincipal user) =>
    TypedResults.Ok(new
    {
        Name = user.Identity?.Name,
        Email = user.FindFirstValue(JwtClaimNames.Email),
        Roles = user.FindAll(JwtClaimNames.Role).Select(claim => claim.Value).ToArray()
    }))
    .RequireAuthorization()
    .WithName("GetCurrentProfile");

app.MapGet("/admin/dashboard", () =>
        TypedResults.Ok(new { Message = "Administrator access granted." }))
    .RequireAuthorization(AuthorizationPolicies.AdminOnly)
    .WithName("GetAdminDashboard");

app.MapGet("/backoffice/operations", () =>
        TypedResults.Ok(new { Message = "Back-office access granted." }))
    .RequireAuthorization(AuthorizationPolicies.BackOffice)
    .WithName("GetBackOfficeOperations");

app.Run();

public partial class Program;
