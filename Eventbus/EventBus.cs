using RabbitMQ.Client;

namespace Eventbus;

internal class EventBus(
    EventBusChannelPool channelPool,
    EventBusConfig config
) : IEventBus, IDisposable
{
    private readonly string exchangeName;
    private IChannel? channel;

    private async Task<IChannel> getChannel()
    {
        channel ??= await channelPool.CreateChanelAsync();
        return channel;
    }


    public async Task Send(string? queue, string routingKey, ReadOnlyMemory<byte> message)
    {
        var properties = new BasicProperties()
        {
            Persistent = true,
            Type = routingKey,
        };
        
        var channel = await getChannel();

        if (string.IsNullOrEmpty(queue))
        {
            await channel.BasicPublishAsync(config.ExchangeName, routingKey, false, properties, message);
        }
        else
        {
            await channel.BasicPublishAsync(queue, routingKey, false, properties, message);
        }
    }

    public void Dispose()
    {
        channelPool.Dispose();
    }
}