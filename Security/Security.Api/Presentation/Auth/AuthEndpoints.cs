using Microsoft.AspNetCore.Http.HttpResults;
using Security.Api.Application.Abstractions.Authentication;
using Security.Api.Application.DTOs.Authentication;

namespace Security.Api.Presentation.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/login", LoginAsync)
            .AllowAnonymous()
            .WithName("Login")
            .WithSummary("Authenticates a user and returns a JWT bearer token.");

        return endpoints;
    }

    private static async Task<Results<Ok<AuthResponse>, UnauthorizedHttpResult, ValidationProblem>> LoginAsync(
        LoginRequest request,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors[nameof(request.Email)] = ["Email is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors[nameof(request.Password)] = ["Password is required."];
        }

        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var response = await authService.LoginAsync(request, cancellationToken);
        return response is null ? TypedResults.Unauthorized() : TypedResults.Ok(response);
    }
}