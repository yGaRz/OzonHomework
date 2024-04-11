﻿using Npgsql;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;

namespace Ozon.Route256.Practice.OrdersService.DAL.Repositories.ShardRepository;

public class BaseShardRepository
{
    private readonly IShardPostgresConnectionFactory _connectionFactory;
    private readonly IShardingRule<long> _shardingRule;

    public BaseShardRepository(
        IShardPostgresConnectionFactory connectionFactory,
        IShardingRule<long> shardingRule)
    {
        _connectionFactory  = connectionFactory;
        _shardingRule       = shardingRule;
    }

    protected ShardNpgsqlConnection GetConnectionByShardKey(long shardKey)
    {
        var bucketId = _shardingRule.GetBucketId(shardKey);
        return _connectionFactory.GetConnectionByBucketId(bucketId);
    }

    protected ShardNpgsqlConnection GetConnectionByBucket(
        int bucketId,
        CancellationToken token)
    {
        return _connectionFactory.GetConnectionByBucketId(bucketId);
    }

    protected int GetBucketByShardKey(long shardKey) => 
        _shardingRule.GetBucketId(shardKey);

    protected IEnumerable<int> AllBuckets => _connectionFactory.GetAllBuckets();
}