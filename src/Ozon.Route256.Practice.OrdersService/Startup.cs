﻿using Ozon.Route256.Practice.CustomerService.ClientBalancing;
using Ozon.Route256.Practice.OrdersService.Infrastructure;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Google.Protobuf.WellKnownTypes;
using Bogus;
using Ozon.Route256.Practice.OrdersService.DataAccess.Orders;
using StackExchange.Redis;
using Ozon.Route256.Practice.OrdersService.DataAccess.CacheCustomers;

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
            _ = GenerateTestDataAsync();

            serviceCollection.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6380"));
            serviceCollection.AddScoped<IRegionRepository,RegionRepository>();
            serviceCollection.AddScoped<IOrdersRepository,OrdersRepository>();
            serviceCollection.AddScoped<ICacheCustomers, RedisCustomerRepository>();






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

        private static async Task GenerateTestDataAsync()
        {
            RegionRepository regionRepository = new RegionRepository();
            await regionRepository.CreateRegionAsync(new DataAccess.Etities.RegionEntity(0, "Moscow"));
            await regionRepository.CreateRegionAsync(new DataAccess.Etities.RegionEntity(1, "StPetersburg"));
            await regionRepository.CreateRegionAsync(new DataAccess.Etities.RegionEntity(2, "Novosibirsk"));

            OrdersRepository ordersRepository = new OrdersRepository();
            Random rand = new Random(0);
            List<CustomerEntity> customerEntities = new List<CustomerEntity>();
            Faker faker = new Faker();

            for(int i=1;i<=6;i++)
            {
                string regionName  = await regionRepository.GetNameByIdRegionAsync(i % 3);
                AddressEntity address = new AddressEntity(regionName,
                                                                faker.Address.City(),
                                                                faker.Address.StreetName(),
                                                                faker.Address.BuildingNumber(),
                                                                faker.Address.StreetSuffix(),
                                                                faker.Address.Latitude(),
                                                                faker.Address.Longitude());               
                CustomerEntity cusromer = new CustomerEntity()
                {
                    Id = i,
                    DefaultAddress = address
                };
                customerEntities.Add(cusromer);
            }


            for (int i=1;i<10;i++)
            {
                List<ProductEntity> goods = new List<ProductEntity>();
                int cnt = rand.Next(1, 3);
                for (int j=0;j<cnt;j++)
                {
                    ProductEntity good = new ProductEntity(faker.Random.Int(0,555555),
                        faker.Commerce.Product(),
                        faker.Random.Int(1,5),
                        faker.Random.Double(1,500),
                        faker.Random.UInt(1,20)
                        );
                    goods.Add(good);
                }
                int rnd = rand.Next(1, 3);
                OrderEntity order = new OrderEntity(i, Models.OrderSourceEnum.WebSite, Models.OrderStateEnum.Created, customerEntities[rnd].Id, customerEntities[rnd].DefaultAddress, goods);
                await ordersRepository.CreateOrderAsync(order);
            }



        }


    }





}
