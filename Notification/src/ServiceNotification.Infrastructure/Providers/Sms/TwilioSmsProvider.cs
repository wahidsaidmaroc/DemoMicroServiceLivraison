using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceNotification.Domain.Enums;
using ServiceNotification.Domain.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ServiceNotification.Infrastructure.Providers.Sms;

public class TwilioSmsProvider(
    IConfiguration configuration,
    ILogger<TwilioSmsProvider> logger) : INotificationProvider
{
    public NotificationChannel Channel => NotificationChannel.SMS;

    public async Task<bool> SendAsync(Domain.Entities.Notification notification, CancellationToken cancellationToken = default)
    {
        var accountSid = configuration["NotificationProviders:Twilio:AccountSid"]
            ?? throw new InvalidOperationException("Twilio AccountSid is not configured.");
        var authToken = configuration["NotificationProviders:Twilio:AuthToken"]
            ?? throw new InvalidOperationException("Twilio AuthToken is not configured.");
        var fromNumber = configuration["NotificationProviders:Twilio:FromNumber"]
            ?? throw new InvalidOperationException("Twilio FromNumber is not configured.");

        TwilioClient.Init(accountSid, authToken);

        logger.LogInformation("Sending SMS via Twilio to {Recipient}", notification.Recipient);

        var message = await MessageResource.CreateAsync(
            to: new PhoneNumber(notification.Recipient),
            from: new PhoneNumber(fromNumber),
            body: notification.Body);

        if (message.ErrorCode is null)
        {
            logger.LogInformation("SMS sent successfully to {Recipient}, SID: {Sid}", notification.Recipient, message.Sid);
            return true;
        }

        logger.LogWarning("Twilio SMS error {ErrorCode}: {ErrorMessage}", message.ErrorCode, message.ErrorMessage);
        return false;
    }
}
