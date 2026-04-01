using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ServiceNotification.Application.Behaviors;
using ServiceNotification.Application.Services;

namespace ServiceNotification.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<NotificationProviderFactory>();
        services.AddScoped<NotificationService>();

        return services;
    }
}
