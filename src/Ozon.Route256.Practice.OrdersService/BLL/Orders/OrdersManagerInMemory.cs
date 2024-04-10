using Bogus.DataSets;
using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.OrdersService.DAL.Models;
using Ozon.Route256.Practice.OrdersService.DAL.Repositories;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;

namespace Ozon.Route256.Practice.OrdersService.DataAccess.Orders
{
    public class OrdersManagerInMemory : IOrdersManager
    {
        private static readonly ConcurrentDictionary<long, OrderEntity> OrdersRep = new ConcurrentDictionary<long, OrderEntity>();
        private readonly IRegionDatabase _regionDatabase;
        public OrdersManagerInMemory(OrdersRepositoryPg ordersRepositoryPg, IRegionDatabase regionDatabase)
        {
            _regionDatabase = regionDatabase;
        }

        public Task CreateOrderAsync(OrderEntity order, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (OrdersRep.TryAdd(order.Id, order))          
                return Task.CompletedTask;
            else
                throw new ArgumentException($"Order with id={order.Id} is already exists");
        }

        public Task<OrderEntity> GetOrderByIdAsync(long id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            if (OrdersRep.TryGetValue(id, out var order))
                return Task.FromResult(order);
            else
                return Task.FromException<OrderEntity>(new NotFoundException($"Заказ с номером {id} не найден"));
        }

        public Task<OrderEntity[]> GetOrdersByCutomerAsync(long idCustomer, DateTime dateStart, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var res = OrdersRep.Values.Where(
                    x => x.CustomerId == idCustomer && dateStart < x.TimeCreate
                ).ToArray();
            return Task.FromResult(res);
        }

        //Сортировку надо делать будет снаружи
        public Task<OrderEntity[]> GetOrdersByRegionAsync(List<string> regionList,
                                                            OrderSourceEnum source,
                                                            CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(OrdersRep.Values.Where(x => regionList.Contains(x.Address.Region) && x.Source == source).ToArray());
        }


        public Task<RegionStatisticEntity[]> GetRegionsStatisticAsync(List<string> regionList, DateTime dateStart, CancellationToken token = default)
        {
            List<RegionStatisticEntity> regions = new List<RegionStatisticEntity>();

            foreach (var region in regionList)
            {
                int countOrders = OrdersRep.Values.Where(x => x.Address.Region == region && x.TimeCreate > dateStart).Count();
                double sumOrders = (double)OrdersRep.Values.Where(x => x.Address.Region == region && x.TimeCreate > dateStart)
                                                    .Sum(x => x.Goods.Sum(z => z.Price));
                double weigthOrder = OrdersRep.Values.Where(x => x.Address.Region == region && x.TimeCreate > dateStart)
                                                    .Sum(x => x.Goods.Sum(z => z.Weight));
                int totalCustomer = OrdersRep.Values.Where(x => x.Address.Region == region && x.TimeCreate > dateStart)
                                                    .GroupBy(x => x.CustomerId).Distinct().Count();
                regions.Add(new RegionStatisticEntity(region, countOrders, sumOrders, weigthOrder, totalCustomer));
            }
            return Task.FromResult(regions.ToArray());
        }

        public Task<bool> SetOrderStateAsync(long id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            if (OrdersRep.TryGetValue(id, out var result))
            {
                result.State = state;
                result.TimeUpdate = timeUpdate;
                return Task.FromResult(true);
            }
            else
                return Task.FromResult(false);
        }
    }
}
