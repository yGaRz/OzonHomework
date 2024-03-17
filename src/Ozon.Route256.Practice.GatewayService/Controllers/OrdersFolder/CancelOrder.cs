using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(StatusCodes.Status200OK)]        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> CancelOrder(long id, CancellationToken cancellationToken)
        {
            try
            {
                CancelOrderByIdRequest request = new CancelOrderByIdRequest { Id = id };
                var responce = await _ordersClient.CancelOrderAsync(request, null, null, cancellationToken);
                if (responce.ReasonCancelError == "")
                    return StatusCode(200, "Заказ отменен успешно");
                else
                    return StatusCode(400, responce.ReasonCancelError);
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return StatusCode(404, "Заказ не найден");
                else
                    return StatusCode(502,"Сервис не отвечает");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
