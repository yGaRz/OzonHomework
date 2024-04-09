﻿using Murmur;
using Ozon.Route256.Practice.Orders.ClientBalancing;

namespace Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;

public interface IShardingRule<TShardKey>
{
    int GetBucketId(TShardKey shardKey);
}

public class IntShardingRule: IShardingRule<int>
{
    private readonly IDbStore _dbStore;

    public IntShardingRule(
        IDbStore dbStore)
    {
        _dbStore = dbStore;
    }

    public int GetBucketId(
        int shardKey)
    {
        var shardKeyHashCode = GetShardKeyHashCode(shardKey);

        return Math.Abs(shardKeyHashCode) % _dbStore.BucketsCount;
    }

    private int GetShardKeyHashCode(
        int shardKey)
    {
        var bytes = BitConverter.GetBytes(shardKey);
        var murmur = MurmurHash.Create32();
        var hash = murmur.ComputeHash(bytes);
        return BitConverter.ToInt32(hash);
    }
}