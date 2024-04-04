using Ozon.Route256.Practice.CustomerService.ClientBalancing;
using Ozon.Route256.Practice.OrdersService.Infrastructure;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Google.Protobuf.WellKnownTypes;
using Bogus;
using Ozon.Route256.Practice.OrdersService.DataAccess.Orders;
using StackExchange.Redis;
using Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProduserNewOrder;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder.Handlers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner;
using Ozon.Route256.Practice.OrdersService.DAL.Common;
using Ozon.Route256.Practice.OrdersService.DAL.Repositories;

namespace Ozon.Route256.Practice.OrdersService
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection serviceCollection)
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
            serviceCollection.AddGrpcClient<LogisticsSimulator.Grpc.LogisticsSimulatorService.LogisticsSimulatorServiceClient>(option =>
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

            serviceCollection.AddSwaggerGen();
            serviceCollection.AddGrpcReflection();
            serviceCollection.AddEndpointsApiExplorer();

            var redis_url = _configuration.GetValue<string>("ROUTE256_REDIS_ADDRESS");
            if (string.IsNullOrEmpty(redis_url))
                throw new ArgumentException("ROUTE256_REDIS_ADDRESS variable is null or empty");
            serviceCollection.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redis_url));

            //Репозитории
            var connectionString = _configuration.GetConnectionString("CustomerDatabase");
            if (!string.IsNullOrEmpty(connectionString))
                serviceCollection.AddSingleton<IPostgresConnectionFactory>(_ => new PostgresConnectionFactory(connectionString));
            else
                throw new Exception($"Connection string not found or empty");

            serviceCollection.AddScoped<RegionRepositoryPg>();
            serviceCollection.AddScoped<IRegionRepository,RegionDatabase>();
            serviceCollection.AddScoped<IOrdersRepository,OrdersRepository>();
            //Редис
            serviceCollection.AddScoped<ICacheCustomers, RedisCustomerRepository>();
            serviceCollection.AddScoped<IGrcpCustomerService, Infrastructure.CacheCustomers.GrpcCustomerService>();
            //Кафка
            var kafka_url = _configuration.GetValue<string>("ROUTE256_KAFKA_ADDRESS");
            if (string.IsNullOrEmpty(redis_url))
                throw new ArgumentException("ROUTE256_KAFKA_ADDRESS variable is null or empty");

            serviceCollection.AddSingleton<IKafkaProducer<long,string>, KafkaProducerProvider>(x=>
                    new KafkaProducerProvider(x.GetRequiredService<ILogger<KafkaProducerProvider>>(),kafka_url));

            serviceCollection.AddSingleton<IOrderProducer, OrderProducer>();
            serviceCollection.AddScoped<IAddOrderHandler, AddOrderHandler>();
            serviceCollection.AddScoped<ISetOrderStateHandler, SetOrderStateHandler>();

            serviceCollection.AddSingleton<KafkaPreOrderProvider>(x =>
                new KafkaPreOrderProvider(x.GetRequiredService<ILogger<KafkaPreOrderProvider>>(), kafka_url));
            serviceCollection.AddHostedService<ConsumerKafkaPreOrder>();

            serviceCollection.AddSingleton< KafkaOrdersEventsProvider>(x =>
                new KafkaOrdersEventsProvider(x.GetRequiredService<ILogger<KafkaOrdersEventsProvider>>(), kafka_url));
            serviceCollection.AddHostedService<ConsumerKafkaOrdersEvents>();
            //service-discovery
            serviceCollection.AddSingleton<IDbStore, DbStore>();
            serviceCollection.AddHostedService<SdConsumerHostedService>();

            //fluent-migration
            serviceCollection.AddFluentMigratorCore()
                .ConfigureRunner(
                    builder => builder
                        .AddPostgres()
                        .ScanIn(GetType().Assembly)
                        .For.Migrations())
                .AddOptions<ProcessorOptions>()
                .Configure(
                    options =>
                    {
                        options.ProviderSwitches = "Force Quote=false";
                        options.Timeout = TimeSpan.FromMinutes(10);
                        options.ConnectionString = connectionString;
                    });
         
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
    }





}
