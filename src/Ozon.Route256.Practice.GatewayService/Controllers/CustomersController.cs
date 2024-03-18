using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;


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
        [SwaggerResponse(200, "Customer list", Type = typeof(List<Customer>))]
        [Produces("application/json")]
        public async Task<ActionResult<List<Customer>>> GetCustomersAsync(CancellationToken cancellationToken)
        {
            try
            {
                var responce = await _customersClient.GetCustomersAsync(new GetCustomersRequest(), null, null, cancellationToken);
                if (responce != null)
                    return responce.Customers.ToList();
                return new List<Customer>();
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return Ok(new List<Customer>());
                else
                    return StatusCode(502,ex);
            }
        }
    }
}
