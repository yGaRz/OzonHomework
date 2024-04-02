﻿
using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;

public abstract class ConsumerBackgroundService<TKey, TValue> : BackgroundService
{
    

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

        string TopicName = "pre_orders";
        _dataProvider.ConsumerPreOrder.Subscribe(TopicName);
        _logger.LogInformation("Start consumer topic {Topic} ", TopicName);

        TopicName = "orders_events";
        _dataProvider.ConsumerOrderEvent.Subscribe(TopicName);
        _logger.LogInformation("Start consumer topic {Topic} ", TopicName);

        while (!stoppingToken.IsCancellationRequested)
        {
            await ConsumePreOrderAsync(stoppingToken);
            await ConsumeOrderEventAsync(stoppingToken);
        }

        _dataProvider.ConsumerPreOrder.Unsubscribe();
        _dataProvider.ConsumerOrderEvent.Unsubscribe();
        _logger.LogInformation("Stop consumer topic {Topic}", TopicName);
    }
    private async Task ConsumePreOrderAsync(CancellationToken cancellationToken)
    {
        ConsumeResult<TKey, TValue>? message = null;
        try
        {
            message = _dataProvider.ConsumerPreOrder.Consume(TimeSpan.FromMilliseconds(100));
            if (message is not null)
            {
                await HandleAsync(message, cancellationToken);
                //_logger.LogInformation($"Message: {message}");  
                _dataProvider.ConsumerPreOrder.Commit();
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
    private async Task ConsumeOrderEventAsync(CancellationToken cancellationToken)
    {
        ConsumeResult<TKey, TValue>? message = null;
        try
        {
            message = _dataProvider.ConsumerOrderEvent.Consume(TimeSpan.FromMilliseconds(100));
            if (message is not null)
            {
                await HandleAsync(message, cancellationToken);
                //_logger.LogInformation($"Message: {message}");
                _dataProvider.ConsumerOrderEvent.Commit();
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
