using Microsoft.Extensions.Logging;
using ServiceNotification.Domain.Enums;
using ServiceNotification.Domain.Interfaces;

namespace ServiceNotification.Infrastructure.Providers;

public class MockEmailProvider(ILogger<MockEmailProvider> logger) : INotificationProvider
{
    public NotificationChannel Channel => NotificationChannel.Email;

    public Task<bool> SendAsync(Domain.Entities.Notification notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[MOCK EMAIL] To: {Recipient}, Subject: {Subject}, Body: {Body}",
            notification.Recipient, notification.Subject, notification.Body);

        return Task.FromResult(true);
    }
}
