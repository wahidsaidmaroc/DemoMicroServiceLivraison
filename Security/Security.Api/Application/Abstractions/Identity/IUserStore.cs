using Security.Api.Domain.Identity;

namespace Security.Api.Application.Abstractions.Identity;

public interface IUserStore
{
    Task<AppUser?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
}