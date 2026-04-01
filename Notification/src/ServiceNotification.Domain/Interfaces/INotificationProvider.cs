using ServiceNotification.Domain.Enums;

namespace ServiceNotification.Domain.Interfaces;

public interface INotificationProvider
{
    NotificationChannel Channel { get; }
    Task<bool> SendAsync(Entities.Notification notification, CancellationToken cancellationToken = default);
}
