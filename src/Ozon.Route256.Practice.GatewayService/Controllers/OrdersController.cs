using Microsoft.AspNetCore.Mvc;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController:ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        OrdersController(ILogger<OrdersController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        public string Get1()
        {
            return "qweqwe";
        }

        [HttpGet]
        [Route("[action]")]
        public string Get2()
        {
            return "qweqwe";
        }

        [HttpGet]
        [Route("[action]")]
        public string Get3()
        {
            return "qweqwe";
        }
        [HttpDelete]
        [Route("[action]")]
        public string CancelOrder(long orderId)
        {
            return (orderId * 1000).ToString();
        }

    }
}
