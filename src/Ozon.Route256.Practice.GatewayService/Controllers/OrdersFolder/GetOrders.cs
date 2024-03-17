using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Practice.CustomerService.DataAccess.Entities;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    public partial class OrdersController
    {
        /// <summary>
        /// Возврат списка заказов
        /// </summary>
        /// <param name="model">RegionsList - список регионов, </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK,Type=typeof(List<Order>))]
       public async Task<ActionResult<List<Order>>> GetOrders(GetOrdersModel model,
                                                                    CancellationToken cancellationToken)
        {
            try
            {
                //RegionsModel model= new RegionsModel();

                GetOrdersRequest request = new GetOrdersRequest()
                {
                    TypeOrder = model.OrderState,
                    PaginationParam = new Pagination() { 
                        PageIndex = model.PaginationParam.PageIndex,
                        PageSize=model.PaginationParam.PageSize,
                        MaxPageSize=model.PaginationParam.MaxPageSize
                    },
                    SortField = model.SortField
                };

                if (model.SortParam != null)
                    request.SortParam = (SortParam)model.SortParam;
                else 
                    request.SortParam = SortParam.None;
                request.Region.Add(model.RegionsList);

                var responce = await _ordersClient.GetOrdersAsync(request, null, null, cancellationToken);

                List<Order> result = new List<Order>();
                if (responce != null)
                    for (int i = 0; i < responce.Orders.Count; i++)
                        result.Add(responce.Orders[i]);
                return StatusCode(200,result);
            }
            catch (RpcException)
            {
                return StatusCode(502);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
