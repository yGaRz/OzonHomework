using Google.Protobuf.WellKnownTypes;
using Npgsql;
using Ozon.Route256.Practice.OrdersService.DAL.Common;
using Ozon.Route256.Practice.OrdersService.DAL.Models;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Data;

namespace Ozon.Route256.Practice.OrdersService.DAL.Repositories
{
    public class OrdersRepositoryPg
    {
        private const string Fields = "id, customer_id, order_source, order_state, time_create, time_update, region_id, count_goods, total_weigth, total_price, address";
        private const string FieldsForInsert = "id, customer_id, order_source, order_state, time_create, time_update, region_id, count_goods, total_weigth, total_price, address";
        private const string Table = "orders";

        private readonly IPostgresConnectionFactory _connectionFactory;

        public OrdersRepositoryPg(
            IPostgresConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Create(OrderDal order, CancellationToken token)
        {
            const string sql = @$"
            insert into {Table} ({FieldsForInsert})
            values (:id,:customer_id , :order_source, :order_state, (CAST(:time_create as timestamp)), (CAST(:time_update as timestamp)), :region_id, :count_goods, :total_weigth, :total_price, (CAST(:address as json)));
        ";
            await using var connection = _connectionFactory.GetConnection();
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.Add("id", order.id);
            command.Parameters.Add("customer_id", order.customer_id);
            command.Parameters.Add("order_source", order.source);
            command.Parameters.Add("order_state", order.state);
            command.Parameters.Add("time_create", order.timeCreate);
            command.Parameters.Add("time_update", order.timeUpdate);
            command.Parameters.Add("region_id", order.regioId);
            command.Parameters.Add("count_goods", order.countGoods);
            command.Parameters.Add("total_weigth", order.totalWeigth);
            command.Parameters.Add("total_price", order.totalPrice);
            command.Parameters.Add("address", order.addressJson);

            await connection.OpenAsync(token);
            await command.ExecuteNonQueryAsync(token);
        }

        public async Task SetStatusById(long Id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token)
        {
            const string sql = @$"
            update {Table} 
                set order_state=:order_state, time_update = (CAST(:time_update as timestamp))
                where id=:id;";
            await using var connection = _connectionFactory.GetConnection();
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.Add("id", Id);
            command.Parameters.Add("order_state", state);
            command.Parameters.Add("time_update", timeUpdate.ToString());
            await connection.OpenAsync(token);
            await command.ExecuteNonQueryAsync(token);
        }

        public async Task<OrderDal?> GetOrderByID(long id, CancellationToken token)
        {
            const string sql = @$"
            select {Fields}
            from {Table}
            where id = :id;";
            await using var connection = _connectionFactory.GetConnection();
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.Add("id", id);

            await connection.OpenAsync(token);
            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, token);

            var result = await ReadOrderDal(reader, token);
            return result.FirstOrDefault();
        }
        private static async Task<OrderDal[]> ReadOrderDal(NpgsqlDataReader reader, CancellationToken token)
        {
            var result = new List<OrderDal>();
            while (await reader.ReadAsync(token))
            {
                result.Add(
                    new OrderDal(
                        id: reader.GetFieldValue<long>(0),
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
        public async Task<OrderStateEnum> GetStatusById(long Id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token)
        {
            const string sql = @$"
            select order_state
                from {Table}
                where id=:id;";
            await using var connection = _connectionFactory.GetConnection();
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.Add("id", Id);
            await connection.OpenAsync(token);
            await using var reader = await command.ExecuteReaderAsync(token);
            var result = await ReadOrderState(reader, token);
            return result;
        }
        private static async Task<OrderStateEnum> ReadOrderState(NpgsqlDataReader reader, CancellationToken token)
        {
            await reader.ReadAsync(token);
            return reader.GetFieldValue<OrderStateEnum>(0);
        }
        public async Task<RegionStatisticDal[]> GetRegionStatistic(int[] regionsId, DateTime timeCreate, CancellationToken token)
        {
            const string sql = @$"
                    SELECT region_id,count(*),sum(total_price), sum(total_weigth), count(distinct customer_id)
                    FROM {Table}
                    where time_create > Cast(:dateCreate as timestamptz) and region_id = any(:arrayid)
                    group by region_id";
            await using var connection = _connectionFactory.GetConnection();
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.Add("dateCreate", timeCreate.ToString());
            command.Parameters.Add("arrayid", regionsId);
            await connection.OpenAsync(token);
            await using var reader = await command.ExecuteReaderAsync(token);
            var result = await ReadRegionStatisticDal(reader, token);
            return result;
        }
        private static async Task<RegionStatisticDal[]> ReadRegionStatisticDal(NpgsqlDataReader reader, CancellationToken token)
        {
            var result = new List<RegionStatisticDal>();
            while (await reader.ReadAsync(token))
            {
                result.Add(
                    new RegionStatisticDal(
                        regionId : reader.GetFieldValue<int>(0),
                        TotalCountOrders: reader.GetFieldValue<int>(1),
                        TotalSumOrders: reader.GetFieldValue<long>(2),
                        TotalWigthOrders: reader.GetFieldValue<long>(3),
                        TotalCustomers: reader.GetFieldValue<int>(4)
                    ));
            }
            return result.ToArray();
        }

        public async Task<OrderDal[]> GetOrdersByCustomerId(long idCustomer, DateTime timeCreate, CancellationToken token)
        {
            const string sql = @$"
            select {Fields}
            from {Table}
            where customer_id = :idCustomer and time_create > Cast(:dateCreate as timestamptz);";
            await using var connection = _connectionFactory.GetConnection();
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.Add("idCustomer", idCustomer);
            command.Parameters.Add("dateCreate", timeCreate.ToString());
            await connection.OpenAsync(token);
            await using var reader = await command.ExecuteReaderAsync(token);

            var result = await ReadOrderDal(reader, token);
            return result;
        }
    }
}
