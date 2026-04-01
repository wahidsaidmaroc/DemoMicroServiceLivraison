using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceNotification.Domain.Interfaces;

namespace ServiceNotification.Infrastructure.Messaging;

public sealed class OutboxProcessor(
    IServiceScopeFactory scopeFactory,
    IBus bus,
    ILogger<OutboxProcessor> logger) : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 20;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Outbox processor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Error processing outbox messages");
            }

            await Task.Delay(PollingInterval, stoppingToken);
        }

        logger.LogInformation("Outbox processor stopped");
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var messages = await outboxRepository.GetUnprocessedAsync(BatchSize, cancellationToken);

        if (messages.Count == 0)
            return;

        logger.LogInformation("Processing {Count} outbox messages", messages.Count);

        foreach (var message in messages)
        {
            try
            {
                var payload = JsonSerializer.Deserialize<JsonElement>(message.Payload);
                var notificationId = payload.GetProperty("NotificationId").GetGuid();

                await bus.Publish(new SendNotificationMessage(notificationId), cancellationToken);

                outboxRepository.MarkAsProcessed(message);

                logger.LogDebug("Outbox message {MessageId} published for notification {NotificationId}",
                    message.Id, notificationId);
            }
            catch (Exception ex)
            {
                message.MarkAsFailed(ex.Message);
                logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
