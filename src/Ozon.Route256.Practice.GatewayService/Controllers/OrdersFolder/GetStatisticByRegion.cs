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
            public string? Region { get; init; }
            public int CountOrders {  get; init; }
            public double TotalSum { get; init; }
            public double TotalWigth { get; init; }
            public int CountCustomer { get; init; }
        }

        /// <summary>
        /// Get region statistic
        /// </summary>
        /// <param name="start">Date start</param>
        /// <param name="regions">Region list</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [SwaggerResponse(200, "Orders aggregation", typeof(StatisticByRegion))]
        [SwaggerResponse(400, "Region not found")]
        public async Task<ActionResult<List<StatisticByRegion>>> GetStatisticByRegion(DateTime start,
                                                                                    [FromBody]List<string>? regions,
                                                                                    CancellationToken cancellationToken)
        {
            try
            {
                GetRegionStatisticRequest request = new GetRegionStatisticRequest();
                request.StartTime = Timestamp.FromDateTimeOffset(start);

                if(regions != null) 
                    request.Region.Add(regions);

                var responce = await _ordersClient.GetRegionStatisticAsync(request, null, null, cancellationToken);
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
                    return Ok(result);
                }
                else 
                    return NotFound();
            }
            catch(RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return BadRequest("Region not found");
                else
                    return StatusCode(502);
            }

        }

    }
}