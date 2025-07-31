using Microsoft.Extensions.DependencyInjection;

namespace Eventbus;

public class EventBusConfigBuilder
{
    private readonly EventBusConfig config;
    private readonly IServiceCollection services;
    
    public EventBusConfigBuilder(IServiceCollection services, Uri uri, string? exchangeName)
    {
        this.services = services;

        config = new EventBusConfig()
        {
            Uri = uri
        };
        if (!string.IsNullOrWhiteSpace(exchangeName))
        {
            config.ExchangeName = exchangeName;
        }
    }

    internal static EventBusConfigBuilder AddEventBus(IServiceCollection services, Uri uri, string? exchangeName)
    {
        var builder = new EventBusConfigBuilder(services, uri, exchangeName);
        
        services.AddSingleton(builder.config);

        services.AddSingleton<EventBusChannelPool>();
        // 
        // services.AddEventbus (Send)

        return builder;
    }
}