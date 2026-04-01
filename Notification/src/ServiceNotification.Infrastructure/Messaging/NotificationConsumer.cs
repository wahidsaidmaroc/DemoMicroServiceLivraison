using MassTransit;
using Microsoft.Extensions.Logging;
using ServiceNotification.Application.Services;

namespace ServiceNotification.Infrastructure.Messaging;

public sealed class NotificationConsumer(
    NotificationService notificationService,
    ILogger<NotificationConsumer> logger) : IConsumer<SendNotificationMessage>
{
    public async Task Consume(ConsumeContext<SendNotificationMessage> context)
    {
        var notificationId = context.Message.NotificationId;

        logger.LogInformation("Consuming notification message for {NotificationId}", notificationId);

        await notificationService.ProcessAsync(notificationId, context.CancellationToken);

        logger.LogInformation("Notification {NotificationId} processed", notificationId);
    }
}
