using Ozon.Route256.Practice.LogisticGrpcFile;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.DataAccess.Orders;
using Ozon.Route256.Practice.OrdersService.GrpcServices;
using Ozon.Route256.Practice.OrdersService.Infrastructure;
using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Repositories;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder.Handlers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProduserNewOrder;
using System.Reflection;

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
            serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            AddGrpc(serviceCollection);
            serviceCollection.AddSwaggerGen();
            serviceCollection.AddEndpointsApiExplorer();
            //Репозитории-----------------------------------------------------------
            serviceCollection.AddInfrastructure(_configuration);

            serviceCollection.AddScoped<Infrastructure.DAL.Repositories.IRegionRepository, RegionShardRepositoryPg>();
            serviceCollection.AddScoped<DataAccess.IRegionDatabase, RegionDatabaseInMemory>();
            serviceCollection.Configure((DataAccess.IRegionDatabase x) => x.Update());
            serviceCollection.AddScoped<IOrdersRepository, OrdersShardRepositoryPg>();
            serviceCollection.AddScoped<IOrdersManager, OrdersManager>(); 

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
            serviceCollection.AddScoped<ISetOrderStateHandler, SetOrderStateHandler>();
            serviceCollection.AddScoped<IAddOrderHandler, AddOrderHandler>();
            serviceCollection.AddScoped<ISetOrderStateHandler, SetOrderStateHandler>();

            serviceCollection.AddSingleton<KafkaPreOrderProvider>(x =>
                new KafkaPreOrderProvider(x.GetRequiredService<ILogger<KafkaPreOrderProvider>>(), kafka_url));
            serviceCollection.AddHostedService<ConsumerKafkaPreOrder>();

            serviceCollection.AddSingleton<KafkaOrdersEventsProvider>(x =>
                new KafkaOrdersEventsProvider(x.GetRequiredService<ILogger<KafkaOrdersEventsProvider>>(), kafka_url));
            serviceCollection.AddHostedService<ConsumerKafkaOrdersEvents>();
        }

        void AddGrpc(IServiceCollection serviceCollection)
        {
            serviceCollection.AddGrpc(option => option.Interceptors.Add<LoggerInterceptor>());
            serviceCollection.AddGrpcClient<LogisticsSimulatorService.LogisticsSimulatorServiceClient>(option =>
            {
                var url = _configuration.GetValue<string>("ROUTE256_LS_ADDRESS");
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentException("ROUTE256_LS_ADDRESS variable is null or empty");
                }

                option.Address = new Uri(url);
            });
            serviceCollection.AddGrpcReflection();
        }
    }





}
