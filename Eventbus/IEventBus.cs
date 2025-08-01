namespace Eventbus;

public interface IEventBus
{
    Task Send(string? queueName, string routingKey, ReadOnlyMemory<byte> message);
}