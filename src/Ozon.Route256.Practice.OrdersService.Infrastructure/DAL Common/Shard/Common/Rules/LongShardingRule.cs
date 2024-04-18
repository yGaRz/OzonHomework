using Murmur;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ClientBalancing;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard.Common.Rules;

public interface IShardingRule<TShardKey>
{
    int GetBucketId(TShardKey shardKey);
}

public class LongShardingRule : IShardingRule<long>
{
    private readonly IDbStore _dbStore;

    public LongShardingRule(
        IDbStore dbStore)
    {
        _dbStore = dbStore;
    }

    public int GetBucketId(
        long shardKey)
    {
        var shardKeyHashCode = GetShardKeyHashCode(shardKey);

        return Math.Abs(shardKeyHashCode) % _dbStore.BucketsCount;
    }

    private int GetShardKeyHashCode(
        long shardKey)
    {
        var bytes = BitConverter.GetBytes(shardKey);
        var murmur = MurmurHash.Create32();
        var hash = murmur.ComputeHash(bytes);
        return BitConverter.ToInt32(hash);
    }
}