﻿using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Collections.Concurrent;
namespace Ozon.Route256.Practice.OrdersService.DataAccess
{
    public interface IOrdersRepository
    {
        //Добавление заказа в репозиторий
        Task CreateOrderAsync(OrderEntity order, CancellationToken token = default);
        
        //Выбор заказа по идентификатору
        Task<OrderEntity> GetOrderByIdAsync(long id, CancellationToken token = default);
        
        //Выбор всех заказов
        Task<OrderEntity[]> GetAllOrdersAsync(CancellationToken token = default);
        
        //Смена состояния заказа
        Task<bool> SetOrderStateAsync(long id, OrderStateEnum state, CancellationToken token = default);
        
        //выборка списка заказов, тут сделаем выборку по типу(web/api/site) + регион, сортировать будем выше.
        Task<OrderEntity[]> GetOrdersByRegionAsync(List<string> regionList, 
                                                            OrderSource source, 
                                                            CancellationToken token = default);
        //Получение агегации(статистики) заказов по региону
        Task<RegionStatisticEntity[]> GetRegionsStatisticAsync(List<string> regionList, Timestamp dateStart, CancellationToken token = default);
        //Получение всех заказов клиента
        Task<OrderEntity[]> GetOrdersByCutomerAsync(long idCustomer, DateTime dateStart, CancellationToken token = default);
    }
}
