using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.DAL.Common;

namespace Ozon.Route256.Practice.OrdersService.DAL.Migrations;

[Migration(2, "Create tables migration")]
public class CreateTables : SqlMigration
{
    protected override string GetUpSql(
        IServiceProvider services) => @"

create table orders(
    id serial primary key,
    customer_id int,
    order_source order_source_enum,
    order_state order_state_enum,
    time_create text,
    time_update text,
    region_id int,
    count_goods int,
    total_weigth numeric,
    total_price numeric,
    address jsonb
);

create table regions(
    id serial primary key,
    region text,
    latitude numeric,
    longitude numeric
);
";

    protected override string GetDownSql(
        IServiceProvider services) => @"

drop table orders;
drop table regions;
";
}