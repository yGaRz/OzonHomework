using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace Ozon.Route256.Practice.GatewayService.Controllers
{
    public partial class OrdersController
    {
        /// <summary>
        /// Возврат списка заказов
        /// </summary>
        /// <param name="regions">Список регионов</param>
        /// <param name="TypeOrder">Тип заказа</param>
        /// <param name="PaginationParam">Параметры пагинации</param>
        /// <param name="ParamSort">Параметры сортировки</param>
        /// <param name="SortField">Поле по которому будет сортировка</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<ActionResult<List<Order>>> GetOrders(List<string> regions,
                                                                        string TypeOrder,
                                                                        string PaginationParam,
                                                                        string? ParamSort,
                                                                        string? SortField,
                                                                        CancellationToken cancellationToken)
        {
            try
            {
                GetOrdersRequest request = new GetOrdersRequest()
                {
                    TypeOrder = TypeOrder,
                    PaginationParam = PaginationParam
                };
                if(ParamSort != null)
                    request.ParamSort = ParamSort;
                if(SortField != null)
                    request.SortField = SortField;
                request.Region.Add(regions);


                var responce = await _ordersClient.GetOrdersAsync(request, null, null, cancellationToken);
                List<Order> result = new List<Order>();
                if (responce != null)
                    for (int i = 0; i < responce.Orders.Count; i++)
                        result.Add(responce.Orders[i]);
                return StatusCode(200,result);
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return StatusCode(400, "Регион не найден");
                else
                    return StatusCode(502);
            }
        }
    }
}
