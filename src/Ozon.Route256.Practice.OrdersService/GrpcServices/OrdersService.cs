using Grpc.Core;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Exceptions;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices
{
    public sealed class OrdersService:Orders.OrdersBase
    {
        public readonly IRegionRepository _regionRepository;
        public readonly IOrdersRepository _ordersRepository;
        public OrdersService(IRegionRepository regionRepository, IOrdersRepository ordersRepository)
        {
            _regionRepository = regionRepository;
            _ordersRepository = ordersRepository;
        }

        //TODO: Ручка отмены заказа
        public override Task<CancelOrderByIdResponse> CancelOrder(CancelOrderByIdRequest request, ServerCallContext context)
        {
            //Логика:
            //1. Проверяем наличие заказа
            //2. Проверяем статус в логистике
            //3. если можем отменить - отменяем и меняем статус заказа.
            CancelOrderByIdResponse response = new CancelOrderByIdResponse();
            if(request.Id==0)
                return Task.FromResult(response);
            if (request.Id == 1)
                throw new RpcException(new Status(StatusCode.NotFound, $"Order {request.Id} not found"));
            if (request.Id == 2)
                throw new RpcException(new Status(StatusCode.Cancelled, $"Order canceled faild, reason={response.ReasonCancelError}"));
            throw new RpcException(new Status(StatusCode.NotFound, $"Order {request.Id} not found"));
            //throw new NotFoundException($"Order by Id = {request.Id} not founded");
        }
        
        //TODO: Ручка возврата статуса заказа
        public override async Task<GetOrderStatusByIdResponse> GetOrderStatusById(GetOrderStatusByIdRequest request, ServerCallContext context)
        {
            var order = await _ordersRepository.GetOrderByIdAsync(request.Id,context.CancellationToken);
            if (order != null)
            {
                throw new NotFoundException($"11111");
            }
            else
                throw new NotFoundException($"Order by Id = {request.Id} not founded");
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

        //TODO: Ручка агрегации заказов по региону
        public override Task<GetRegionStatisticResponse> GetRegionStatistic(GetRegionStatisticRequest request, ServerCallContext context)
        {
            //Получение агрегации по регионам
            //проверям имена, и если есть делаем выборку из репозитория
            return base.GetRegionStatistic(request, context);
        }


    }

}
