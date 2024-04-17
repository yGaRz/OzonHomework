
using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;

public abstract class ConsumerBackgroundService<TKey, TValue> : BackgroundService
{
    private string TopicName;
    private readonly IKafkaConsumer<TKey, TValue> _dataProvider;
    private readonly ILogger<ConsumerBackgroundService<TKey, TValue>> _logger;
    protected readonly IServiceScope _scope;
    public ConsumerBackgroundService(IServiceProvider serviceProvider,
                                IKafkaConsumer<TKey,TValue> kafkaDataProvider, 
                                ILogger<ConsumerBackgroundService<TKey, TValue>> logger,
                                string topicName)
    {
        _dataProvider = kafkaDataProvider;
        _logger = logger;
        _scope = serviceProvider.CreateScope();
        TopicName= topicName;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        if (stoppingToken.IsCancellationRequested)
            return;

        _dataProvider.Consumer.Subscribe(TopicName);
        _logger.LogInformation("Start consumer topic {Topic} ", TopicName);

        while (!stoppingToken.IsCancellationRequested)
        {
            await ConsumeAsync(stoppingToken);
        }

        _dataProvider.Consumer.Unsubscribe();
        _logger.LogInformation("Stop consumer topic {Topic}", TopicName);
    }
    private async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        ConsumeResult<TKey, TValue>? message = null;
        try
        {
            message = _dataProvider.Consumer.Consume(TimeSpan.FromMilliseconds(100));
            if (message is not null)
            {
                await HandleAsync(message, cancellationToken);
                _dataProvider.Consumer.Commit();
            }
            else
                await Task.Delay(100, cancellationToken);
            return;
        }
        catch (Exception exc)
        {
            var key = message is not null ? message.Message.Key!.ToString() : "No key";
            var value = message is not null ? message.Message.Value!.ToString() : "No value";
            _logger.LogError(exc, "Error process message Key:{Key} Value:{Value}", key, value);
        }
    }
    protected abstract Task HandleAsync(ConsumeResult<TKey, TValue> message, CancellationToken cancellationToken);
    public override void Dispose()
    {
        _scope.Dispose();
        base.Dispose();
    }

}
