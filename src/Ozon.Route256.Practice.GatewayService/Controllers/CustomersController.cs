using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Ozon.Route256.Practice.GatewayService.Etities;
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
        [SwaggerResponse(200, "Customer list", Type = typeof(List<CustomerEntity>))]
        [Produces("application/json")]
        public async Task<ActionResult<CustomerEntity>> GetCustomersAsync(CancellationToken cancellationToken)
        {
            try
            {
                var responce = await _customersClient.GetCustomersAsync(new GetCustomersRequest(), null, null, cancellationToken);
                //if (responce != null)
                return Ok(responce.Customers.Select(From).ToList());
                //return Ok(new List<CustomerEntity>());
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return NotFound(new List<Customer>());
                else
                    return StatusCode(502,ex);
            }
            catch(Exception ex)
            {
                var a = ex.Message;
                return BadRequest(a);
            }
        }

        private static AddressEntity From(Address address)
        {
            return new AddressEntity(
                address.Region,
                address.City,
                address.Street,
                address.Building,
                address.Apartment,
                address.Latitude,
                address.Longitude);
        }
        private static CustomerEntity From(Customer customer)
        {
            return new CustomerEntity(customer.Id, customer.
                FirstName,
                customer.LastName,
                customer.MobileNumber,
                customer.Email,
                From(customer.DefaultAddress),
                customer.Addressed.Select(From).ToArray()
                );

        }

    }
}
