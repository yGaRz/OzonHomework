using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{

    public partial class OrdersController
    {
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
        public async Task<ActionResult<GetOrdersByRegionResponse>> GetOrderByRegion(DateTime start,
            List<string>? regions,
            CancellationToken cancellationToken)
        {
            try
            {
                GetOrdersByRegionRequest request = new GetOrdersByRegionRequest();
                request.StartTime = Timestamp.FromDateTimeOffset(start);
                if (regions != null)
                    request.Region.Add(regions);
                var responce = await _ordersClient.GetOrdersByRegionAsync(request, null, null, cancellationToken);
                if (responce != null)
                    return StatusCode(200, responce);
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