using Ozon.Route256.Practice.OrdersService.DAL.Models;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;

namespace Ozon.Route256.Practice.OrdersService.DAL.Repositories.ShardRepository
{
    public class RegionShardRepositoryPg : BaseShardRepository, IRegionRepository
    {
        public RegionShardRepositoryPg(
                IShardPostgresConnectionFactory connectionFactory,
                IShardingRule<int> shardingRule,
                IShardingRule<string> stringShardingRule) : base(connectionFactory, shardingRule, stringShardingRule)
        {
        }
        public Task<int> Create(RegionDal regions, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<RegionDal[]> GetAll(CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
