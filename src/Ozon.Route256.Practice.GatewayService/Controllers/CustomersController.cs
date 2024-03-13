using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger;
        private readonly Customers.CustomersClient _customersClient;
        public CustomersController(ILogger<CustomersController> logger,
                                Customers.CustomersClient client)
        {
            _logger = logger;
            _customersClient = client;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<List<Customer>>> GetCustomersAsync(CancellationToken cancellationToken)
        {
            List<Customer> result = new List<Customer>();
            try
            {
                var request = new GetCustomersRequest();
                await _customersClient.GetCustomersAsync(request,null,null,cancellationToken);
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return StatusCode(404);
                else
                    return StatusCode(502);
            }
            return result;
        }
    }
}
