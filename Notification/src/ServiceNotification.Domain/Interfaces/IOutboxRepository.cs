using ServiceNotification.Domain.Entities;

namespace ServiceNotification.Domain.Interfaces;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(int batchSize, CancellationToken cancellationToken = default);
    void MarkAsProcessed(OutboxMessage message);
}
