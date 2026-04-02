using Security.Api.Application.DTOs.Authentication;

namespace Security.Api.Application.Abstractions.Authentication;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}