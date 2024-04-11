using Dapper;
using Microsoft.AspNetCore.Connections;
using Npgsql;
using Ozon.Route256.Practice.OrdersService.DAL.Models;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;
using Ozon.Route256.Practice.OrdersService.Models;

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
            values (:id,:customer_id , :order_source, :order_state, (CAST(:time_create as timestamp)), (CAST(:time_update as timestamp)), :region_id, :count_goods, :total_weigth, :total_price, (CAST(:address as json)));
        ";
            var param = new DynamicParameters();
            param.Add("id", order.id);
            param.Add("customer_id", order.customer_id);
            param.Add("order_source", order.source);
            param.Add("order_state", order.state);
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

        public Task<OrderDal?> GetOrderByID(long id, CancellationToken token)
        {
            throw new NotImplementedException();
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

        public Task SetStatusById(long Id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
