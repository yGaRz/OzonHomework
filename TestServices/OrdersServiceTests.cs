using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Utils;
using Moq;
using Ozon.Route256.Practice;
using Ozon.Route256.Practice.LogisticsSimulator.Grpc;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.DataAccess.Orders;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.GrpcServices;
using Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Data;
using TestServices.Helpers;

namespace TestServices;

public class OrdersServiceTests
{

    //[Fact]
    //public async Task Get_Region_From_Repository()
    //{
    //    var mockRegion = new Mock<IRegionDatabase>();
    //    var context = TestServerCallContext.Create();
    //    var lst = Array.Empty<string>();
    //    mockRegion.Setup(m => m.GetRegionsEntityByNameAsync(lst, context.CancellationToken).ReturnsAsync(
    //                        () => {
    //                            List<RegionEntity> res = new List<RegionEntity>();

    //                            return res.ToArray();
    //                        }));

    //    var service = new OrdersService(mockRegion.Object,null,null,null);

    //    var request = new Ozon.Route256.Practice.GetRegionRequest() { };        
    //    var regionResponce = await service.GetRegion(request, context);
    //    Assert.NotNull(regionResponce);
    //    Assert.Contains("Moscow",regionResponce.Region);
    //    Assert.DoesNotContain("London", regionResponce.Region);
    //}
    //[Fact]
    //public async Task Get_region_from_empty_repository()
    //{
    //    var mockRegion = new Mock<IRegionDatabase>();
    //    var context = TestServerCallContext.Create();

    //    mockRegion.Setup(m => m.GetRegionsAsync(context.CancellationToken)).ReturnsAsync(
    //                        () => {
    //                            string[] regions = { };
    //                            return regions;
    //                        });

    //    var service = new OrdersService(mockRegion.Object, null, null, null);

    //    var request = new GetRegionRequest() { };
    //    var regionResponce = await service.GetRegion(request, context);
    //    Assert.NotNull(regionResponce);
    //    Assert.DoesNotContain("Moscow", regionResponce.Region);
    //}

    [Fact]
    public async Task Get_Order_Status_Success()
    {
        var mockOrders = new Mock<IOrdersRepository>();
        var context = TestServerCallContext.Create();
        int idOrder = 1;
        mockOrders.Setup(m => m.GetOrderByIdAsync(idOrder, context.CancellationToken)).ReturnsAsync(
            () =>  { return OrderGenerator.GenerateOrder(1, idOrder); });
        var service = new OrdersService(null, mockOrders.Object, null, null);
        var request = new GetOrderStatusByIdRequest() { Id = idOrder };


        var status = await service.GetOrderStatusById(request, context);


        Assert.NotNull(status);
        Assert.True(status.LogisticStatus == OrderState.Created);
    }
        [Fact]
    public async Task Get_Order_Status_NotFound()
    {
        var mockOrders = new Mock<IOrdersRepository>();
        var context = TestServerCallContext.Create();
        long id = 1;
        mockOrders.Setup(m => m.GetOrderByIdAsync(id, context.CancellationToken)).ThrowsAsync(new NotFoundException($"Заказ с номером {id} не найден"));
        var service = new OrdersService(null, mockOrders.Object, null,  null);

        var request = new GetOrderStatusByIdRequest() { Id = id };

        await Assert.ThrowsAsync<NotFoundException>(()=>  service.GetOrderStatusById(request, context));
    }

    [Fact]
    public async Task Exception_Cancel_Order_Not_Found()
    {
        var mockOrders = new Mock<IOrdersRepository>();
        var context = TestServerCallContext.Create();
        long id = 1;
        mockOrders.Setup(m => m.GetOrderByIdAsync(id, context.CancellationToken)).ThrowsAsync(new NotFoundException($"Заказ с номером {id} не найден"));
        var service = new OrdersService(null, mockOrders.Object, null, null);

        var request = new CancelOrderByIdRequest() { Id=id };

        await Assert.ThrowsAsync<NotFoundException>(() => service.CancelOrder(request, context));
    }

    [Fact]
    public async Task Exception_Cancel_Order_Logistic_Exception()
    {
        // Arrange
        var mockOrders = new Mock<IOrdersRepository>();
        var context = TestServerCallContext.Create();
        long idOrder = 1;
        mockOrders.Setup(m => m.GetOrderByIdAsync(idOrder, context.CancellationToken)).ReturnsAsync(() => {
            var address = new Address()
            {
                Apartment = "",
                Building = "",
                City = "",
                Latitude = 1,
                Longitude = 1,
                Region = "Moscow",
                Street = ""

            };
            return new OrderEntity(1, OrderSourceEnum.Api, OrderStateEnum.Created, 1, address, new List<ProductEntity>());
        });
        var mockCall = CallHelpers.CreateAsyncUnaryCall(new CancelResult{ Success = false, Error = "some text" });
        var mockLogistic = new Mock<LogisticsSimulatorService.LogisticsSimulatorServiceClient>();

        mockLogistic.Setup(m => m.OrderCancelAsync(new Ozon.Route256.Practice.LogisticsSimulator.Grpc.Order() { Id = idOrder }, null, null, context.CancellationToken)).Returns(mockCall);
        var service = new OrdersService(null, mockOrders.Object, mockLogistic.Object, null);
        var request = new CancelOrderByIdRequest() { Id = idOrder };

        await Assert.ThrowsAsync<RpcException>(() => service.CancelOrder(request, context));
    }

    [Fact]
    public async Task Exception_Cancel_Order_Logistic_Success()
    {
        // Arrange
        var mockOrders = new Mock<IOrdersRepository>();
        var context = TestServerCallContext.Create();
        long idOrder = 1;
        mockOrders.Setup(m => m.GetOrderByIdAsync(idOrder, context.CancellationToken)).ReturnsAsync(() => {
            var address = new Address()
            {
                Apartment = "",
                Building = "",
                City = "",
                Latitude = 1,
                Longitude = 1,
                Region = "Moscow",
                Street = ""

            };
            return new OrderEntity(1, OrderSourceEnum.Api, OrderStateEnum.Created, 1, address, new List<ProductEntity>());
        });
        var mockCall = CallHelpers.CreateAsyncUnaryCall(new CancelResult { Success = true, Error = "" });
        var mockLogistic = new Mock<LogisticsSimulatorService.LogisticsSimulatorServiceClient>();

        mockLogistic.Setup(m => m.OrderCancelAsync(new Ozon.Route256.Practice.LogisticsSimulator.Grpc.Order() { Id = idOrder }, null, null, context.CancellationToken)).Returns(mockCall);
        var service = new OrdersService(null, mockOrders.Object, mockLogistic.Object, null);
        var request = new CancelOrderByIdRequest() { Id = idOrder };

        //Act
        var responce = await service.CancelOrder(request, context);

        //Assert
        Assert.NotNull(responce);
    }

    [Fact]
    public void Get_Get_Customer_Orders_Customer_NotFound()
    {
        var mockCustomer = new Mock<IGrcpCustomerService>();
        var context = TestServerCallContext.Create();
        int id = 1;
        mockCustomer.Setup(m => m.GetCustomer(id, context.CancellationToken)).Throws(new RpcException(new Status(StatusCode.InvalidArgument, $"Клиент с id={id} не найден")));

        var request = new GetOrdersByCustomerIDRequest() { Id = id, PageIndex = 1, PageSize = 20, StartTime = DateTime.Now.ToUniversalTime().ToTimestamp() };
        var service = new OrdersService(null, null, null, mockCustomer.Object);

        Assert.ThrowsAsync<RpcException>(() => service.GetOrdersByCustomerID(request, context));
    }
    //[Fact]
    //public async void Get_Get_Customer_Orders_Customer_Responce_Valid()
    //{
    //    var mockCustomer = new Mock<IGrcpCustomerService>();
    //    var context = TestServerCallContext.Create();
    //    int id = 1;
    //    mockCustomer.Setup(m => m.GetCustomer(id, context.CancellationToken)).ReturnsAsync(() =>
    //    {
    //        return new CustomerEntity()
    //        {
    //            Id = id,
    //            FirstName= "Name",
    //            LastName= "Surname",
    //            Email="xxx@yyy.ru",
    //            DefaultAddress = new AddressEntity("","","","","",1,1),
    //            Phone=""
    //        };
    //    }
    //        );
    //    var request = new GetOrdersByCustomerIDRequest() { Id = id, PageIndex = 1, PageSize = 20, StartTime = DateTime.Now.ToUniversalTime().ToTimestamp() };
    //    IOrdersRepository rep = new OrdersDatabase();

    //    var service = new OrdersService(null, rep, null, mockCustomer.Object);

    //    var responce =await service.GetOrdersByCustomerID(request,context);

    //    Assert.NotNull(responce);   

    //    //Assert.ThrowsAsync<RpcException>(async () => { await service.GetOrdersByCustomerID(request, context); });
    //}
}
