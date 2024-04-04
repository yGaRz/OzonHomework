using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.DAL.Common;
using Ozon.Route256.Practice.OrdersService.DataAccess;

namespace Ozon.Route256.Practice.OrdersService.DAL.Migrations;

[Migration(3, "Add some regions migration")]
public class AddRegions : SqlMigration
{
    protected override string GetUpSql(
        IServiceProvider services) => @"
            INSERT INTO regions (id, region, latitude, longitude)
            VALUES (0, 'Moscow', 55.72, 37.65);
            INSERT INTO regions (id, region, latitude, longitude)
            VALUES (1, 'StPetersburg', 59.88, 30.40);
            INSERT INTO regions (id, region, latitude, longitude)
            VALUES (2, 'Novosibirsk', 55.01, 82.55);
";

    protected override string GetDownSql(
        IServiceProvider services) => @"
truncate table regions;
";
}

