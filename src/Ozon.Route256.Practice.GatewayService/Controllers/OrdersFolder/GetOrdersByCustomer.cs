using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Practice.GatewayService.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    public partial class OrdersController
    {
        /// <summary>
        /// Get customer orders
        /// </summary>
        /// <param name="id">id customer</param>
        /// <param name="pageIndex">page index for pagination</param>
        /// <param name="pageSize">count item on page</param>
        /// <param name="start">start time for find</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [SwaggerResponse(200, "Orders list", typeof(OrdersListModel))]
        [SwaggerResponse(400, "Customer not found")]
        public async Task<ActionResult<OrdersListModel>> GetOrderByCustomer([FromHeader]int id,
                                                                    [FromHeader]uint pageIndex,
                                                                    DateTime start,
                                                                    CancellationToken cancellationToken,
                                                                    uint pageSize = 50)
        {
            try
            {
                GetOrdersByCustomerIDRequest request = new GetOrdersByCustomerIDRequest()
                {
                    Id = id,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    StartTime = Timestamp.FromDateTimeOffset(start)
                };
                var responce = await _ordersClient.GetOrdersByCustomerIDAsync(request, null, null, cancellationToken);
                OrdersListModel result = new OrdersListModel() { PageIndex = responce.PageNumber };
                foreach(var a in responce.Orders)
                    result.ListOrder.Add(a);
                return StatusCode(200,result);
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return StatusCode(400, "Customer not found");
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