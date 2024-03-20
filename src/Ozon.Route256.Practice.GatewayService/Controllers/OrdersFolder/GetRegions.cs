using Grpc.Core;

using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    public partial class OrdersController
    {
        /// <summary>
        /// Get region list
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Region list or empty</returns>
        [HttpGet("[action]")]
        [SwaggerResponse(200, "Region list",typeof(List<string>))]
        [Produces("application/json")]
        public async Task<ActionResult<List<string>>> GetRegions(CancellationToken cancellationToken)
        {
            try
            {
                var responce = await _ordersClient.GetRegionAsync(new GetRegionRequest(), null, null, cancellationToken);
                return Ok(responce.Region.ToList());
            }
            catch(RpcException ex)
            {
                return StatusCode(502, "The service is not responding:" + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
