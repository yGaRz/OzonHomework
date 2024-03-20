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
        public Task<OrderEntity[]> GetOrdersByCutomer(long idCustomer, DateTime dateStart, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(OrdersRep.Values.Where(
                    x => x.Customer.Id == idCustomer && Timestamp.FromDateTime(dateStart) < x.TimeCreate
                ).ToArray());
        }
        //Сортировку надо делать будет снаружи
        public Task<OrderEntity[]> GetOrdersByRegionAsync(List<string> regionList, OrderSource source, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(OrdersRep.Values.Where(x => regionList.Contains(x.Customer.Address.Region) && x.Source==source).ToArray());
        }
        //TODO: Репозиторий. Получение статистики
        public Task<RegionStatisticEntity[]> GetRegionsStatisticAsync(List<string> regionList, DateTime dateStart, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        //Выборка всех заказов из репозитория
        public Task<OrderEntity[]> GetAllOrdersAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(OrdersRep.Values.ToArray());
        }
        //Смена статуса заказа в Репозитории
        Task<bool> IOrdersRepository.SetOrderStateAsync(long id, OrderStateEnum state, CancellationToken token)
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
