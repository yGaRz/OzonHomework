﻿using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Ozon.Route256.Practice.CustomerGprcFile;
using Ozon.Route256.Practice.GatewayService.Infrastructure;
using Ozon.Route256.Practice.OrdersGrpcFile;

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
                new BalancerAddress(_configuration.GetValue<string>("ROUTE256_OS1_ADDRESS"),
                                                            int.Parse(_configuration.GetValue<string>("ROUTE256_OS1_PORT"))),
                new BalancerAddress(_configuration.GetValue<string>("ROUTE256_OS2_ADDRESS"),
                                                            int.Parse(_configuration.GetValue<string>("ROUTE256_OS2_PORT"))),
                 new BalancerAddress(_configuration.GetValue<string>("ROUTE256_OS3_ADDRESS"),
                                                            int.Parse(_configuration.GetValue<string>("ROUTE256_OS3_PORT"))) 
                //new BalancerAddress("localhost", 5191),
                //new BalancerAddress("localhost", 5192),                
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
            serviceCollection.AddSwaggerGen(options=>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Ozon.Route256.RestAPI",
                    Description = "Nosov Arsenij homework"
                });
                options.EnableAnnotations();
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Ozon.Route256.Practice.GatewayService.xml");
                options.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseRouting();
            applicationBuilder.UseSwagger();
            applicationBuilder.UseSwaggerUI();
            //applicationBuilder.UseHttpsRedirection();
            applicationBuilder.UseEndpoints(endpointRouteBuilder =>
            {
                endpointRouteBuilder.MapControllers();
            });
        }

    }
}