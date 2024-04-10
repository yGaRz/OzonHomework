using Microsoft.AspNetCore.Connections;
using Dapper;
using Ozon.Route256.Practice.OrdersService.DAL.Models;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;

namespace Ozon.Route256.Practice.OrdersService.DAL.Repositories.ShardRepository
{
    public class RegionShardRepositoryPg : BaseShardRepository, IRegionRepository
    {
        private const string Fields = "id, region as Name, latitude, longitude";
        private const string FieldsForInsert = "region, latitude, longitude";
        private const string Table = $"{ShardsHelper.BucketPlaceholder}.regions";
        public RegionShardRepositoryPg(
                IShardPostgresConnectionFactory connectionFactory,
                IShardingRule<int> shardingRule) : base(connectionFactory, shardingRule)
        {
        }
        public async Task<int> Create(RegionDal regions, CancellationToken token)
        {
            const string sql = @$"
            insert into {Table} ({FieldsForInsert})
            values (:region_name, :latitude, :longitude)
            returning id;      
            ";

            await using var connection = GetConnectionByBucket(0,token);

            var param = new DynamicParameters();
            param.Add("region_name", regions.Name);
            param.Add("latitude", regions.Latitude);
            param.Add("longitude", regions.Longitude);

            var cmd = new CommandDefinition(sql, cancellationToken: token);
            var result = await connection.QueryAsync<int>(cmd);
            return result.ToArray().FirstOrDefault();
        }

        public async Task<RegionDal[]> GetAll(CancellationToken token)
        {
            const string sql = @$"
            select id as id , region as name, latitude, longitude
            from {Table};
            ";
            await using var connection = GetConnectionByBucket(0,token);
            var result = await connection.QueryAsync<RegionDal>(sql);
            return result.ToArray();
        }
    }
}
