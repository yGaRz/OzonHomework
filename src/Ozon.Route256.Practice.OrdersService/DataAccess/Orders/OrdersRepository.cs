using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Collections.Concurrent;
using System.Reflection;

namespace Ozon.Route256.Practice.OrdersService.DataAccess.Orders
{
    public class OrdersRepository : IOrdersRepository
    {
        private static readonly ConcurrentDictionary<long, OrderEntity> OrdersRep = new ConcurrentDictionary<long, OrderEntity>();
        public Task CreateOrderAsync(OrderEntity order, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (OrdersRep.TryAdd(order.Id, order))
                return Task.CompletedTask;
            else
                return Task.FromException(new Exception($"Order with id={order.Id} is already exists"));
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
                    x => x.Customer.Id == idCustomer && dateStart < x.TimeCreate
                ).ToArray();
            return Task.FromResult(res);
        }

        //Сортировку надо делать будет снаружи
        public Task<OrderEntity[]> GetOrdersByRegionAsync(List<string> regionList,
                                                            OrderSourceEnum source,
                                                            CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(OrdersRep.Values.Where(x => regionList.Contains(x.Customer.Address.Region) && x.Source == source).ToArray());
        }

        Task<OrderEntity[]> IOrdersRepository.GetOrdersByRegionAsync(List<string> regionList,
                                                                        DateTime dateStart,
                                                                        CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(OrdersRep.Values.Where(x => regionList.Contains(x.Customer.Address.Region) && x.TimeCreate > dateStart).ToArray());
        }

        public Task<RegionStatisticEntity[]> GetRegionsStatisticAsync(List<string> regionList, DateTime dateStart, CancellationToken token = default)
        {
            List<RegionStatisticEntity> regions = new List<RegionStatisticEntity>();

            foreach (var region in regionList)
            {
                uint countOrders = (uint)OrdersRep.Values.Where(x => x.Customer.Address.Region == region && x.TimeCreate > dateStart).Count();
                double sumOrders = (double)OrdersRep.Values.Where(x => x.Customer.Address.Region == region && x.TimeCreate > dateStart)
                                                    .Sum(x => x.Goods.Sum(z => z.Price));
                double weigthOrder = OrdersRep.Values.Where(x => x.Customer.Address.Region == region && x.TimeCreate > dateStart)
                                                    .Sum(x => x.Goods.Sum(z => z.Weight));
                uint totalCustomer = (uint)OrdersRep.Values.Where(x => x.Customer.Address.Region == region && x.TimeCreate > dateStart)
                                                    .GroupBy(x => x.Customer.Id).Distinct().Count();
                regions.Add(new RegionStatisticEntity(region, countOrders, sumOrders, weigthOrder, totalCustomer));
            }
            return Task.FromResult(regions.ToArray());
        }
        public Task<OrderEntity[]> GetAllOrdersAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(OrdersRep.Values.ToArray());
        }
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
