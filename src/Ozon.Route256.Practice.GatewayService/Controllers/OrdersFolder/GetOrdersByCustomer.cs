using Google.Protobuf.WellKnownTypes;
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
        [ProducesResponseType(StatusCodes.Status200OK,Type=typeof(List<Order>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<Order>>> GetOrderByCustomer(int id,
                                                        DateTime start,
                                                        Pagination paginationParam,
                                                        CancellationToken cancellationToken)
        {
            try
            {
                if (start > DateTime.Now)
                    return StatusCode(400, "Некорректно указана дата и время поиска заказов");
                GetOrdersByCustomerIDRequest request = new GetOrdersByCustomerIDRequest()
                {
                    Id = id,
                    PaginationParam = new Pagination()
                    {
                        PageIndex = paginationParam.PageIndex,
                        PageSize = paginationParam.PageSize
                       // MaxPageSize = paginationParam.MaxPageSize
                    },
                    StartTime = Timestamp.FromDateTimeOffset(start)
                };
                var responce = await _ordersClient.GetOrdersByCustomerIDAsync(request, null, null, cancellationToken);
                return StatusCode(200, responce.Orders.ToList());
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return StatusCode(400, "Покупатель не найден");
                else
                    return StatusCode(502);
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
    }
}