using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Ozon.Route256.Practice.GatewayService.Infrastructure;

namespace Ozon.Route256.Practice.GatewayService
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

            var factory = new StaticResolverFactory(address => new[]
            {
                new BalancerAddress("localhost", 5191),
                new BalancerAddress("localhost", 5192)
                //new BalancerAddress("orders-service-1", 5005),
                //new BalancerAddress("orders-service-2", 5005)
            });
            serviceCollection.AddSingleton<ResolverFactory>(factory);
            serviceCollection.AddGrpcClient<Orders.OrdersClient>(options =>
            {
                options.Address = new Uri("static:///orders-service");
            }).ConfigureChannel(x =>
            {
                x.Credentials = ChannelCredentials.Insecure;
                x.ServiceConfig = new ServiceConfig
                {
                    LoadBalancingConfigs = { new LoadBalancingConfig("round_robin") }
                };
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
            serviceCollection.AddControllers();
            serviceCollection.AddEndpointsApiExplorer();
            serviceCollection.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseRouting();
            applicationBuilder.UseSwagger();
            applicationBuilder.UseSwaggerUI();
            applicationBuilder.UseHttpsRedirection();
            applicationBuilder.UseEndpoints(endpointRouteBuilder =>
            {
                endpointRouteBuilder.MapControllers();
            });
        }

    }
}