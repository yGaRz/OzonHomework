using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.DAL.Common;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;

namespace Ozon.Route256.Practice.OrdersService.DAL.Migrations;

[Migration(1, "Create enums migration")]
public class Create_enums : ShardSqlMigration
{
    protected override string GetUpSql(
        IServiceProvider services) => @"
            CREATE TYPE order_source_enum AS ENUM ('WebSite','Mobile','Api');
            CREATE TYPE order_state_enum AS ENUM ('Created','SentToCustomer','Delivered','Lost','Cancelled');

";

    protected override string GetDownSql(
        IServiceProvider services) => @"
            DROP TYPE if exists order_source_enum cascade;
            DROP TYPE if exists order_state_enum cascade;
";

}
