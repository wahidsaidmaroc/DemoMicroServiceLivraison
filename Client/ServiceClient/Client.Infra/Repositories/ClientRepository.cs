using Client.Application.Interfaces;
using Client.Infra.Persistence;
using ClientEntity = Client.Domain.Entities.Client;

namespace Client.Infra.Repositories;

public class ClientRepository(ClientDbContext db) : IClientRepository
{
    public async Task AddRangeAsync(IEnumerable<ClientEntity> clients, CancellationToken cancellationToken = default)
    {
        await db.Clients.AddRangeAsync(clients, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}
