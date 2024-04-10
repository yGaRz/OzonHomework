using Ozon.Route256.Practice.OrdersService.DAL.Models;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;
using Ozon.Route256.Practice.OrdersService.Models;

namespace Ozon.Route256.Practice.OrdersService.DAL.Repositories.ShardRepository
{
    public class OrdersShardRepositoryPg : BaseShardRepository, IOrdersRepository
    {
        public OrdersShardRepositoryPg(
        IShardPostgresConnectionFactory connectionFactory,
        IShardingRule<int> shardingRule) : base(connectionFactory, shardingRule)
        {
        }

        public Task Create(OrderDal order, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDal?> GetOrderByID(long id, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDal[]> GetOrdersByCustomerId(long idCustomer, DateTime timeCreate, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDal[]> GetOrdersByRegion(int[] regionsId, OrderSourceEnum source, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<RegionStatisticDal[]> GetRegionStatistic(int[] regionsId, DateTime timeCreate, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<OrderStateEnum> GetStatusById(long Id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task SetStatusById(long Id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
