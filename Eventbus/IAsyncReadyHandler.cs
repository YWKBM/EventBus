namespace Eventbus;

public interface IAsyncReadyHandler
{
    Task Handle();
}