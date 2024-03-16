using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GetOrderStatus(int id, CancellationToken cancellationToken)
        {
            if (id == 0)
                return BadRequest();
            try
            {
                GetOrderStatusByIdRequest request = new GetOrderStatusByIdRequest { Id = id };
                var responce = await _ordersClient.GetOrderStatusByIdAsync(request, null, null, cancellationToken);
                return responce.LogisticStatus;
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return StatusCode(404);
                else
                    return StatusCode(502);
            }
        }
    }
}
