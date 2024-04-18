using Dapper;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard.Common;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard.Common.Rules;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.RegionManager.Repository;
internal class RegionShardRepositoryPg : BaseShardRepository, IRegionRepository
{
    private const string Fields = "id, region as Name, latitude, longitude";
    private const string FieldsForInsert = "region, latitude, longitude";
    private const string Table = $"{ShardsHelper.BucketPlaceholder}.regions";
    public RegionShardRepositoryPg(
            IShardPostgresConnectionFactory connectionFactory,
            IShardingRule<long> longShardingRule,
            IShardingRule<SourceRegion> sourceShardingRule) : base(connectionFactory, longShardingRule, sourceShardingRule)
    {
    }
    public async Task Create(RegionDal regions, CancellationToken token)
    {
        const string sql = @$"
            insert into {Table} ({FieldsForInsert})
            values (:region_name, :latitude, :longitude)";

        foreach (var bucketId in AllBuckets)
        {
            await using var connection = GetConnectionByBucket(bucketId, token);
            var param = new DynamicParameters();
            param.Add("region_name", regions.Name);
            param.Add("latitude", regions.Latitude);
            param.Add("longitude", regions.Longitude);

            var cmd = new CommandDefinition(sql, cancellationToken: token);
            var result = await connection.QueryAsync<int>(cmd);
        }
    }

    public async Task<RegionDal[]> GetAll(CancellationToken token)
    {
        const string sql = @$"
            select id as id , region as name, latitude, longitude
            from {Table};
            ";
        await using var connection = GetConnectionByBucket(0, token);
        var result = await connection.QueryAsync<RegionDal>(sql);
        return result.ToArray();
    }
}
