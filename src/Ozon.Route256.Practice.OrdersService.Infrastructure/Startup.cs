using FirebirdSql.Data.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ClientBalancing;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Common;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard.Common;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard.Common.Rules;
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

        return serviceCollection;
    }
}
