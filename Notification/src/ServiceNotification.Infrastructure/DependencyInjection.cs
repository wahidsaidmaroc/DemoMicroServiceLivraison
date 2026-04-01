using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceNotification.Domain.Interfaces;
using ServiceNotification.Infrastructure.Messaging;
using ServiceNotification.Infrastructure.Persistence;
using ServiceNotification.Infrastructure.Persistence.Repositories;
using ServiceNotification.Infrastructure.Providers;
using ServiceNotification.Infrastructure.Providers.Email;
using ServiceNotification.Infrastructure.Providers.Sms;
using ServiceNotification.Infrastructure.Providers.WhatsApp;

namespace ServiceNotification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDatabase(configuration);

        // Repositories & UoW
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Notification providers
        RegisterNotificationProviders(services, configuration);

        // MassTransit + RabbitMQ
        RegisterMassTransit(services, configuration);

        // Outbox background processor
        services.AddHostedService<OutboxProcessor>();

        return services;
    }

    private static void RegisterNotificationProviders(IServiceCollection services, IConfiguration configuration)
    {
        var useMock = configuration.GetValue<bool>("NotificationProviders:UseMock");

        if (useMock)
        {
            services.AddScoped<INotificationProvider, MockEmailProvider>();
            services.AddScoped<INotificationProvider, MockSmsProvider>();
            services.AddScoped<INotificationProvider, MockWhatsAppProvider>();
        }
        else
        {
            services.AddScoped<INotificationProvider, SendGridEmailProvider>();
            services.AddScoped<INotificationProvider, TwilioSmsProvider>();
            services.AddScoped<INotificationProvider, TwilioWhatsAppProvider>();
        }
    }

    private static void RegisterMassTransit(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(bus =>
        {
            bus.AddConsumer<NotificationConsumer>();

            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(
                    configuration["RabbitMq:Host"] ?? "localhost",
                    configuration["RabbitMq:VirtualHost"] ?? "/",
                    h =>
                    {
                        h.Username(configuration["RabbitMq:Username"] ?? "guest");
                        h.Password(configuration["RabbitMq:Password"] ?? "guest");
                    });

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
