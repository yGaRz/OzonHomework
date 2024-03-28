﻿
using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;

public abstract class ConsumerBackgroundService<TKey, TValue> : BackgroundService
{
    //TODO: вынести название топика в переменные окружения
    private const string TopicName = "pre_orders";
    private readonly IKafkaDataProvider<TKey, TValue> _dataProvider;
    private readonly ILogger<ConsumerBackgroundService<TKey, TValue>> _logger;
    protected readonly IServiceScope _scope;
    public ConsumerBackgroundService(IServiceProvider serviceProvider,
                                IKafkaDataProvider<TKey,TValue> kafkaDataProvider, 
                                ILogger<ConsumerBackgroundService<TKey, TValue>> logger)
    {
        _dataProvider = kafkaDataProvider;
        _logger = logger;
        _scope = serviceProvider.CreateScope();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        if (stoppingToken.IsCancellationRequested)
            return;

        _dataProvider.Consumer.Subscribe(TopicName);
        _logger.LogInformation("Start consumer topic {Topic}", TopicName);

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

            if (message is null)
            {
                await Task.Delay(100, cancellationToken);
                return;
            }

            //TODO: делаем обрабоку данных
            await HandleAsync(message, cancellationToken);
            //_logger.LogInformation($"Message: {message}");  
            _dataProvider.Consumer.Commit();
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
