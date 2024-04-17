using MediatR;
using Npgsql;
using Ozon.Route256.Practice.OrdersService.Application.Commands;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Repositories;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;
using System.Text.Json;

namespace Ozon.Route256.Practice.OrdersService.DataAccess.Orders;

public class OrdersManager : IOrdersManager
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IRegionDatabase _regionDatabase;
    private readonly IMediator _mediator;
    public OrdersManager(IOrdersRepository ordersRepository, IRegionDatabase regionDatabase, IMediator mediator)
    {
        _ordersRepository = ordersRepository;
        _regionDatabase = regionDatabase;
        _mediator = mediator;
    }
    public async Task CreateOrderAsync(PreOrderDto order, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var region = await _regionDatabase.GetRegionEntityByNameAsync(order.Address.Region, token);
        await _mediator.Send(new CreateOrderByPreOrderCommand(order), token);
    }

    public async Task<OrderDao> GetOrderByIdAsync(long id, CancellationToken token = default)
    {
        var order = await _ordersRepository.GetOrderByID(id, token);
        if (order != null)
            return await FromOrderDal(order);
        else
            throw new NotFoundException($"Заказ с номером {id} не найден");
    }

    public async Task<OrderDao[]> GetOrdersByCutomerAsync(long idCustomer, DateTime dateStart, CancellationToken token = default)
    {
        var orders = await _ordersRepository.GetOrdersByCustomerId(idCustomer, dateStart, token);
        List<OrderDao> result = new List<OrderDao>();
        foreach (var order in orders)
            result.Add(await FromOrderDal(order));
        return result.ToArray();
    }

    public async Task<OrderDao[]> GetOrdersByRegionAsync(List<string> regionList, OrderSourceEnum source, CancellationToken token = default)
    {
        var regionsId = await _regionDatabase.GetRegionsEntityByNameAsync(regionList.ToArray(), token);
        var orders = await _ordersRepository.GetOrdersByRegion(regionsId.Select(x => x.Id).ToArray(), source,token);
        List<OrderDao> result = new List<OrderDao>();
        foreach (var order in orders)
            result.Add(await FromOrderDal(order));
        return result.ToArray();
    }

    public async Task<RegionStatisticDto[]> GetRegionsStatisticAsync(List<string> regionList, DateTime dateStart, CancellationToken token = default)
    {
        List<RegionStatisticDto> regions = new List<RegionStatisticDto>();
        var regionsId = await _regionDatabase.GetRegionsEntityByNameAsync(regionList.ToArray(), token);
        var regionsStatistic = await _ordersRepository.GetRegionStatistic(regionsId.Select(x=>x.Id).ToArray(), dateStart, token);
        foreach(var rs in regionsStatistic)
            regions.Add(await FromStatisticDalAsync(rs));
        return regions.ToArray();
    }
    public async Task<bool> SetOrderStateAsync(long id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        try
        {
            await _ordersRepository.SetStatusById(id, state, timeUpdate, token);
            return true;
        }
        catch (PostgresException)
        {
            return false;
        }
    }
    private async Task<OrderDao> FromOrderDal(OrderDal order)
    {
        return new OrderDao()
        {
            Id = order.id,
            CustomerId = order.customer_id,
            Source = order.source,
            CountGoods = order.countGoods,
            Address = JsonSerializer.Deserialize<Etities.AddressDto>(order.addressJson),
            Goods = new List<ProductDto>(),
            Region = (await _regionDatabase.GetRegionEntityByIdAsync(order.regionId)).Name,
            State = order.state,
            TotalWeigth = order.totalWeigth,
            TimeCreate = order.timeCreate,
            TimeUpdate = order.timeUpdate,
            TotalPrice = order.totalPrice
        };
    }
    private async Task<RegionStatisticDto> FromStatisticDalAsync(RegionStatisticDal regionStatisticDal) =>
        new RegionStatisticDto(
            (await _regionDatabase.GetRegionEntityByIdAsync(regionStatisticDal.regionId)).Name,
            regionStatisticDal.TotalCountOrders,
            regionStatisticDal.TotalSumOrders,
            regionStatisticDal.TotalWigthOrders,
            regionStatisticDal.TotalCustomers);


}