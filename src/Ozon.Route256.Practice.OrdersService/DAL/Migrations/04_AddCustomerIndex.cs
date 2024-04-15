using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;

namespace Ozon.Route256.Practice.OrdersService.DAL.Migrations;

[Migration(4, "Create enums migration")]
public class AddCustomerIndex : ShardSqlMigration
{
    protected override string GetUpSql(
        IServiceProvider services) => @"
            create index customer_id_idx on orders (customer_id);

";

    protected override string GetDownSql(
        IServiceProvider services) => @"
            drop index customer_id_idx;
";

}