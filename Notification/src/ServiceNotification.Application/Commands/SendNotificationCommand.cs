using MediatR;
using ServiceNotification.Domain.Enums;

namespace ServiceNotification.Application.Commands;

public sealed record SendNotificationCommand(
    NotificationChannel Channel,
    string Recipient,
    string? Subject,
    string Body) : IRequest<Guid>;
