using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.DAL.Common;

namespace Ozon.Route256.Practice.OrdersService.DAL.Migrations;

[Migration(1, "Create enums migration")]
public class Create_enums : SqlMigration
{
    protected override string GetUpSql(
        IServiceProvider services) => @"
                        CREATE TYPE order_source_enum AS ENUM ('WebSite','Mobile','Api');
                        CREATE TYPE order_state_enum AS ENUM ('Created','SentToCustomer','Delivered','Lost','Cancelled');
";

    protected override string GetDownSql(
        IServiceProvider services) => @"
                        DROP TYPE order_source_enum;
                        DROP TYPE order_state_enum;
";

}
