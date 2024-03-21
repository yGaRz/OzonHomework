using Ozon.Route256.Practice.CustomerService.ClientBalancing;
using Ozon.Route256.Practice.OrdersService.Infrastructure;
using Ozon.Route256.Practice.OrdersService.DataAccess;

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

            ///TODO: Убрать тестовые данные.
            RegionRepository regionRepository = new RegionRepository();
            regionRepository.CreateRegionAsync(new DataAccess.Etities.RegionEntity(0, "Moscow"));
            regionRepository.CreateRegionAsync(new DataAccess.Etities.RegionEntity(1, "StPetersburg"));
            regionRepository.CreateRegionAsync(new DataAccess.Etities.RegionEntity(2, "Novosibirsk"));
            serviceCollection.AddScoped<IRegionRepository,RegionRepository>();
            serviceCollection.AddScoped<IOrdersRepository,OrdersRepository>();


            serviceCollection.AddSingleton<IDbStore, DbStore>();
            serviceCollection.AddHostedService<SdConsumerHostedService>();
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
