using Microsoft.EntityFrameworkCore;
using ServiceNotification.Domain.Enums;
using ServiceNotification.Domain.Interfaces;

namespace ServiceNotification.Infrastructure.Persistence.Repositories;

public class NotificationRepository(AppDbContext dbContext) : INotificationRepository
{
    public async Task<Domain.Entities.Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Notifications.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<Domain.Entities.Notification>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Notifications
            .Where(n => n.Status == NotificationStatus.Pending || n.Status == NotificationStatus.Retrying)
            .OrderBy(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Domain.Entities.Notification notification, CancellationToken cancellationToken = default)
    {
        await dbContext.Notifications.AddAsync(notification, cancellationToken);
    }

    public void Update(Domain.Entities.Notification notification)
    {
        dbContext.Notifications.Update(notification);
    }
}
