using MediatR;
using Npgsql;
using Ozon.Route256.Practice.OrdersService.Application.Commands;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models.Enums;
using Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.Orders.Repository;
using Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.RegionManager;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.Orders;
internal class OrdersManager : IOrdersManager
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
    public async Task CreateOrderAsync(OrderDal order, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var region = await _regionDatabase.GetRegionEntityByIdAsync(order.regionId, token);
        await _ordersRepository.Create(order, token);
    }
    public async Task<OrderDal> GetOrderByIdAsync(long id, CancellationToken token = default)
    {
        var order = await _ordersRepository.GetOrderByID(id, token);
        if (order != null)
            return order;
        else
            throw new NotFoundException($"Заказ с номером {id} не найден");
    }
    public async Task<OrderDal[]> GetOrdersByCutomerAsync(long idCustomer, DateTime dateStart, CancellationToken token = default)
    {
        return await _ordersRepository.GetOrdersByCustomerId(idCustomer, dateStart, token);
    }
    public async Task<OrderDal[]> GetOrdersByRegionAsync(List<string> regionList, OrderSourceEnum source, CancellationToken token = default)
    {
        var regionsId = await _regionDatabase.GetRegionsEntityByNameAsync(regionList.ToArray(), token);
        var orders = await _ordersRepository.GetOrdersByRegion(regionsId.Select(x => x.Id).ToArray(), source, token);
        return orders.ToArray();
    }
    public async Task<RegionStatisticDto[]> GetRegionsStatisticAsync(List<string> regionList, DateTime dateStart, CancellationToken token = default)
    {

        var regionsId = await _regionDatabase.GetRegionsEntityByNameAsync(regionList.ToArray(), token);
        var regionsStatistic = await _ordersRepository.GetRegionStatistic(regionsId.Select(x => x.Id).ToArray(), dateStart, token);
        List<RegionStatisticDto> regions = new List<RegionStatisticDto>();
        foreach (var rs in regionsStatistic)
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
    private async Task<RegionStatisticDto> FromStatisticDalAsync(RegionStatisticDal regionStatisticDal) =>
        new RegionStatisticDto(
            (await _regionDatabase.GetRegionEntityByIdAsync(regionStatisticDal.regionId)).Name,
            regionStatisticDal.TotalCountOrders,
            regionStatisticDal.TotalSumOrders,
            regionStatisticDal.TotalWigthOrders,
            regionStatisticDal.TotalCustomers);

}