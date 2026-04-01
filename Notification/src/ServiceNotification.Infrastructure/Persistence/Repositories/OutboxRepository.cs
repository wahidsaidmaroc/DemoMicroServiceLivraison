using Microsoft.EntityFrameworkCore;
using ServiceNotification.Domain.Entities;
using ServiceNotification.Domain.Interfaces;

namespace ServiceNotification.Infrastructure.Persistence.Repositories;

public class OutboxRepository(AppDbContext dbContext) : IOutboxRepository
{
    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await dbContext.OutboxMessages.AddAsync(message, cancellationToken);
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        return await dbContext.OutboxMessages
            .Where(o => o.ProcessedAt == null)
            .OrderBy(o => o.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public void MarkAsProcessed(OutboxMessage message)
    {
        message.MarkAsProcessed();
        dbContext.OutboxMessages.Update(message);
    }
}
