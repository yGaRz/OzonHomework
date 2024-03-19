using Microsoft.AspNetCore.Mvc;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;

        private readonly Orders.OrdersClient _ordersClient;
        public OrdersController(ILogger<OrdersController> logger,
                                    Orders.OrdersClient ordersClient)
        {
            _logger = logger;
            _ordersClient = ordersClient;
        }

    }
}
