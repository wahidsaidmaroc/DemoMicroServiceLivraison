using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using ServiceNotification.Domain.Enums;
using ServiceNotification.Domain.Interfaces;

namespace ServiceNotification.Infrastructure.Providers.Email;

public class SendGridEmailProvider(
    IConfiguration configuration,
    ILogger<SendGridEmailProvider> logger) : INotificationProvider
{
    public NotificationChannel Channel => NotificationChannel.Email;

    public async Task<bool> SendAsync(Domain.Entities.Notification notification, CancellationToken cancellationToken = default)
    {
        var apiKey = configuration["NotificationProviders:SendGrid:ApiKey"]
            ?? throw new InvalidOperationException("SendGrid API key is not configured.");

        var fromEmail = configuration["NotificationProviders:SendGrid:FromEmail"] ?? "noreply@example.com";
        var fromName = configuration["NotificationProviders:SendGrid:FromName"] ?? "Notification Service";

        var client = new SendGridClient(apiKey);

        var msg = new SendGridMessage
        {
            From = new EmailAddress(fromEmail, fromName),
            Subject = notification.Subject,
            PlainTextContent = notification.Body,
            HtmlContent = notification.Body
        };
        msg.AddTo(new EmailAddress(notification.Recipient));

        logger.LogInformation("Sending email via SendGrid to {Recipient}", notification.Recipient);

        var response = await client.SendEmailAsync(msg, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Email sent successfully to {Recipient}", notification.Recipient);
            return true;
        }

        var body = await response.Body.ReadAsStringAsync(cancellationToken);
        logger.LogWarning("SendGrid returned {StatusCode}: {Body}", response.StatusCode, body);
        return false;
    }
}
