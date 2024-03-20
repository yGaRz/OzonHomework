using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Collections.Concurrent;
using System.Reflection;

namespace Ozon.Route256.Practice.OrdersService.DataAccess
{
    public class OrdersRepository : IOrdersRepository
    {
        private static readonly ConcurrentDictionary<long, OrderEntity> OrdersRep = new ConcurrentDictionary<long, OrderEntity>();
        public Task CreateOrderAsync(OrderEntity order, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            PropertyInfo propertyInfo = typeof(OrderEntity).GetProperty(nameof(OrderEntity.State));
            propertyInfo.SetValue(order, OrderStateEnum.Created);
            //order.State = Models.OrderStateEnum.Created;


            if (OrdersRep.TryAdd(order.Id, order))
                return Task.CompletedTask;
            else
                return Task.FromException(new Exception($"Order with id={order.Id} is already exists"));
        }

        public Task<OrderEntity> GetOrderByIdAsync(long id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var order = OrdersRep.Values.First(x=>x.Id == id);
            token.ThrowIfCancellationRequested();
            return Task.FromResult(order);
        }

        public Task<OrderEntity[]> GetOrdersByCutomer(long idCustomer, DateTime dateStart, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(OrdersRep.Values.Where(x => x.Customer.Id == idCustomer).ToArray());
        }
        public Task<OrderEntity[]> GetOrdersByRegionAsync(List<string> regionList, OrderSource source, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(OrdersRep.Values.Where(x => regionList.Contains(x.Customer.Address.Region) && x.Source==source).ToArray());
        }
        public Task<RegionStatisticEntity[]> GetRegionsStatisticAsync(List<string> regionList, DateTime dateStart, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
        public Task<OrderEntity[]> GetAllOrdersAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(OrdersRep.Values.ToArray());
        }

        Task<bool> IOrdersRepository.SetOrderStateAsync(long id, OrderState state, CancellationToken token)
        {
            if(OrdersRep.TryGetValue(id, out var result))
            {
                
            }
            throw new NotImplementedException();
        }
    }
}
