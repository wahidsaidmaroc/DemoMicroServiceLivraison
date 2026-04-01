namespace ServiceNotification.Domain.Interfaces;

public interface INotificationRepository
{
    Task<Entities.Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.Notification>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Entities.Notification notification, CancellationToken cancellationToken = default);
    void Update(Entities.Notification notification);
}
