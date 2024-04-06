﻿using Npgsql;
using Ozon.Route256.Practice.OrdersService.DAL.Models;
using Ozon.Route256.Practice.OrdersService.DAL.Repositories;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Text.Json;

namespace Ozon.Route256.Practice.OrdersService.DataAccess.Orders;

public class OrdersDatabase : IOrdersRepository
{
    private readonly OrdersRepositoryPg _ordersRepositoryPg;
    private readonly IRegionDatabase _regionDatabase;
    public OrdersDatabase(OrdersRepositoryPg ordersRepositoryPg, IRegionDatabase regionDatabase)
    {
        _ordersRepositoryPg = ordersRepositoryPg;
        _regionDatabase = regionDatabase;
    }
    public async Task CreateOrderAsync(OrderEntity order, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        try
        {
            var region = await _regionDatabase.GetRegionEntityByNameAsync(order.Region, token);
            var orderDal = ToInsertDal(order, region.Id);
            await _ordersRepositoryPg.Create(orderDal, token);
        }
        catch (PostgresException)
        {
            throw new ArgumentException($"Order with id={order.Id} is already exists");
        }
    }

    public async Task<OrderEntity> GetOrderByIdAsync(long id, CancellationToken token = default)
    {
        var order = await _ordersRepositoryPg.GetOrderByID(id, token);
        if (order != null)
            return await FromOrderDal(order);
        else
            throw new NotFoundException($"Заказ с номером {id} не найден");
    }

    public async Task<OrderEntity[]> GetOrdersByCutomerAsync(long idCustomer, DateTime dateStart, CancellationToken token = default)
    {
        var orders = await _ordersRepositoryPg.GetOrdersByCustomerId(idCustomer, dateStart, token);
        List<OrderEntity> result = new List<OrderEntity>();
        foreach (var order in orders)
            result.Add(await FromOrderDal(order));
        return result.ToArray();
    }

    public async Task<OrderEntity[]> GetOrdersByRegionAsync(List<string> regionList, OrderSourceEnum source, CancellationToken token = default)
    {
        var regionsId = await _regionDatabase.GetRegionsEntityByNameAsync(regionList.ToArray(), token);
        var orders = await _ordersRepositoryPg.GetOrdersByRegion(regionsId.Select(x => x.Id).ToArray(), source,token);
        List<OrderEntity> result = new List<OrderEntity>();
        foreach (var order in orders)
            result.Add(await FromOrderDal(order));
        return result.ToArray();
    }

    public async Task<RegionStatisticEntity[]> GetRegionsStatisticAsync(List<string> regionList, DateTime dateStart, CancellationToken token = default)
    {
        List<RegionStatisticEntity> regions = new List<RegionStatisticEntity>();
        var regionsId = await _regionDatabase.GetRegionsEntityByNameAsync(regionList.ToArray(), token);
        var regionsStatistic = await _ordersRepositoryPg.GetRegionStatistic(regionsId.Select(x=>x.Id).ToArray(), dateStart, token);
        foreach(var rs in regionsStatistic)
            regions.Add(await FromStatisticDalAsync(rs));
        return regions.ToArray();
    }
    public async Task<bool> SetOrderStateAsync(long id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        try
        {
            await _ordersRepositoryPg.SetStatusById(id, state, timeUpdate, token);
            return true;
        }
        catch (PostgresException)
        {
            return false;
        }
    }
    private OrderDal ToInsertDal(OrderEntity order, int regionId)
    {
        return new OrderDal(order.Id,
            order.CustomerId,
            order.Source,
            order.State,
            order.TimeCreate,
            order.TimeUpdate,
            regionId,
            order.CountGoods,
            order.TotalWeigth,
            order.TotalPrice,
            JsonSerializer.Serialize(order.Address));
    }
    private async Task<OrderEntity> FromOrderDal(OrderDal order)
    {
        return new OrderEntity()
        {
            Id = order.id,
            CustomerId = order.customer_id,
            Source = order.source,
            CountGoods = order.countGoods,
            Address = JsonSerializer.Deserialize<AddressEntity>(order.addressJson),
            Goods = new List<ProductEntity>(),
            Region = (await _regionDatabase.GetRegionEntityByIdAsync(order.regioId)).Name,
            State = order.state,
            TotalWeigth = order.totalWeigth,
            TimeCreate = order.timeCreate,
            TimeUpdate = order.timeUpdate,
            TotalPrice = order.totalPrice
        };
    }
    private async Task<RegionStatisticEntity> FromStatisticDalAsync(RegionStatisticDal regionStatisticDal) =>
        new RegionStatisticEntity(
            (await _regionDatabase.GetRegionEntityByIdAsync(regionStatisticDal.regionId)).Name,
            regionStatisticDal.TotalCountOrders,
            regionStatisticDal.TotalSumOrders,
            regionStatisticDal.TotalWigthOrders,
            regionStatisticDal.TotalCustomers);


}