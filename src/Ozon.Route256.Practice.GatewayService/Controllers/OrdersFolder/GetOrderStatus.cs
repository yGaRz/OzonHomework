using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Practice.GatewayService.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    public partial class OrdersController
    {
        /// <summary>
        /// Получение статуса заказа
        /// </summary>
        /// <param name="id">Номер заказа</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [SwaggerResponse(200, "Order status in logistics")]
        [SwaggerResponse(404, "Order not found")]
        [Produces("application/json")]
        public async Task<ActionResult<OrderState>> GetOrderStatus(int id, CancellationToken cancellationToken)
        {
            try
            {
                var responce = await _ordersClient.GetOrderStatusByIdAsync(new GetOrderStatusByIdRequest { Id = id }, null, null, cancellationToken);
                return Ok(responce.LogisticStatus.ToString());
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return NotFound();
                else
                    return StatusCode(502, "The service is not responding:" + ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
    }
}
