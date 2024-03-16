using Grpc.Core;

using Microsoft.AspNetCore.Mvc;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    public partial class OrdersController
    {
        /// <summary>
        /// Получение списка регионов
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Список всех регионов или пустой список</returns>
        [HttpGet("[action]")]
        public async Task<ActionResult<List<string>>> GetRegions(CancellationToken cancellationToken)
        {
            try
            {
                GetRegionRequest request = new GetRegionRequest();
                var responce = await _ordersClient.GetRegionAsync(request, null, null, cancellationToken);
                return StatusCode(200, responce.Region.ToList());
            }
            catch
            {
                return StatusCode(502);
            }
        }
    }
}
