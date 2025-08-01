namespace Eventbus;

internal record EventBusConfig
{
    public record ConsumerConfig
    {
        public required Type Consumer { get; set; }
        
        public required string Queue { get; set; }
        
        public required string[] RoutingKeys { get; set; }
        
        public required bool AutoDelete { get; set; }
    }
    
    public const string DefaultExchangeName = "eventbus";

    public required Uri Uri { get; set; }

    public string ExchangeName { get; set; } = DefaultExchangeName;

    public List<ConsumerConfig> ConsumerHandlers { get; set; } = [];

}