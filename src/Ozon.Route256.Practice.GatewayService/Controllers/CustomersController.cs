﻿using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Ozon.Route256.Practice.CustomerGprcFile;
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
        public async Task<ActionResult<List<CustomerEntity>>> GetCustomersAsync(CancellationToken cancellationToken)
        {
            try
            {
                var responce = await _customersClient.GetCustomersAsync(new GetCustomersRequest(), null, null, cancellationToken);
                return Ok(responce.Customers.Select(Convert).ToList());
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return NotFound(new List<CustomerEntity>());
                else
                    return StatusCode(502,ex);
            }
            catch(Exception ex)
            {
                var a = ex.Message;
                return BadRequest(a);
            }
        }

        private static AddressEntity Convert(Address address)
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
        private static CustomerEntity Convert(Customer customer)
        {
            return new CustomerEntity(customer.Id, customer.
                FirstName,
                customer.LastName,
                customer.MobileNumber,
                customer.Email,
                Convert(customer.DefaultAddress),
                customer.Addressed.Select(Convert).ToArray()
                );

        }

    }
}
