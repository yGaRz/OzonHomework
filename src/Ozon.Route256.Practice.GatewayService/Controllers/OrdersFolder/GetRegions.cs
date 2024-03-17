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
                List<string> result = responce.Region.ToList();
                return StatusCode(200, result);
            }
            catch(RpcException e)
            {
                return StatusCode(502,e.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
