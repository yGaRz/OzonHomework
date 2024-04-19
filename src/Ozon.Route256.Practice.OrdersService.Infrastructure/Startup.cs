using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Practice.CustomerGprcFile;
using Ozon.Route256.Practice.OrdersService.Application;
using Ozon.Route256.Practice.OrdersService.Application.Commands.CreateOrder;
using Ozon.Route256.Practice.OrdersService.Application.Commands.UpdateOrderState;
using Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ClientBalancing;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Common;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard.Common;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard.Common.Rules;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Database;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Mappers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository;
using Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.Orders;
using Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.Orders.Repository;
using Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.RegionManager;
using Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.RegionManager.Repository;
using Ozon.Route256.Practice.OrdersService.Infrastructure.RedisCacheCustomers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.RedisCacheCustomers.Redis;
using Ozon.Route256.Practice.SdServiceGrpcFile;
using StackExchange.Redis;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure;

public static class Startup
{
    [Obsolete]
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration _configuration)
    {
        serviceCollection.AddGrpcClient<SdService.SdServiceClient>(option =>
        {
            var url = _configuration.GetValue<string>("ROUTE256_SD_ADDRESS");
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("ROUTE256_SD_ADDRESS variable is null or empty");
            }

            option.Address = new Uri(url);
        });

        serviceCollection.AddScoped<IOrdersServiceReadRepository, OrdersServiceReadRepository>();
        serviceCollection.AddScoped<ICustomerRepositoryAdapter, CustomerServiceAdapter>();
        serviceCollection.AddScoped<IOrdersManager, OrdersManager>();
        serviceCollection.AddScoped<IOrdersRepository, OrdersShardRepositoryPg>();
        serviceCollection.AddScoped<IRegionRepository, RegionShardRepositoryPg>();
        serviceCollection.AddScoped<IRegionDatabase, RegionDatabaseInMemory>();
        serviceCollection.AddScoped<IDataReadMapper, DataLayerMapper>();
        serviceCollection.AddScoped<IDataWriteMapper, DataLayerMapper>();
        serviceCollection.AddScoped<IUnitOfCreateOrder, UnitOfCreateOrder>();
        serviceCollection.AddScoped<IUnitOfUpdateOrder, UnitOfUpdateOrder>();
        PostgresMapping.MapCompositeTypes();
        serviceCollection.Configure<DbOptions>(_configuration.GetSection(nameof(DbOptions)));
        serviceCollection.AddSingleton<IShardPostgresConnectionFactory, ShardConnectionFactory>();
        serviceCollection.AddSingleton<IShardingRule<long>, LongShardingRule>();
        serviceCollection.AddSingleton<IShardingRule<SourceRegion>, SourceRegionShardingRule>();
        serviceCollection.AddSingleton<IShardMigrator, ShardMigrator>();

        serviceCollection.AddHostedService<SdConsumerHostedService>();
        serviceCollection.AddSingleton<IDbStore, DbStore>();

        var redis_url = _configuration.GetValue<string>("ROUTE256_REDIS_ADDRESS");
        if (string.IsNullOrEmpty(redis_url))
            throw new ArgumentException("ROUTE256_REDIS_ADDRESS variable is null or empty");
        serviceCollection.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redis_url));
        serviceCollection.AddScoped<ICacheCustomers, RedisCustomerRepository>();
        serviceCollection.AddScoped<IGrcpCustomerService, GrpcCustomerService>();
        serviceCollection.AddGrpcClient<Customers.CustomersClient>(option =>
        {
            var url = _configuration.GetValue<string>("ROUTE256_CS_ADDRESS");
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("ROUTE256_CS_ADDRESS variable is null or empty");
            }

            option.Address = new Uri(url);
        });

        return serviceCollection;
    }
}
