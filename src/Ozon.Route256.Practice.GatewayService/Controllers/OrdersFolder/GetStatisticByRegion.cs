using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Practice.GatewayService.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{

    public partial class OrdersController
    {
        public record StatisticByRegion()
        {
            public string? Region;
            public int CountOrders;
            public double TotalSum;
            public double TotalWigth;
            public int CountCustomer;
        }

        /// <summary>
        /// Get region statistic
        /// </summary>
        /// <param name="start">Date start</param>
        /// <param name="regions">Region list</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [SwaggerResponse(200, "Orders aggregation", typeof(OrdersListModel))]
        [SwaggerResponse(400, "Region not found")]
        public async Task<ActionResult<List<StatisticByRegion>>> GetStatisticByRegion(DateTime start,
                                                                                    [FromBody]List<string>? regions,
                                                                                    CancellationToken cancellationToken)
        {
            try
            {
                GetOrdersByRegionRequest request = new GetOrdersByRegionRequest();
                request.StartTime = Timestamp.FromDateTimeOffset(start);

                if(regions != null) 
                    request.Region.Add(regions);

                var responce = await _ordersClient.GetOrdersByRegionAsync(request, null, null, cancellationToken);
                if (responce != null)
                {
                    List<StatisticByRegion> result = new List<StatisticByRegion>();
                    foreach (var a in responce.Statistic)
                        result.Add(new StatisticByRegion()
                        {
                            CountCustomer = a.CountCustomer,
                            Region = a.Region,
                            CountOrders = a.TotalCountOrders,
                            TotalSum = a.TotalSumOrders,
                            TotalWigth = a.TotalWightOrders
                        });
                    return StatusCode(200, result);
                }
                else 
                    return NotFound();
            }
            catch(RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return StatusCode(400, "Region not found");
                else
                    return StatusCode(502);
            }

        }

    }
}