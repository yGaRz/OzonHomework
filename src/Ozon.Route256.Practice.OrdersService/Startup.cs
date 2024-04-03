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

namespace Ozon.Route256.Practice.OrdersService
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async void ConfigureServices(IServiceCollection serviceCollection)
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



            serviceCollection.AddScoped<IRegionRepository,RegionRepository>();
            serviceCollection.AddScoped<IOrdersRepository,OrdersRepository>();

            serviceCollection.AddScoped<ICacheCustomers, RedisCustomerRepository>();
            serviceCollection.AddScoped<IGrcpCustomerService, Infrastructure.CacheCustomers.GrpcCustomerService>();

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

            serviceCollection.AddSingleton<IDbStore, DbStore>();
            serviceCollection.AddHostedService<SdConsumerHostedService>();

            var connectionString = _configuration.GetConnectionString("CustomerDatabase");
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
            
            //PostgresMapping.MapCompositeTypes();

            await GenerateRegionAsync(serviceCollection);
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

        private static async Task GenerateRegionAsync(IServiceCollection services)
        {
            RegionRepository regionRepository = new RegionRepository();
            await regionRepository.CreateRegionAsync(new DataAccess.Etities.RegionEntity(0, "Moscow",55.72,37.65));
            await regionRepository.CreateRegionAsync(new DataAccess.Etities.RegionEntity(1, "StPetersburg",59.88,82.55));
            await regionRepository.CreateRegionAsync(new DataAccess.Etities.RegionEntity(2, "Novosibirsk",55.01,82.55));
        }


    }





}
