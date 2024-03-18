using Grpc.Core;
using Ozon.Route256.Practice.OrdersService.Exceptions;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices
{
    public sealed class OrdersService:Orders.OrdersBase
    {
        //Отмена заказа
        public override Task<CancelOrderByIdResponse> CancelOrder(CancelOrderByIdRequest request, ServerCallContext context)
        {
            //Логика:
            //1. Проверяем наличие заказа
            //2. Проверяем статус в логистике
            //3. если можем отменить - отменяем и меняем статус заказа.
            CancelOrderByIdResponse response = new CancelOrderByIdResponse();
            if(request.Id==0)
                throw new RpcException(new Status(StatusCode.NotFound, $"Order {request.Id} not found"));
            if (request.Id == 1)
                return Task.FromResult(response);
            if (request.Id == 2)
            {
                response.ReasonCancelError = "Невозможно отменить заказ номер 2";
                return Task.FromResult(response);
            }
            throw new RpcException(new Status(StatusCode.NotFound, $"Order {request.Id} not found"));
            //throw new NotFoundException($"Order by Id = {request.Id} not founded");
        }
        public override Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, ServerCallContext context)
        {
            //Обращаемся в репозиторий и возвращаем массив,
            //в переспективе грузим список заказов в Redis и оттуда выдаем страницами
            return Task.FromResult(new GetOrdersResponse());
        }
        public override Task<GetOrdersByCustomerIDResponse> GetOrdersByCustomerID(GetOrdersByCustomerIDRequest request, ServerCallContext context)
        {
            //Проверить есть ли пользователь в системе - GetCustomerByIdRequest
            //если есть, обратится в репозиторий за списком заказов
            throw new NotFoundException($"Client with id = {request.Id} not founded");
        }
        public override Task<GetRegionStatisticResponse> GetRegionStatistic(GetRegionStatisticRequest request, ServerCallContext context)
        {
            //Получение агрегации по регионам
            //проверям имена, и если есть делаем выборку из репозитория
            return base.GetRegionStatistic(request, context);
        }

        public override Task<GetOrderStatusByIdResponse> GetOrderStatusById(GetOrderStatusByIdRequest request, ServerCallContext context)
        {
            //Проверяем наличие заказа в системе
            //если есть проверям в логистике
            throw new NotFoundException($"Order by Id = {request.Id} not founded");
        }
        public override Task<GetRegionResponse> GetRegion(GetRegionRequest request, ServerCallContext context)
        {
            //возврат списка регионов из репозитория IRegionRepository
            return Task.FromResult( new GetRegionResponse());
        }
    }

}
