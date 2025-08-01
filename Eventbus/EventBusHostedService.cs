using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Eventbus;

internal class EventBusHostedService(
    IServiceScopeFactory scopeFactory,
    EventBusConfig config,
    EventBusChannelPool channelPool
    ) : BackgroundService, IDisposable
{
    private readonly List<IChannel> channels = [];
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var consumer in config.ConsumerHandlers)
        {
            await registerConsumer(consumer);
        }
        
        using var scope = scopeFactory.CreateScope();
        var readyHandlers = scope.ServiceProvider.GetServices<IAsyncReadyHandler>();

        foreach (var readyHandler in readyHandlers)
        {
            await readyHandler.Handle();
        }
    }

    private async Task registerConsumer(EventBusConfig.ConsumerConfig consumerConfig)
    {
        var channel = await channelPool.CreateChanelAsync();
        
        await channel.QueueDeclareAsync(consumerConfig.Queue, durable: true, autoDelete: consumerConfig.AutoDelete, exclusive: false);

        foreach (var routingKey in consumerConfig.RoutingKeys)
        {
            await channel.QueueBindAsync(consumerConfig.Queue, config.ExchangeName, routingKey);
        }

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var scope = scopeFactory.CreateScope();
                var consumer = scope.ServiceProvider.GetRequiredService(consumerConfig.Consumer) as IAsyncConsumer;
                await consumer!.Handle(ea);

                await channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                await channel.BasicRejectAsync(ea.DeliveryTag, false);
            }
        };

        await channel.BasicConsumeAsync(consumerConfig.Queue, autoAck: false, consumer);
        
        channels.Add(channel);
    }
    
    
    public new void Dispose()
    {
        foreach (var channel in channels)
        {
            channel.Dispose();
        }

        base.Dispose();
        GC.SuppressFinalize(this);
    }
}