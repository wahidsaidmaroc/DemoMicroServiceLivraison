using MediatR;
using ServiceNotification.Application.DTOs;
using ServiceNotification.Domain.Interfaces;

namespace ServiceNotification.Application.Queries;

public sealed class GetNotificationQueryHandler(
    INotificationRepository notificationRepository) : IRequestHandler<GetNotificationQuery, NotificationDto?>
{
    public async Task<NotificationDto?> Handle(GetNotificationQuery request, CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (notification is null)
            return null;

        return new NotificationDto(
            notification.Id,
            notification.Channel,
            notification.Recipient,
            notification.Subject,
            notification.Body,
            notification.Status.ToString(),
            notification.ProviderName,
            notification.ErrorMessage,
            notification.CreatedAt,
            notification.SentAt,
            notification.RetryCount);
    }
}
