using ServiceNotification.Domain.Enums;

namespace ServiceNotification.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public string Recipient { get; private set; } = string.Empty;
    public string? Subject { get; private set; }
    public string Body { get; private set; } = string.Empty;
    public NotificationStatus Status { get; private set; }
    public string? ProviderName { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }
    public int RetryCount { get; private set; }

    private Notification() { } // EF Core

    public static Notification Create(NotificationChannel channel, string recipient, string body, string? subject = null)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            Channel = channel,
            Recipient = recipient,
            Body = body,
            Subject = subject,
            Status = NotificationStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
            RetryCount = 0
        };
    }

    public void MarkAsSent(string providerName)
    {
        Status = NotificationStatus.Sent;
        ProviderName = providerName;
        SentAt = DateTimeOffset.UtcNow;
        ErrorMessage = null;
    }

    public void MarkAsFailed(string error)
    {
        Status = NotificationStatus.Failed;
        ErrorMessage = error;
    }

    public void IncrementRetry()
    {
        RetryCount++;
        Status = NotificationStatus.Retrying;
    }
}
