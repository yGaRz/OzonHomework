﻿using Bogus;
using Ozon.Route256.Practice.OrdersService.Application;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Kafka.ProducerNewOrder.Handlers;
using System.Diagnostics;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer;

internal sealed class AddOrderHandler : IAddOrderHandler
{
    private readonly ILogger<AddOrderHandler> _logger;
    private readonly IOrderServiceAdapter _orderServiceAdapter;
    private readonly IKafkaAdapter _kafkaAdapter;
    public AddOrderHandler(ILogger<AddOrderHandler> logger, IOrderServiceAdapter orderServiceAdapter, IKafkaAdapter kafkaAdapter)
    {
        _orderServiceAdapter = orderServiceAdapter;
        _logger = logger;
        _kafkaAdapter = kafkaAdapter;
    }
    public async Task<bool> Handle(long id, string message, DateTime timeCreate, CancellationToken token)
    {
        try
        {
            //Сам класс нужен тут, чтобы мы могли изменить в теле запроса регион и customerId, для последующей логики
            var order = new PreOrderDto(id, message, timeCreate);
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            //Это необходимо чтобы хоть как то работала логика заказов с покупателями и регионами
            order.CustomerId = order.CustomerId % 10 + 1;
            Faker faker = new Faker();
            var region = await  _orderServiceAdapter.GetRegion(faker.Random.Int(1, 3),token);
            order.Address.Region = region.Name;


            await _orderServiceAdapter.CreateOrder(order, token);
            if (GetDistance(order.Address.Latitude, order.Address.Longitude, region.Latitude, region.Longitude) < 5000)
            {
                await _kafkaAdapter.ProduceAsync(new[] { order.Id }, token);
                _logger.LogInformation("Заказ {@order} отправлен", order);
            }
            else
                _logger.LogInformation($"Заказ {order.Id} не будет отправлен из-за превышения расстояния до склада");
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"При обработке заказа {id} возникла ошибка {ex.Message}");
            return false;
        }
        return true;
    }

    private static double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371d;
        var dLat = Deg2Rad(lat2 - lat1);
        var dLon = Deg2Rad(lon2 - lon1);
        var a =
            Math.Sin(dLat / 2d) * Math.Sin(dLat / 2d) +
            Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
            Math.Sin(dLon / 2d) * Math.Sin(dLon / 2d);
        var c = 2d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1d - a));
        var d = R * c;
        return d;
    }
    private static double Deg2Rad(double deg)
    {
        return deg * (Math.PI / 180d);
    }
}
