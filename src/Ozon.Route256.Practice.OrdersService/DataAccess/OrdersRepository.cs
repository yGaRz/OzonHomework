using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Collections.Concurrent;
using System.Reflection;

namespace Ozon.Route256.Practice.OrdersService.DataAccess
{
    public class OrdersRepository : IOrdersRepository
    {
        private static readonly ConcurrentDictionary<long, OrderEntity> OrdersRep = new ConcurrentDictionary<long, OrderEntity>();
        //Добавление заказа
        public Task CreateOrderAsync(OrderEntity order, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (OrdersRep.TryAdd(order.Id, order))
                return Task.CompletedTask;
            else
                return Task.FromException(new Exception($"Order with id={order.Id} is already exists"));
        }
        //Получение заказа по Id
        public Task<OrderEntity> GetOrderByIdAsync(long id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            if(OrdersRep.TryGetValue(id,out var order))
                return Task.FromResult(order);
            else
                return Task.FromException<OrderEntity>(new NotFoundException($"Заказ с номером {id} не найден"));
        }
        //Получение списка заказов начиная с указанного времени
        public Task<OrderEntity[]> GetOrdersByCutomerAsync(long idCustomer, DateTime dateStart, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var res = OrdersRep.Values.Where(
                    x => x.Customer.Id == idCustomer && Timestamp.FromDateTime(dateStart) < x.TimeCreate
                ).ToArray();
            return Task.FromResult(res);
        }
        //Сортировку надо делать будет снаружи
        public Task<OrderEntity[]> GetOrdersByRegionAsync(List<string> regionList, OrderSourceEnum source, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(OrdersRep.Values.Where(x => regionList.Contains(x.Customer.Address.Region) && x.Source == source).ToArray());
        }
        //Репозиторий. Получение статистики select * group by  и поехали 
        public Task<RegionStatisticEntity[]> GetRegionsStatisticAsync(List<string> regionList, Timestamp dateStart, CancellationToken token = default)
        {
            List<RegionStatisticEntity> regions = new List<RegionStatisticEntity>();
            foreach(var region in regionList)
            {
                uint countOrders = (uint)OrdersRep.Values.Where(x => x.Customer.Address.Region == region && x.TimeCreate>dateStart).Count();
                double sumOrders = (double)OrdersRep.Values.Where(x => x.Customer.Address.Region == region && x.TimeCreate > dateStart).Sum(x=>x.Goods.Sum(z=>z.Price));
                double weigthOrder = OrdersRep.Values.Where(x => x.Customer.Address.Region == region && x.TimeCreate > dateStart).Sum(x => x.Goods.Sum(z => z.Weight));
                uint totalCustomer = (uint)OrdersRep.Values.Where(x => x.Customer.Address.Region == region && x.TimeCreate > dateStart).GroupBy(x=>x.Customer.Id).Distinct().Count();
                regions.Add(new RegionStatisticEntity(region, countOrders, sumOrders, weigthOrder, totalCustomer));
            }
            return Task.FromResult(regions.ToArray());
        }

        //Выборка всех заказов из репозитория
        public Task<OrderEntity[]> GetAllOrdersAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(OrdersRep.Values.ToArray());
        }
        //Смена статуса заказа в Репозитории
        public Task<bool> SetOrderStateAsync(long id, OrderStateEnum state, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (OrdersRep.TryGetValue(id, out var result))
            {
                result.State = state;
                return Task.FromResult(true);
            }
            else
                return Task.FromResult(false);
        }
    }
}
