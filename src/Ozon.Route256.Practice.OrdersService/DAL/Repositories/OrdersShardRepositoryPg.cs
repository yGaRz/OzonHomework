using Dapper;
using Microsoft.AspNetCore.Connections;
using Npgsql;
using Ozon.Route256.Practice.OrdersService.DAL.Common;
using Ozon.Route256.Practice.OrdersService.DAL.Models;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common.Rules;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Data;
using System.Data.Common;

namespace Ozon.Route256.Practice.OrdersService.DAL.Repositories
{
    public class OrdersShardRepositoryPg : BaseShardRepository, IOrdersRepository
    {
        private const string Fields = "id, customer_id, order_source, order_state, time_create, time_update, region_id, count_goods, total_weigth, total_price, address";
        private const string FieldsForInsert = "id, customer_id, order_source, order_state, time_create, time_update, region_id, count_goods, total_weigth, total_price, address";
        private const string Table = $"{ShardsHelper.BucketPlaceholder}.orders";
        public OrdersShardRepositoryPg(
        IShardPostgresConnectionFactory connectionFactory,
        IShardingRule<long> longShardingRule,
        IShardingRule<SourceRegion> sourceShardingRule) : base(connectionFactory, longShardingRule, sourceShardingRule)
        {
        }

        public async Task Create(OrderDal order, CancellationToken token)
        {
            const string sql = @$"
            insert into {Table} ({FieldsForInsert})
            values (:id,:customer_id , :order_source::{ShardsHelper.BucketPlaceholder}.order_source_enum, :order_state::{ShardsHelper.BucketPlaceholder}.order_state_enum, (CAST(:time_create as timestamp)), (CAST(:time_update as timestamp)), :region_id, :count_goods, :total_weigth, :total_price, (CAST(:address as json)));
        ";
            var param = new DynamicParameters();
            param.Add("id", order.id);
            param.Add("customer_id", order.customer_id);
            param.Add("order_source", order.source.ToString());
            param.Add("order_state", order.state.ToString());
            param.Add("time_create", order.timeCreate);
            param.Add("time_update", order.timeUpdate);
            param.Add("region_id", order.regionId);
            param.Add("count_goods", order.countGoods);
            param.Add("total_weigth", order.totalWeigth);
            param.Add("total_price", order.totalPrice);
            param.Add("address", order.addressJson);

            await using (var connection = GetConnectionByShardKey(order.id))
            {
                var cmd = new CommandDefinition(sql, param, cancellationToken: token);
                await connection.ExecuteAsync(cmd);
            }

            const string indexSql = $@"
            insert into  {ShardsHelper.BucketPlaceholder}.idx_order_source (source, region_id, order_id)
            VALUES (:source_str::{ShardsHelper.BucketPlaceholder}.order_source_enum, :regionId ,:id)
            ";

            await using (var connection = GetConnectionBySearchKey(new SourceRegion(order.regionId, order.source)))
            {
                string source_str = order.source.ToString();
                await connection.ExecuteAsync(indexSql, new { source_str, order.regionId, order.id });
            }
        }
        public async Task SetStatusById(long Id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token)
        {
            const string sql = @$"
            update {Table} docker
                set order_state=:order_state::{ShardsHelper.BucketPlaceholder}.order_state_enum, time_update = (CAST(:time_update as timestamp))
                where id=:id;";
            var param = new DynamicParameters();
            param.Add("id", Id);
            param.Add("order_state", state.ToString());
            param.Add("time_update", timeUpdate);
            await using (var connection = GetConnectionByShardKey(Id))
            {
                var cmd = new CommandDefinition(sql, param, cancellationToken: token);
                await connection.ExecuteAsync(cmd);
            }
        }
        public async Task<OrderDal?> GetOrderByID(long id, CancellationToken token)
        {
            const string sql = @$"
            select {Fields}
            from {Table}
            where id = :id limit 1;";

            await using var connection = GetConnectionByShardKey(id);
            var cmd = new CommandDefinition(sql, new { id }, cancellationToken: token);
            await using var reader = await connection.ExecuteReaderAsync(cmd);
            var result = await ReadOrderDal(reader, token);
            return result;
        }
        public async Task<OrderDal[]> GetOrdersByCustomerId(long idCustomer, DateTime timeCreate, CancellationToken token)
        {
            var result = new List<OrderDal>();
            foreach (var bucketId in AllBuckets)
            {
                const string sql = @$"
                            select {Fields}
                            from {Table}
                            where customer_id = :idCustomer and time_create > Cast(:timeCreate as timestamptz);";

                await using var connection = GetConnectionByBucket(bucketId, token);
                var cmd = new CommandDefinition(sql, new { idCustomer, timeCreate }, cancellationToken: token);
                await using var reader = await connection.ExecuteReaderAsync(cmd);
                var orders = await ReadOrdersDal(reader, token);
                result.AddRange(orders);
            }
            return result.ToArray();
        }
        public async Task<OrderDal[]> GetOrdersByRegion(int[] regionsId, OrderSourceEnum source, CancellationToken token)
        {
            const string indexSql = @$"
            select order_id 
            from {ShardsHelper.BucketPlaceholder}.idx_order_source
            where source = :source_str::{ShardsHelper.BucketPlaceholder}.order_source_enum and region_id = any(:regionsId);";

            List<int> ordersIds = new List<int>();
            foreach (int region in regionsId)
            {
                await using (var connectionIndex = GetConnectionBySearchKey(new SourceRegion(region, source)))
                {
                    string source_str = source.ToString();
                    var ordersIdsbyRegion = await connectionIndex.QueryAsync<int>(indexSql, new { source_str, regionsId });
                    ordersIds.AddRange(ordersIdsbyRegion);
                }
            }
            const string sql = @$"
                    select {Fields}
                    from {Table}
                    where id = any(:idsInBucket)";
            var bucketToIdsMap = ordersIds
                .Select(orderId => (BucketId: GetBucketByShardKey(orderId), OrderId: orderId))
                .GroupBy(x => x.BucketId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.OrderId).ToArray());

            var result = new List<OrderDal>();
            foreach (var (bucketId, idsInBucket) in bucketToIdsMap)
            {
                await using var connection = GetConnectionByBucket(bucketId, token);
                //var ordersInBucket = await connection.QueryAsync<OrderDal>(sql, new { ids = idsInBucket });
                var cmd = new CommandDefinition(sql, new { idsInBucket, regionsId }, cancellationToken: token);
                await using var reader = await connection.ExecuteReaderAsync(cmd);
                var ordersInBucket = await ReadOrdersDal(reader, token);
                result.AddRange(ordersInBucket);
            }
            return result.ToArray();
        }
        public async Task<RegionStatisticDal[]> GetRegionStatistic(int[] regionsId, DateTime timeCreate, CancellationToken token)
        {
            var total = new List<RegionStatisticDal>();
            foreach (var bucketId in AllBuckets)
            {
                const string sql = @$"
                    SELECT region_id,count(*), sum(total_price), sum(total_weigth), count(distinct customer_id)
                    FROM {Table}
                    where time_create > Cast(:timeCreate as timestamptz) and region_id = any(:regionsId)
                    group by region_id;";

                await using var connection = GetConnectionByBucket(bucketId, token);
                var cmd = new CommandDefinition(sql, new { timeCreate, regionsId }, cancellationToken: token);
                await using var reader = await connection.ExecuteReaderAsync(cmd);
                var statistic = await ReadRegionStatisticDal(reader, token);
                total.AddRange(statistic);
            }
            var regionsStatistic = total.GroupBy(x => x.regionId).Select(r => new RegionStatisticDal(
                r.Key,
                r.Sum(q => q.TotalCountOrders),
                r.Sum(q => q.TotalSumOrders),
                r.Sum(q => q.TotalWigthOrders),
                r.Sum(x => x.TotalCustomers))).ToArray();
            return regionsStatistic;
        }
        private static async Task<OrderDal> ReadOrderDal(DbDataReader reader, CancellationToken token)
        {
            await reader.ReadAsync(token);
            return new OrderDal(
                        id: reader.GetFieldValue<int>(0),
                        customer_id: reader.GetFieldValue<int>(1),
                        source: reader.GetFieldValue<OrderSourceEnum>(2),
                        state: reader.GetFieldValue<OrderStateEnum>(3),
                        timeCreate: reader.GetFieldValue<DateTime>(4),
                        timeUpdate: reader.GetFieldValue<DateTime>(5),
                        regionId: reader.GetFieldValue<int>(6),
                        countGoods: reader.GetFieldValue<int>(7),
                        totalWeigth: reader.GetFieldValue<double>(8),
                        totalPrice: reader.GetFieldValue<double>(9),
                        addressJson: reader.GetFieldValue<string>(10)
                    );
        }
        private static async Task<OrderDal[]> ReadOrdersDal(DbDataReader reader, CancellationToken token)
        {
            var result = new List<OrderDal>();
            while (await reader.ReadAsync(token))
            {
                result.Add(
                    new OrderDal(
                        id: reader.GetFieldValue<int>(0),
                        customer_id: reader.GetFieldValue<int>(1),
                        source: reader.GetFieldValue<OrderSourceEnum>(2),
                        state: reader.GetFieldValue<OrderStateEnum>(3),
                        timeCreate: reader.GetFieldValue<DateTime>(4),
                        timeUpdate: reader.GetFieldValue<DateTime>(5),
                        regionId: reader.GetFieldValue<int>(6),
                        countGoods: reader.GetFieldValue<int>(7),
                        totalWeigth: reader.GetFieldValue<double>(8),
                        totalPrice: reader.GetFieldValue<double>(9),
                        addressJson: reader.GetFieldValue<string>(10)
                    ));
            }
            return result.ToArray();
        }
        private static async Task<RegionStatisticDal[]> ReadRegionStatisticDal(DbDataReader reader, CancellationToken token)
        {
            var result = new List<RegionStatisticDal>();
            while (await reader.ReadAsync(token))
            {
                result.Add(
                    new RegionStatisticDal(
                        regionId: reader.GetFieldValue<int>(0),
                        TotalCountOrders: reader.GetFieldValue<int>(1),
                        TotalSumOrders: reader.GetFieldValue<long>(2),
                        TotalWigthOrders: reader.GetFieldValue<long>(3),
                        TotalCustomers: reader.GetFieldValue<int>(4)
                    ));
            }
            return result.ToArray();
        }
    }
}
