using ClientEntity = Client.Domain.Entities.Client;

namespace Client.Application.Interfaces;

public interface IClientRepository
{
    Task AddRangeAsync(IEnumerable<ClientEntity> clients, CancellationToken cancellationToken = default);
}
