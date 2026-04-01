using Microsoft.Extensions.Logging;
using ServiceNotification.Domain.Enums;
using ServiceNotification.Domain.Interfaces;

namespace ServiceNotification.Infrastructure.Providers;

public class MockSmsProvider(ILogger<MockSmsProvider> logger) : INotificationProvider
{
    public NotificationChannel Channel => NotificationChannel.SMS;

    public Task<bool> SendAsync(Domain.Entities.Notification notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[MOCK SMS] To: {Recipient}, Body: {Body}",
            notification.Recipient, notification.Body);

        return Task.FromResult(true);
    }
}
