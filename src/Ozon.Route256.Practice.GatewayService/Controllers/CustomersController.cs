using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger;
        private readonly Customers.CustomersClient _customersClient;
        public CustomersController(ILogger<CustomersController> logger,
                                Customers.CustomersClient client)
        {
            _logger = logger;
            _customersClient = client;
        }
        /// <summary>
        /// Возврат списка клиентов
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK,Type =typeof(List<Customer>))]
        public async Task<ActionResult<List<Customer>>> GetCustomersAsync(CancellationToken cancellationToken)
        {
            try
            {
                var request = new GetCustomersRequest();
                var responce = await _customersClient.GetCustomersAsync(request, null, null, cancellationToken);
                if (responce != null)
                    return responce.Customers.ToList();
                return new List<Customer>();
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return StatusCode(404);
                else
                    return StatusCode(502);
            }
        }
    }
}
