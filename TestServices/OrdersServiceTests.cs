using Grpc.Core;
using Grpc.Core.Utils;
using Moq;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.GrpcServices;
using TestServices.Helpers;

namespace TestServices;

public class OrdersServiceTests
{
    //public override async Task<GetRegionResponse> GetRegion(GetRegionRequest request, ServerCallContext context)
    //{
    //    var regions = await _regionRepository.GetRegionsAsync();
    //    var result = new GetRegionResponse
    //    {
    //        Region = { regions.ToArray() }
    //    };
    //    return result;
    //}

    [Fact]
    public async Task TestGetReion1()
    {
        var mockRegion = new Mock<IRegionRepository>();
        var context = TestServerCallContext.Create();

        mockRegion.Setup(m => m.GetRegionsAsync(context.CancellationToken)).ReturnsAsync(
                            (string[] s) => {
                                string[] regions = { "Moscow", "StPetersburg", "Novosibirsk"};
                                return regions;
                            });

        var service = new OrdersService(mockRegion.Object,null,null,null);

        var request = new Ozon.Route256.Practice.GetRegionRequest() { };
        
        var regionResponce = await service.GetRegion(request, context);
        Assert.Contains("Moscow",regionResponce.Region);

    }

}
