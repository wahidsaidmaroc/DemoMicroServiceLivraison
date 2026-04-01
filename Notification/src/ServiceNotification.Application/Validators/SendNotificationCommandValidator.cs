using FluentValidation;
using ServiceNotification.Application.Commands;
using ServiceNotification.Domain.Enums;

namespace ServiceNotification.Application.Validators;

public sealed class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Le corps du message est obligatoire.");

        RuleFor(x => x.Recipient)
            .NotEmpty().WithMessage("Le destinataire est obligatoire.");

        RuleFor(x => x.Channel)
            .IsInEnum().WithMessage("Le canal de notification est invalide.");



        // Email-specific rules
        When(x => x.Channel == NotificationChannel.Email, () =>
        {
            RuleFor(x => x.Recipient)
                .EmailAddress().WithMessage("Le format de l'adresse email est invalide.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Le sujet est obligatoire pour un email.");
        });

        // SMS-specific rules
        When(x => x.Channel == NotificationChannel.SMS, () =>
        {
            RuleFor(x => x.Recipient)
                .Matches(@"^\+[1-9]\d{6,14}$")
                .WithMessage("Le numéro de téléphone doit être au format international (ex: +33612345678).");
        });

        // WhatsApp-specific rules
        When(x => x.Channel == NotificationChannel.WhatsApp, () =>
        {
            RuleFor(x => x.Recipient)
                .Matches(@"^\+[1-9]\d{6,14}$")
                .WithMessage("Le numéro WhatsApp doit être au format international (ex: +33612345678).");
        });
    }
}
