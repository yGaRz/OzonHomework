using Microsoft.AspNetCore.Mvc;

namespace Ozon.Route256.Practice.OrdersService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloController
    {
        private readonly ILogger<HelloController> _logger;
        public HelloController(ILogger<HelloController> logger)
        {
            _logger = logger;
        }
        [HttpGet(Name = "GetGreeting")]
        public string GetGreeting()
        {
            return $"This is Order service";
        }

    }
}
