using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Practice.GatewayService.Etities;
using Ozon.Route256.Practice.GatewayService.Models;
using Swashbuckle.AspNetCore.Annotations;
using Ozon.Route256.Practice.CustomerGprcFile;
using Ozon.Route256.Practice.OrdersGrpcFile;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    public partial class OrdersController
    {
        /// <summary>
        /// Получение списка заказов клиента
        /// </summary>
        /// <param name="id">Номер клиента</param>
        /// <param name="pageIndex">Номер страницы запроса</param>
        /// <param name="pageSize">Максимальное количество заказов на странице</param>
        /// <param name="start">Время начиная с которого происходит поиск</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [SwaggerResponse(200, "Orders list", typeof(CustomerOrdersListModel))]
        [SwaggerResponse(400, "Customer not found")]
        public async Task<ActionResult<CustomerOrdersListModel>> GetOrderByCustomer([FromHeader] int id,
                                                                    [FromHeader] uint pageIndex = 1,
                                                                    DateTime start = default,                                                                    
                                                                    uint pageSize = 50,
                                                                    CancellationToken cancellationToken = default)
        {
            try
            {
                GetOrdersByCustomerIDRequest request = new GetOrdersByCustomerIDRequest()
                {
                    Id = id,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    StartTime = Timestamp.FromDateTimeOffset(start)
                };
                var responce = await _ordersClient.GetOrdersByCustomerIDAsync(request, null, null, cancellationToken);

                CustomerOrdersListModel result = new CustomerOrdersListModel() { 
                    PageIndex = responce.PageNumber,
                    address = Convert(responce.AddressCustomer),
                    CustomerName = responce.NameCustomer,
                    Phone=responce.PhoneNumber,
                    Region=responce.Region                   
                };
                foreach(var a in responce.Orders)
                    result.ListOrder.Add(a);
                return Ok(result);
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return BadRequest();
                else
                    return StatusCode(502);
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
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
    }
}