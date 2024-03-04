using Microsoft.AspNetCore.Mvc;

namespace Ozon.Route256.Practice.CustomerService.Controllers
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
        [HttpGet("{Name}", Name = "GetGreeting")]
        public string GetGreeting(string Name)
        {
            return $" Hello {Name} this is Customer service";
        }

    }
}
