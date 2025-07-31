using RabbitMQ.Client;

namespace Eventbus;

internal class EventBusChannelPool(
    EventBusConfig config
    ) : IDisposable
{
    private IConnection connection => initialize(config).GetAwaiter().GetResult();
    
    private static async Task<IConnection> initialize(EventBusConfig config)
    {
        var connectionFactory = new ConnectionFactory()
        {
            Uri = config.Uri,
            AutomaticRecoveryEnabled = true
        };
        
        var connection = await connectionFactory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(config.ExchangeName, ExchangeType.Topic, true, false);
        
        return connection;
    }
    
    public Task<IChannel> CreateChanelAsync() => connection.CreateChannelAsync();

    public void Dispose()
    {
        connection.Dispose();
        GC.SuppressFinalize(this);
    }
}