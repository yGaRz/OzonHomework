using Dapper;
using Microsoft.AspNetCore.Connections;
using Npgsql;
using Ozon.Route256.Practice.OrdersService.DAL.Common;
using Ozon.Route256.Practice.OrdersService.DAL.Models;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Data;
using System.Data.Common;

namespace Ozon.Route256.Practice.OrdersService.DAL.Repositories.ShardRepository
{
    public class OrdersShardRepositoryPg : BaseShardRepository, IOrdersRepository
    {
        private const string Fields = "id, customer_id, order_source, order_state, time_create, time_update, region_id, count_goods, total_weigth, total_price, address";
        private const string FieldsForInsert = "id, customer_id, order_source, order_state, time_create, time_update, region_id, count_goods, total_weigth, total_price, address";
        private const string Table = $"{ShardsHelper.BucketPlaceholder}.orders";
        public OrdersShardRepositoryPg(
        IShardPostgresConnectionFactory connectionFactory,
        IShardingRule<long> shardingRule) : base(connectionFactory, shardingRule)
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
            param.Add("region_id", order.regioId);
            param.Add("count_goods", order.countGoods);
            param.Add("total_weigth", order.totalWeigth);
            param.Add("total_price", order.totalPrice);
            param.Add("address", order.addressJson);

            await using (var connection = GetConnectionByShardKey(order.id))
            {
                var cmd = new CommandDefinition(sql, param, cancellationToken: token);
                await connection.ExecuteAsync(cmd);
            }
        }
        public async Task SetStatusById(long Id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token)
        {
            const string sql = @$"
            update {Table} 
                set order_state=:order_state::{ShardsHelper.BucketPlaceholder}.order_state_enum, time_update = (CAST(:time_update as timestamp))
                where id=:id;";
            var param = new DynamicParameters();
            param.Add("id", Id);
            param.Add("order_state", state.ToString());
            param.Add("time_update", timeUpdate.ToString());
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
            where id = :id;";

            await using var connection = GetConnectionByShardKey(id);
            var cmd = new CommandDefinition(sql, new { id }, cancellationToken: token);
            await using var reader = await connection.ExecuteReaderAsync(cmd);
            var result = await ReadOrderDal(reader, token);
            return result.FirstOrDefault();
        }
        private static async Task<OrderDal[]> ReadOrderDal(DbDataReader reader, CancellationToken token)
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
                        regioId: reader.GetFieldValue<int>(6),
                        countGoods: reader.GetFieldValue<int>(7),
                        totalWeigth: reader.GetFieldValue<double>(8),
                        totalPrice: reader.GetFieldValue<double>(9),
                        addressJson: reader.GetFieldValue<string>(10)
                    ));
            }
            return result.ToArray();
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


    }
}
