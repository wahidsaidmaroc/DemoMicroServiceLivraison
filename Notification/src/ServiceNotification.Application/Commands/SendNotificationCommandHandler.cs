using System.Text.Json;
using MediatR;
using ServiceNotification.Domain.Entities;
using ServiceNotification.Domain.Interfaces;

namespace ServiceNotification.Application.Commands;

public sealed class SendNotificationCommandHandler(
    INotificationRepository notificationRepository,
    IOutboxRepository outboxRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SendNotificationCommand, Guid>
{
    public async Task<Guid> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = Domain.Entities.Notification.Create(
            request.Channel,
            request.Recipient,
            request.Body,
            request.Subject);

        await notificationRepository.AddAsync(notification, cancellationToken);

        var outboxPayload = JsonSerializer.Serialize(new { NotificationId = notification.Id });
        var outboxMessage = OutboxMessage.Create(
            nameof(SendNotificationCommand),
            outboxPayload);

        await outboxRepository.AddAsync(outboxMessage, cancellationToken);



        await unitOfWork.SaveChangesAsync(cancellationToken);

        return notification.Id;
    }
}
