using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController:ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly Orders.OrdersClient _ordersClient;
        public OrdersController(ILogger<OrdersController> logger, 
                                    Orders.OrdersClient ordersClient)
        {
            _logger = logger;
            _ordersClient = ordersClient;
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public string CancelOrder(int id)
        {
            CancelOrderByIdRequest request  = new CancelOrderByIdRequest { Id = id };
            CancelOrderByIdResponse answer = new CancelOrderByIdResponse();
            try
            {
                answer = _ordersClient.CancelOrder(request);
            }
            catch (RpcException ex)
            {
                return ex.ToString();
            }
            return answer.ToString();
        }
    }
}
