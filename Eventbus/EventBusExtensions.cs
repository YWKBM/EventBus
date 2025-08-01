using Microsoft.Extensions.DependencyInjection;

namespace Eventbus;

public static class EventBusExtensions
{
    public static EventBusConfigBuilder AddEventbus(this IServiceCollection services, Uri uri, string? exchangeName = null)
    {
        return EventBusConfigBuilder.AddEventBus(services, uri, exchangeName);
    }
}