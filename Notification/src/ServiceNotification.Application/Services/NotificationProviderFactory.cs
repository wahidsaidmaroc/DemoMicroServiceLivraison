using ServiceNotification.Domain.Enums;
using ServiceNotification.Domain.Interfaces;

namespace ServiceNotification.Application.Services;

public sealed class NotificationProviderFactory(IEnumerable<INotificationProvider> providers)
{
    private readonly Dictionary<NotificationChannel, INotificationProvider> _providers =
        providers.ToDictionary(p => p.Channel);

    public INotificationProvider GetProvider(NotificationChannel channel)
    {
        if (!_providers.TryGetValue(channel, out var provider))
            throw new InvalidOperationException($"No notification provider registered for channel '{channel}'.");

        return provider;
    }
}
