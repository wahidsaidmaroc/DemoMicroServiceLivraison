using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceNotification.Domain.Enums;
using ServiceNotification.Domain.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ServiceNotification.Infrastructure.Providers.WhatsApp;

public class TwilioWhatsAppProvider(
    IConfiguration configuration,
    ILogger<TwilioWhatsAppProvider> logger) : INotificationProvider
{
    public NotificationChannel Channel => NotificationChannel.WhatsApp;

    public async Task<bool> SendAsync(Domain.Entities.Notification notification, CancellationToken cancellationToken = default)
    {
        var accountSid = configuration["NotificationProviders:Twilio:AccountSid"]
            ?? throw new InvalidOperationException("Twilio AccountSid is not configured.");
        var authToken = configuration["NotificationProviders:Twilio:AuthToken"]
            ?? throw new InvalidOperationException("Twilio AuthToken is not configured.");
        var fromNumber = configuration["NotificationProviders:Twilio:WhatsAppFromNumber"]
            ?? throw new InvalidOperationException("Twilio WhatsApp FromNumber is not configured.");

        TwilioClient.Init(accountSid, authToken);

        logger.LogInformation("Sending WhatsApp message via Twilio to {Recipient}", notification.Recipient);

        var message = await MessageResource.CreateAsync(
            to: new PhoneNumber($"whatsapp:{notification.Recipient}"),
            from: new PhoneNumber($"whatsapp:{fromNumber}"),
            body: notification.Body);

        if (message.ErrorCode is null)
        {
            logger.LogInformation("WhatsApp message sent to {Recipient}, SID: {Sid}", notification.Recipient, message.Sid);
            return true;
        }

        logger.LogWarning("Twilio WhatsApp error {ErrorCode}: {ErrorMessage}", message.ErrorCode, message.ErrorMessage);
        return false;
    }
}
