using Microsoft.Extensions.DependencyInjection;

namespace Eventbus;

public class EventBusConfigBuilder
{
    private readonly EventBusConfig config;
    private readonly IServiceCollection services;
    
    private EventBusConfigBuilder(IServiceCollection services, Uri uri, string? exchangeName)
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
        services.AddScoped<IEventBus, EventBus>();
        services.AddHostedService<EventBusHostedService>();
        
        return builder;
    }

    public EventBusConfigBuilder AddConsumer<T>(string queue, params string[] routingKeys)
    where T : class, IAsyncConsumer
    {
        services.AddScoped<T>();
        var consumer = new EventBusConfig.ConsumerConfig()
        {
            Consumer = typeof(T),
            Queue = queue,
            RoutingKeys = routingKeys,
            AutoDelete = false
        };
        config.ConsumerHandlers.Add(consumer);

        return this;
    }

    public EventBusConfigBuilder AddReadyHandler<T>()
    where T : class, IAsyncReadyHandler
    {
        services.AddScoped<IAsyncReadyHandler, T>();

        return this;
    }
}