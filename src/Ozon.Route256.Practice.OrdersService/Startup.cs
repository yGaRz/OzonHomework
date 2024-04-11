using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Ozon.Route256.Practice.CustomerGprcFile;
using Ozon.Route256.Practice.LogisticGrpcFile;
using Ozon.Route256.Practice.OrdersService.DAL.Common;
using Ozon.Route256.Practice.OrdersService.DAL.Repositories;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.DataAccess.Orders;
using Ozon.Route256.Practice.OrdersService.Infrastructure;
using Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder.Handlers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProduserNewOrder;
using Ozon.Route256.Practice.SdServiceGrpcFile;
using StackExchange.Redis;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;
using Ozon.Route256.Practice.OrdersService.ClientBalancing;
using Ozon.Route256.Practice.Orders.ClientBalancing;
using Ozon.Route256.Practice.OrdersService.DAL.Repositories.ShardRepository;

namespace Ozon.Route256.Practice.OrdersService
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Obsolete]
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            AddGrpc(serviceCollection);
            serviceCollection.AddSwaggerGen();
            serviceCollection.AddEndpointsApiExplorer();
            AddServiceDiscovery(serviceCollection);
            //Репозитории-----------------------------------------------------------

            PostgresMapping.MapCompositeTypes();
            //var connectionString = _configuration.GetConnectionString("OrdersManagerPg");
            //if (!string.IsNullOrEmpty(connectionString))
            //    serviceCollection.AddSingleton<IPostgresConnectionFactory>(_ => new PostgresConnectionFactory(connectionString));
            //else
            //    throw new Exception($"Connection string not found or empty");

            serviceCollection.Configure<DbOptions>(_configuration.GetSection(nameof(DbOptions)));
            serviceCollection.AddSingleton<IShardPostgresConnectionFactory, ShardConnectionFactory>();
            serviceCollection.AddSingleton<IShardingRule<long>, LongShardingRule>();
            serviceCollection.AddSingleton<IShardMigrator, ShardMigrator>();

            serviceCollection.AddScoped<IRegionRepository, RegionShardRepositoryPg>();
            serviceCollection.AddScoped<IRegionDatabase, RegionDatabaseInMemory>();
            serviceCollection.Configure<IRegionDatabase>(x => x.Update());
            serviceCollection.AddScoped<IOrdersRepository, OrdersShardRepositoryPg>();
            serviceCollection.AddScoped<IOrdersManager, OrdersManagerPg>(); 

            AddRedis(serviceCollection);
            AddKafka(serviceCollection);
        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseRouting();
            applicationBuilder.UseSwagger();
            applicationBuilder.UseSwaggerUI();            

            applicationBuilder.UseEndpoints(endpointRouteBuilder =>
            {
                endpointRouteBuilder.MapGrpcService<GrpcServices.OrdersService>();
                endpointRouteBuilder.MapGrpcReflectionService();
            });
        }
        void AddKafka(IServiceCollection serviceCollection)
        {
            var kafka_url = _configuration.GetValue<string>("ROUTE256_KAFKA_ADDRESS");
            if (string.IsNullOrEmpty(kafka_url))
                throw new ArgumentException("ROUTE256_KAFKA_ADDRESS variable is null or empty");

            serviceCollection.AddSingleton<IKafkaProducer<long, string>, KafkaProducerProvider>(x =>
                    new KafkaProducerProvider(x.GetRequiredService<ILogger<KafkaProducerProvider>>(), kafka_url));

            serviceCollection.AddSingleton<IOrderProducer, OrderProducer>();
            serviceCollection.AddScoped<IAddOrderHandler, AddOrderHandler>();
            serviceCollection.AddScoped<ISetOrderStateHandler, SetOrderStateHandler>();

            serviceCollection.AddSingleton<KafkaPreOrderProvider>(x =>
                new KafkaPreOrderProvider(x.GetRequiredService<ILogger<KafkaPreOrderProvider>>(), kafka_url));
            serviceCollection.AddHostedService<ConsumerKafkaPreOrder>();

            serviceCollection.AddSingleton<KafkaOrdersEventsProvider>(x =>
                new KafkaOrdersEventsProvider(x.GetRequiredService<ILogger<KafkaOrdersEventsProvider>>(), kafka_url));
            serviceCollection.AddHostedService<ConsumerKafkaOrdersEvents>();
        }
        void AddRedis(IServiceCollection serviceCollection)
        {
            var redis_url = _configuration.GetValue<string>("ROUTE256_REDIS_ADDRESS");
            if (string.IsNullOrEmpty(redis_url))
                throw new ArgumentException("ROUTE256_REDIS_ADDRESS variable is null or empty");
            serviceCollection.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redis_url));
            serviceCollection.AddScoped<ICacheCustomers, RedisCustomerRepository>();
            serviceCollection.AddScoped<IGrcpCustomerService, GrpcCustomerService>();
        }
        void AddGrpc(IServiceCollection serviceCollection)
        {
            serviceCollection.AddGrpc(option => option.Interceptors.Add<LoggerInterceptor>());
            serviceCollection.AddGrpcClient<SdService.SdServiceClient>(option =>
            {
                var url = _configuration.GetValue<string>("ROUTE256_SD_ADDRESS");
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentException("ROUTE256_SD_ADDRESS variable is null or empty");
                }

                option.Address = new Uri(url);
            });
            serviceCollection.AddGrpcClient<LogisticsSimulatorService.LogisticsSimulatorServiceClient>(option =>
            {
                var url = _configuration.GetValue<string>("ROUTE256_LS_ADDRESS");
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentException("ROUTE256_LS_ADDRESS variable is null or empty");
                }

                option.Address = new Uri(url);
            });
            serviceCollection.AddGrpcClient<Customers.CustomersClient>(option =>
            {
                var url = _configuration.GetValue<string>("ROUTE256_CS_ADDRESS");
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentException("ROUTE256_CS_ADDRESS variable is null or empty");
                }

                option.Address = new Uri(url);
            });
            serviceCollection.AddGrpcReflection();
        }
        void AddServiceDiscovery(IServiceCollection serviceCollection)
        {
            serviceCollection.AddHostedService<SdConsumerHostedService>();
            serviceCollection.AddSingleton<IDbStore, DbStore>();
        }
    }





}
