using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard.Common;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Migrations;


[Migration(5, "Create enums migration")]
public class OrderSourceIndex : ShardSqlMigration
{
    protected override string GetUpSql(
        IServiceProvider services) => @"
        create table if not exists idx_order_source
        (
            source  order_source_enum NOT NULL,
            region_id int NOT NULL,
            order_id int  NOT NULL,
            PRIMARY KEY (source, region_id , order_id)
        );

";

    protected override string GetDownSql(
        IServiceProvider services) => @"
    drop table if exists idx_order_source;
";

}
