using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.DataAccess
{
    public class OrdersRepository : IOrdersRepository
    {
        public Task CreateOrderAsync(OrderEntity order, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<OrderEntity> GetOrderByCustomer(int id, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderEntity>> GetOrdersByCutomer(ulong idCustomer, DateTime dateStart, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderEntity>> GetOrdersByRegionAsync(List<string> regionList, OrderSource source, SortParam param = SortParam.None, string field_name = "", CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<RegionStatisticEntity>> GetRegionsStatisticAsync(List<string> regionList, DateTime dateStart, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetOrderStateAsync(int id, OrderState state, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
