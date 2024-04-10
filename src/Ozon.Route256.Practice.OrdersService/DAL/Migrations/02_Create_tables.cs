using FluentMigrator;
using Ozon.Route256.Practice.OrdersService.DAL.Common;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;

namespace Ozon.Route256.Practice.OrdersService.DAL.Migrations;

[Migration(2, "Create tables migration")]
public class CreateTables : ShardSqlMigration
{
    protected override string GetUpSql(
        IServiceProvider services) => @"

create table if not exists orders(
    id serial primary key,
    customer_id int,
    order_source order_source_enum,
    order_state order_state_enum,
    time_create timestamp,
    time_update timestamp,
    region_id int not null check(region_id>0),
    count_goods int not null check(count_goods>0),
    total_weigth numeric not null check(total_weigth>0),
    total_price numeric not null check(total_price>0),
    address jsonb
);

create table if not exists regions(
    id serial primary key,
    region text,
    latitude numeric,
    longitude numeric
);
";

    protected override string GetDownSql(
        IServiceProvider services) => @"

drop table if exists orders;
drop table if exists  regions;
";
}