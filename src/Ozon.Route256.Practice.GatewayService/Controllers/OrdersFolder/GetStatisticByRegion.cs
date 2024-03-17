using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{

    public partial class OrdersController
    {
        public record StatisticByRegion()
        {
            public string Region;
            public int CountOrders;
            public double TotalSum;
            public double TotalWigth;
            public int CountCustomer;
        }

        /// <summary>
        /// Запрос списка заказов с агрегацией по региону
        /// </summary>
        /// <param name="start">Дата/Время от которой строить агрецгацию</param>
        /// <param name="regions">Список регионов по которым нужна агрегация</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<StatisticByRegion>>> GetStatisticByRegion(DateTime start,
                                                                                    [FromBody]List<string>? regions,
                                                                                    CancellationToken cancellationToken)
        {
            try
            {
                if (start > DateTime.Now)
                    return StatusCode(400, "Некорректно указана дата и время поиска заказов");

                GetOrdersByRegionRequest request = new GetOrdersByRegionRequest();
                request.StartTime = Timestamp.FromDateTimeOffset(start);

                if(regions != null) 
                    foreach (var a in regions)
                        request.Region.Add(new Region() { NameRegion = a });

                var responce = await _ordersClient.GetOrdersByRegionAsync(request, null, null, cancellationToken);
                if (responce != null)
                else 
                    return NotFound();
            }
            catch(RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return StatusCode(400, "Регион не найден");
                else
                    return StatusCode(502);
            }

        }

    }
}