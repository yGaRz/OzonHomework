using Murmur;
using Ozon.Route256.Practice.Orders.ClientBalancing;
using Ozon.Route256.Practice.OrdersService.Models;

namespace Ozon.Route256.Practice.OrdersService.DAL.Shard.Common.Rules;
public class SourceRegionShardingRule : IShardingRule<SourceRegion>
{
    private readonly IDbStore _dbStore;

    public SourceRegionShardingRule(IDbStore dbStore)
    {
        _dbStore = dbStore;
    }

    public int GetBucketId(SourceRegion shardKey)
    {
        var shardKeyHashCode = GetShardKeyHashCode(shardKey);
        return Math.Abs(shardKeyHashCode) % _dbStore.BucketsCount;
    }

    private int GetShardKeyHashCode(SourceRegion shardKey)
    {
        //var bytes = BitConverter.GetBytes((int)shardKey*1001);
        //var murmur = MurmurHash.Create32();
        //var hash = murmur.ComputeHash(bytes);
        //Возьмем некоторое большое простое число чтобы было меньше коллизий
        return ((int)shardKey.source * 17 + shardKey.region_id) % _dbStore.BucketsCount;
    }
}