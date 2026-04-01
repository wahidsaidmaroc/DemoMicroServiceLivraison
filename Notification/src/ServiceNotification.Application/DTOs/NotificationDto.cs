using ServiceNotification.Domain.Enums;

namespace ServiceNotification.Application.DTOs;

public sealed record NotificationDto(
    Guid Id,
    NotificationChannel Channel,
    string Recipient,
    string? Subject,
    string Body,
    string Status,
    string? ProviderName,
    string? ErrorMessage,
    DateTimeOffset CreatedAt,
    DateTimeOffset? SentAt,
    int RetryCount);



