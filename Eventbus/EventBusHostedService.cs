using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Eventbus;

internal class EventBusHostedService(
    IServiceScopeFactory scopeFactory,
    EventBusConfig config,
    EventBusChannelPool channelPool
    ) : BackgroundService
{
    private readonly List<IChannel> channels = [];

    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // TODO: start consumers
        // TODO: start readyHandlers
        
        
        throw new NotImplementedException();
    }
}