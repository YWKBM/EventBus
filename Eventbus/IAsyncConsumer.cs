using RabbitMQ.Client.Events;

namespace Eventbus;

public interface IAsyncConsumer
{
    Task Handle(BasicDeliverEventArgs args);
}