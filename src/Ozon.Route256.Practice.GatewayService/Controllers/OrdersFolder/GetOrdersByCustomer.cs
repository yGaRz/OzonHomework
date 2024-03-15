using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    public partial class OrdersController
    {
        /// <summary>
        /// Получение списка заказов клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        /// <param name="start">Дата/время от которого стоить агрегацию</param>
        /// <param name="paginationParam">Параметры пагинации</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<Order>>> GetOrderByCustomer(int id,
                                                        DateTime start,
                                                        string paginationParam,
                                                        CancellationToken cancellationToken)
        {
            try
            {
                GetOrdersByCustomerIDRequest request = new GetOrdersByCustomerIDRequest();
                request.Id = id;
                request.PaginationParam = paginationParam;
                request.StartTime.Seconds = start.ToFileTimeUtc();
                var responce = await _ordersClient.GetOrdersByCustomerIDAsync(request, null, null, cancellationToken);
                if (responce != null)
                    return StatusCode(200, responce.Orders.ToList());
                return StatusCode(400, "Нескольких регионов нет в системе");
            }
            catch (RpcException ex)
            {
                return StatusCode(502);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}