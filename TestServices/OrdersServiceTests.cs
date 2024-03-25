using Grpc.Core;
using Grpc.Core.Utils;
using Moq;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.GrpcServices;
using TestServices.Helpers;

namespace TestServices;

public class OrdersServiceTests
{

    [Fact]
    public async Task TestGetReion()
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
    public async Task TestGetEmptyReion()
    {
        var mockRegion = new Mock<IRegionRepository>();
        var context = TestServerCallContext.Create();

        mockRegion.Setup(m => m.GetRegionsAsync(context.CancellationToken)).ReturnsAsync(
                            () => {
                                string[] regions = { };
                                return regions;
                            });

        var service = new OrdersService(mockRegion.Object, null, null, null);

        var request = new Ozon.Route256.Practice.GetRegionRequest() { };
        var regionResponce = await service.GetRegion(request, context);
        Assert.NotNull(regionResponce);
        Assert.DoesNotContain("Moscow", regionResponce.Region);
    }


}
