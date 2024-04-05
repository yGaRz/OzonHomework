using Npgsql;
using Ozon.Route256.Practice.OrdersService.DAL.Common;
using Ozon.Route256.Practice.OrdersService.DAL.Models;

namespace Ozon.Route256.Practice.OrdersService.DAL.Repositories
{
    public class OrdersRepositoryPg
    {
        private const string FieldsForInsert = "id, customer_id, order_source, order_state, time_create, time_update, region_id, count_goods, total_weigth, total_price, address";
        private const string Table = "orders";

        private readonly IPostgresConnectionFactory _connectionFactory;

        public OrdersRepositoryPg(
            IPostgresConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Create(
            OrderDal order,
            CancellationToken token)
        {
            const string sql = @$"
            insert into {Table} ({FieldsForInsert})
            values (:id,:customer_id , :order_source, :order_state, :time_create, :time_update, :region_id, :count_goods, :total_weigth, :total_price, :address);
        ";

            await using var connection = _connectionFactory.GetConnection();
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.Add("id", order.id);
            command.Parameters.Add("customer_id", order.customer_id);
            command.Parameters.Add("order_source", order.source);
            command.Parameters.Add("order_state", order.state);
            command.Parameters.Add("time_create", order.dateCreate);
            command.Parameters.Add("time_update", order.dateUpdate);
            command.Parameters.Add("region_id", order.regioId);
            command.Parameters.Add("count_goods", order.countGoods);
            command.Parameters.Add("total_weigth", order.totalWeigth);
            command.Parameters.Add("total_price", order.totalPrice);
            command.Parameters.Add("address", order.addressJson);

            await connection.OpenAsync(token);
            await command.ExecuteNonQueryAsync(token);
        }

    }
}
