using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Ozon.Route256.Practice.LogisticGrpcFile;
using Ozon.Route256.Practice.OrdersService.Application;
using Ozon.Route256.Practice.OrdersService.GrpcServices;
using Ozon.Route256.Practice.OrdersService.Infrastructure;
using Ozon.Route256.Practice.OrdersService.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Kafka.KafkaProducerNewOrder;
using Ozon.Route256.Practice.OrdersService.Kafka.ProducerNewOrder.Handlers;
using Serilog;
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
            Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(_configuration)
                    .Enrich.WithMemoryUsage()
                    .CreateLogger();
            serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            AddGrpc(serviceCollection);
            serviceCollection.AddSwaggerGen();
            serviceCollection.AddEndpointsApiExplorer();
            serviceCollection.AddScoped<IKafkaAdapter, KafkaAdapter>();
            serviceCollection.AddScoped<IOrderServiceAdapter, OrderServiceAdapter>();
            serviceCollection.AddSingleton<IContractsMapper, ContractsMapper>();

            serviceCollection.AddApplication();
            serviceCollection.AddInfrastructure(_configuration);
            AddKafka(serviceCollection);

            serviceCollection.AddOpenTelemetry()
                        .WithTracing(builder => builder
                            .AddAspNetCoreInstrumentation()
                            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(nameof(OrdersService)))
                            .AddNpgsql()
                            .AddSource("Create order activity")
                            .AddSource("Update order activity")
                            //.AddSource("Grpc Interceptor")
                            .AddJaegerExporter(options =>
                            {
                                options.AgentHost = "localhost";
                                options.AgentPort = 6831;
                                options.Protocol = JaegerExportProtocol.UdpCompactThrift;
                                options.ExportProcessorType = ExportProcessorType.Simple;
                            }));
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
