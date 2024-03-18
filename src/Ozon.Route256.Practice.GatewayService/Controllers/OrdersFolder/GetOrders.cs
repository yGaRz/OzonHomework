﻿using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Practice.CustomerService.DataAccess.Entities;
using Ozon.Route256.Practice.GatewayService.Models;
using Swashbuckle.AspNetCore.Annotations;

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
        public async Task<ActionResult<OrdersListModel>> GetOrders(uint pageIndex,
                                                                GetOrdersModel model,
                                                                CancellationToken cancellationToken)
        {
            try
            {
                GetOrdersRequest request = new GetOrdersRequest()
                {
                    TypeOrder = model.OrderState,
                    PageIndex = pageIndex,
                    PageSize = model.PageSize,
                    SortField = model.SortField,
                    SortParam = model.SortParam != null ? (SortParam)model.SortParam : SortParam.None
                };
                request.Region.Add(model.RegionsList);

                var responce = await _ordersClient.GetOrdersAsync(request, null, null, cancellationToken);

                OrdersListModel res = new OrdersListModel();
                if (responce != null)
                    for (int i = 0; i < responce.Orders.Count; i++)
                        res.ListOrder.Add(responce.Orders[i]);
                return StatusCode(200,res);
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
                    return StatusCode(400);
                return StatusCode(502, "The service is not responding:" + ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
