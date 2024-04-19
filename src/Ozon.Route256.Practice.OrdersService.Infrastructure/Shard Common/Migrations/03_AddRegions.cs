using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard.Common;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Migrations;

[Migration(3, "Add some regions migration")]
public class AddRegions : ShardSqlMigration
{
    protected override string GetUpSql(
        IServiceProvider services) => @"
    INSERT INTO regions(region, latitude, longitude)
            VALUES('Moscow', 55.72, 37.65);
    INSERT INTO regions(region, latitude, longitude)
            VALUES('StPetersburg', 59.88, 30.40);
    INSERT INTO regions( region, latitude, longitude)
            VALUES('Novosibirsk', 55.01, 82.55);
";

    protected override string GetDownSql(
        IServiceProvider services) => @"
    truncate table regions;
";
}

