using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Practice.GatewayService.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    public partial class OrdersController
    {
        /// <summary>
        /// Get orders
        /// </summary>
        /// <param name="pageIndex">Page number</param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [SwaggerResponse(200, "Orders list", typeof(OrdersListModel))]
        [SwaggerResponse(400, "Region not found")]
        [Produces("application/json")]
        public async Task<ActionResult<OrdersListModel>> GetOrders([FromHeader]uint pageIndex,
                                                                [FromBody]GetOrdersModel model,
                                                                CancellationToken cancellationToken)
        {
            var results = new List<ValidationResult>();
            if(!ModelState.IsValid)
            {
                StringBuilder sb = new StringBuilder();
                foreach(var item in results)
                    sb.Append(item.ErrorMessage+"\r\n");
                return BadRequest(sb.ToString());
            }

            try
            {
                GetOrdersRequest request = new GetOrdersRequest()
                {
                    Source = (OrderSource)model.Source,
                    PageIndex = pageIndex,
                    PageSize = model.PageSize,
                    SortField = model.SortField,
                    SortParam = (SortParam)model.SParam
                };
                request.Region.Add(model.RegionsList);

                var responce = await _ordersClient.GetOrdersAsync(request, null, null, cancellationToken);

                OrdersListModel res = new OrdersListModel();
                if (responce != null)
                    for (int i = 0; i < responce.Orders.Count; i++)
                        res.ListOrder.Add(responce.Orders[i]);
                return Ok(res);
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return BadRequest();
                return StatusCode(502, "The service is not responding:" + ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
