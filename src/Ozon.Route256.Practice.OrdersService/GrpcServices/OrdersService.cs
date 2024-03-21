using Grpc.Core;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.LogisticsSimulator.Grpc;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices
{
    public sealed class OrdersService:Orders.OrdersBase
    {
        public readonly IRegionRepository _regionRepository;
        public readonly IOrdersRepository _ordersRepository;
        public readonly LogisticsSimulatorService.LogisticsSimulatorServiceClient _logisticsSimulatorServiceClient;
        public OrdersService(IRegionRepository regionRepository, 
            IOrdersRepository ordersRepository, 
            LogisticsSimulatorService.LogisticsSimulatorServiceClient logisticsSimulatorServiceClient)
        {
            _regionRepository = regionRepository;
            _ordersRepository = ordersRepository;
            _logisticsSimulatorServiceClient = logisticsSimulatorServiceClient;
        }
        //Ручка возврата статуса заказа
        public override async Task<GetOrderStatusByIdResponse> GetOrderStatusById(GetOrderStatusByIdRequest request, ServerCallContext context)
        {
            var order = await _ordersRepository.GetOrderByIdAsync(request.Id, context.CancellationToken);
            if (order != null)       
                return new GetOrderStatusByIdResponse() { LogisticStatus = (OrderState)order.State };
            else
                throw new NotFoundException($"Order by Id = {request.Id} not founded");
        }
        //Ручка отмены заказа
        public override async Task<CancelOrderByIdResponse> CancelOrder(CancelOrderByIdRequest request, ServerCallContext context)
        {
            var id = request.Id;
            var order = await _ordersRepository.GetOrderByIdAsync(id);
            context.CancellationToken.ThrowIfCancellationRequested();
            if(order!=null)
            {
                try
                {
                    var requestLogistic = new LogisticsSimulator.Grpc.Order() { Id = request.Id };
                    var responceLogistic = await _logisticsSimulatorServiceClient.OrderCancelAsync(requestLogistic, null, null, context.CancellationToken);
                    context.CancellationToken.ThrowIfCancellationRequested();
                    if (responceLogistic.Success)
                        throw new RpcException(new Status(StatusCode.Aborted, responceLogistic.Error));
                    else
                        throw new RpcException(new Status(StatusCode.Cancelled, responceLogistic.Error));
                }
                catch (RpcException e)
                {
                    throw new RpcException(new Status(StatusCode.Aborted,$"Logistic exception: {e.Message}"));
                }
            }
            else
                throw new RpcException(new Status(StatusCode.NotFound, $"Order by Id = {request.Id} not founded"));
            //CancelOrderByIdResponse response = new CancelOrderByIdResponse();
            //if(request.Id==0)
            //    return Task.FromResult(response);
            //if (request.Id == 1)
            //    throw new RpcException(new Status(StatusCode.NotFound, $"Order {request.Id} not found"));
            //if (request.Id == 2)
            //    throw new RpcException(new Status(StatusCode.Cancelled, $"Order canceled faild, reason={response.ReasonCancelError}"));
            //throw new RpcException(new Status(StatusCode.NotFound, $"Order {request.Id} not found"));
        }
        

        
        //Ручка возврата списка регионов
        public override async Task<GetRegionResponse> GetRegion(GetRegionRequest request, ServerCallContext context)
        {
            var regions = await _regionRepository.GetRegionsAsync();
            var result = new GetRegionResponse
            {
                Region = { regions.ToArray() }
            };
            return result;
        }

        //TODO: Ручка возврата списка заказов, выборка заказов по списку регионов и типу заказа -> список закзов
        public override Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, ServerCallContext context)
        {





            return base.GetOrders(request, context);
        }

        //TODO: Ручка получения всех заказов клиента
        public override Task<GetOrdersByCustomerIDResponse> GetOrdersByCustomerID(GetOrdersByCustomerIDRequest request, ServerCallContext context)
        {
            //Проверить есть ли пользователь в системе - GetCustomerByIdRequest
            //если есть, обратится в репозиторий за списком заказов
            throw new NotFoundException($"Client with id = {request.Id} not founded");
        }

        //Ручка агрегации заказов по региону
        public override async Task<GetRegionStatisticResponse> GetRegionStatistic(GetRegionStatisticRequest request, ServerCallContext context)
        {
            if (!await _regionRepository.IsRegionInRepository(request.Region.ToArray(), context.CancellationToken))
                throw new RpcException(new Status(StatusCode.NotFound, "Region not found"));
            GetRegionStatisticResponse regionStatisticResponse = new GetRegionStatisticResponse();
            var result = await _ordersRepository.GetRegionsStatisticAsync(request.Region.ToList(), request.StartTime, context.CancellationToken);
            foreach ( var item in result )
            {
                regionStatisticResponse.Statistic.Add(new RegionStatisticMessage()
                {
                    Region = item.RegionName,
                    CountCustomer = (int)item.TotalCustomer,
                    TotalCountOrders= (int)item.TotalCountOrders,
                    TotalSumOrders= (int)item.TotalSumOrders,
                    TotalWightOrders= item.TotalWigthOrder
                });
            }
            return regionStatisticResponse;
        }


    }

}
