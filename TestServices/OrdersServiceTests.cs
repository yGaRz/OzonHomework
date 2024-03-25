using Grpc.Core;
using Grpc.Core.Utils;
using Moq;
using Ozon.Route256.Practice;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.GrpcServices;
using TestServices.Helpers;

namespace TestServices;

public class OrdersServiceTests
{

    [Fact]
    public async Task Get_Region_From_Repository()
    {
        var mockRegion = new Mock<IRegionRepository>();
        var context = TestServerCallContext.Create();

        mockRegion.Setup(m => m.GetRegionsAsync(context.CancellationToken)).ReturnsAsync(
                            () => {
                                string[] regions = { "Moscow", "StPetersburg", "Novosibirsk"};
                                return regions;
                            });

        var service = new OrdersService(mockRegion.Object,null,null,null);

        var request = new Ozon.Route256.Practice.GetRegionRequest() { };        
        var regionResponce = await service.GetRegion(request, context);
        Assert.NotNull(regionResponce);
        Assert.Contains("Moscow",regionResponce.Region);
        Assert.DoesNotContain("London", regionResponce.Region);
    }

    [Fact]
    public async Task Get_region_from_empty_repository()
    {
        var mockRegion = new Mock<IRegionRepository>();
        var context = TestServerCallContext.Create();

        mockRegion.Setup(m => m.GetRegionsAsync(context.CancellationToken)).ReturnsAsync(
                            () => {
                                string[] regions = { };
                                return regions;
                            });

        var service = new OrdersService(mockRegion.Object, null, null, null);

        var request = new GetRegionRequest() { };
        var regionResponce = await service.GetRegion(request, context);
        Assert.NotNull(regionResponce);
        Assert.DoesNotContain("Moscow", regionResponce.Region);
    }


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
        var service = new OrdersService(null, mockOrders.Object, null, null);

        var request = new GetOrderStatusByIdRequest() { Id = id };

        await Assert.ThrowsAsync<NotFoundException>(()=>  service.GetOrderStatusById(request, context));
    }


    
}
