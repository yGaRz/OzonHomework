﻿using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models.Enums;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.Orders.Repository;

internal interface IOrdersRepository
{
    Task Create(OrderDal order, CancellationToken token);
    Task<OrderDal?> GetOrderByID(long id, CancellationToken token);
    Task<OrderDal[]> GetOrdersByCustomerId(long idCustomer, DateTime timeCreate, CancellationToken token);
    Task<RegionStatisticDal[]> GetRegionStatistic(int[] regionsId, DateTime timeCreate, CancellationToken token);
    //Task<OrderStateEnum> GetStatusById(long Id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token);
    Task SetStatusById(long Id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token);
    Task<OrderDal[]> GetOrdersByRegion(int[] regionsId, OrderSourceEnum source, CancellationToken token);
}