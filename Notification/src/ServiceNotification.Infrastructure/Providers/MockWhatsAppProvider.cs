using Microsoft.Extensions.Logging;
using ServiceNotification.Domain.Enums;
using ServiceNotification.Domain.Interfaces;

namespace ServiceNotification.Infrastructure.Providers;

public class MockWhatsAppProvider(ILogger<MockWhatsAppProvider> logger) : INotificationProvider
{
    public NotificationChannel Channel => NotificationChannel.WhatsApp;

    public Task<bool> SendAsync(Domain.Entities.Notification notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "[MOCK WHATSAPP] To: {Recipient}, Body: {Body}",
            notification.Recipient, notification.Body);

        return Task.FromResult(true);
    }
}
