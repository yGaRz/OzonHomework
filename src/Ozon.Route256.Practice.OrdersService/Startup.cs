using Ozon.Route256.Practice.LogisticGrpcFile;
using Ozon.Route256.Practice.OrdersService.Application;
using Ozon.Route256.Practice.OrdersService.GrpcServices;
using Ozon.Route256.Practice.OrdersService.Infrastructure;
using Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
using Ozon.Route256.Practice.OrdersService.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Kafka.ProducerNewOrder;
using Ozon.Route256.Practice.OrdersService.Kafka.ProducerNewOrder.Handlers;
using Ozon.Route256.Practice.OrdersService.Kafka.ProduserNewOrder;
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

            serviceCollection.AddScoped<IOrderServiceAdapter, OrderServiceAdapter>();
            serviceCollection.AddSingleton<IContractsMapper, ContractsMapper>();

            serviceCollection.AddApplication();
            serviceCollection.AddInfrastructure(_configuration);
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
