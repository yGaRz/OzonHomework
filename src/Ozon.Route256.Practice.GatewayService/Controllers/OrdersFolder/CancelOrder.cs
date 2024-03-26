using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    public partial class OrdersController
    {
        /// <summary>
        /// Отмена заказа
        /// </summary>
        /// <param name="id">Номер заказа</param>
        /// <param name="cancellationToken"></param>        
        /// <returns></returns>   
        [HttpDelete("[action]")]
        [SwaggerResponse(200, "Order canceled successfully")]
        [SwaggerResponse(400, "The order cannot be canceled")]
        [SwaggerResponse(404, "Order not found")]
        [Produces("application/json")]
        public async Task<ActionResult> CancelOrder(long id, CancellationToken cancellationToken)
        {            
            try
            {
                CancelOrderByIdRequest request = new CancelOrderByIdRequest { Id = id };
                var responce = await _ordersClient.CancelOrderAsync(request, null, null, cancellationToken);
                return Ok();
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.Cancelled)
                    return BadRequest(ex.Message);
                if(ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return NotFound(ex.Message);
                else
                    return StatusCode(502, "The service is not responding:" + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
