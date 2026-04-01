using MediatR;
using ServiceNotification.Application.DTOs;

namespace ServiceNotification.Application.Queries;

public sealed record GetNotificationQuery(Guid Id) : IRequest<NotificationDto?>;
