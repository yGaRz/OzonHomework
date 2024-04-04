using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.DAL.Common;
namespace Ozon.Route256.Practice.OrdersService.DAL.Migrations;

[Migration(4, "Add region type migrator")]
public class AddRegionType:SqlMigration
{
    protected override string GetUpSql(
        IServiceProvider services) => @"
create type region as
(
    id int,
    name text,
    latitude numeric,
    longitude numeric
);
";

    protected override string GetDownSql(
        IServiceProvider services) => @"
drop type region;
";
}
