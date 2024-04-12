using Npgsql;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common.Rules;
using Ozon.Route256.Practice.OrdersService.Models;

namespace Ozon.Route256.Practice.OrdersService.DAL.Repositories.ShardRepository;

public class BaseShardRepository
{
    private readonly IShardPostgresConnectionFactory _connectionFactory;
    private readonly IShardingRule<long> _longShardingRule;
    private readonly IShardingRule<SourceRegion> _sourceShardingRule;

    public BaseShardRepository(
        IShardPostgresConnectionFactory connectionFactory,
        IShardingRule<long> longShardingRule,
        IShardingRule<SourceRegion> sourceShardingRule)
    {
        _connectionFactory  = connectionFactory;
        _longShardingRule = longShardingRule;
        _sourceShardingRule = sourceShardingRule;
    }

    protected ShardNpgsqlConnection GetConnectionByShardKey(long shardKey)
    {
        var bucketId = _longShardingRule.GetBucketId(shardKey);
        return _connectionFactory.GetConnectionByBucketId(bucketId);
    }

    protected ShardNpgsqlConnection GetConnectionByBucket(
        int bucketId,
        CancellationToken token)
    {
        return _connectionFactory.GetConnectionByBucketId(bucketId);
    }

    protected int GetBucketByShardKey(long shardKey) =>
        _longShardingRule.GetBucketId(shardKey);

    protected ShardNpgsqlConnection GetConnectionBySearchKey(SourceRegion searchKey)
    {
        var bucketId = _sourceShardingRule.GetBucketId(searchKey);
        return _connectionFactory.GetConnectionByBucketId(bucketId);
    }


    protected IEnumerable<int> AllBuckets => _connectionFactory.GetAllBuckets();
}