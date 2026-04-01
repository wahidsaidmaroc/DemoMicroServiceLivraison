using Microsoft.Extensions.Logging;
using ServiceNotification.Domain.Interfaces;

namespace ServiceNotification.Application.Services;

public sealed class NotificationService(
    NotificationProviderFactory providerFactory,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    ILogger<NotificationService> logger)
{
    public async Task ProcessAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await notificationRepository.GetByIdAsync(notificationId, cancellationToken);

        if (notification is null)
        {
            logger.LogWarning("Notification {NotificationId} not found", notificationId);
            return;
        }

        var provider = providerFactory.GetProvider(notification.Channel);

        try
        {
            var success = await provider.SendAsync(notification, cancellationToken);

            if (success)
            {
                notification.MarkAsSent(provider.GetType().Name);
                logger.LogInformation(
                    "Notification {NotificationId} sent via {Provider} to {Recipient}",
                    notificationId, provider.GetType().Name, notification.Recipient);
            }
            else
            {
                notification.MarkAsFailed("Provider returned failure");
                logger.LogWarning(
                    "Notification {NotificationId} failed via {Provider}",
                    notificationId, provider.GetType().Name);
            }
        }
        catch (Exception ex)
        {
            notification.MarkAsFailed(ex.Message);
            logger.LogError(ex,
                "Error sending notification {NotificationId} via {Provider}",
                notificationId, provider.GetType().Name);
        }

        notificationRepository.Update(notification);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
