using Grpc.Core;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.LogisticsSimulator.Grpc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Ozon.Route256.Practice.OrdersService.Models;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices
{
    public sealed class OrdersService:Orders.OrdersBase
    {
        public readonly IRegionRepository _regionRepository;
        public readonly IOrdersRepository _ordersRepository;
        public readonly LogisticsSimulatorService.LogisticsSimulatorServiceClient _logisticsSimulatorServiceClient;
        public readonly Customers.CustomersClient _customersClient;
        public OrdersService(IRegionRepository regionRepository, 
            IOrdersRepository ordersRepository, 
            LogisticsSimulatorService.LogisticsSimulatorServiceClient logisticsSimulatorServiceClient,
            Customers.CustomersClient customersClient
            )
        {
            _regionRepository = regionRepository;
            _ordersRepository = ordersRepository;
            _logisticsSimulatorServiceClient = logisticsSimulatorServiceClient;
            _customersClient = customersClient;
        }
        //: Ручка возврата статуса заказа
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
            if (order != null)
            {
                var requestLogistic = new LogisticsSimulator.Grpc.Order() { Id = request.Id };
                var responceLogistic = await _logisticsSimulatorServiceClient.OrderCancelAsync(requestLogistic, null, null, context.CancellationToken);
                context.CancellationToken.ThrowIfCancellationRequested();
                if (responceLogistic.Success)
                    return new CancelOrderByIdResponse();
                else
                    throw new RpcException(new Status(StatusCode.Cancelled, responceLogistic.Error));
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
        public override async Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, ServerCallContext context)
        {
            if (!await _regionRepository.IsRegionInRepository(request.Region.ToArray(), context.CancellationToken))
                throw new RpcException(new Status(StatusCode.NotFound, "Region not found"));
            var orders = await _ordersRepository.GetOrdersByRegionAsync(request.Region.ToList(),(OrderSourceEnum)request.Source,context.CancellationToken);
            //TODO:Тут надо сделать сортировку по полям по полям, синий трактор едет к нам


            GetOrdersResponse responce = new GetOrdersResponse();

            return responce;
        }

        //Ручка получения всех заказов клиента
        public override async Task<GetOrdersByCustomerIDResponse> GetOrdersByCustomerID(GetOrdersByCustomerIDRequest request, ServerCallContext context)
        {
            //Проверить есть ли пользователь в системе - GetCustomerByIdRequest
            //если есть, обратится в репозиторий за списком заказов
            bool isCustomer = true;
            try
            {
                var id = request.Id;
                GetCustomerByIdRequest customer = new GetCustomerByIdRequest() { Id = id };
                GetCustomerByIdResponse respCustomer=new GetCustomerByIdResponse();
                try
                {
                    respCustomer = await _customersClient.GetCustomerByIdAsync(customer);
                }
                catch (RpcException)
                {
                    isCustomer = false;
                }
                if (!isCustomer)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, $"Клиент с id={id} не найден"));

                var orders = await _ordersRepository.GetOrdersByCutomerAsync(id, request.StartTime.ToDateTime());

                Task.WaitAll(Task.FromResult(orders));

                GetOrdersByCustomerIDResponse responce = new GetOrdersByCustomerIDResponse();
                responce.NameCustomer = $"{respCustomer.Customer.FirstName} {respCustomer.Customer.LastName}";
                responce.PhoneNumber = respCustomer.Customer.MobileNumber;
                responce.Region = respCustomer.Customer.DefaultAddress.Region;
                responce.AddressCustomer= respCustomer.Customer.DefaultAddress;

                foreach (var order in orders) 
                {
                    var added = new Order()
                    {
                        CountGoods = order.CountGoods,
                        DateCreate = order.TimeCreate,
                        Id = order.Id,
                        TotalWeight = order.TotalWeigth,
                        OrderSource = (OrderSource)order.Source,
                        OrderState = (OrderState)order.State,
                        TotalSum = order.TotalSum
                    };
                    foreach(var g in order.Goods)
                    {
                        added.ProductList.Add(new Product()
                        {
                            Id = g.Id,
                            Name = g.Name,
                            Quantity = g.Quantity,
                            Price = g.Price,
                            Wight = g.Weight
                        });
                    }
                    responce.Orders.Add(added);                    
                }
                return responce;
            }
            catch (RpcException)
            {
                isCustomer = false;
            }
            if (isCustomer)
            {

                throw new RpcException(new Status(StatusCode.NotFound, $"Client with id = {request.Id} not founded"));
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"Client with id = {request.Id} not founded"));

        }

        //Ручка агрегации заказов по региону
        public override async Task<GetRegionStatisticResponse> GetRegionStatistic(GetRegionStatisticRequest request, ServerCallContext context)
        {
            if (!await _regionRepository.IsRegionInRepository(request.Region.ToArray(), context.CancellationToken))
                throw new RpcException(new Status(StatusCode.NotFound, "Region not found"));
            GetRegionStatisticResponse regionStatisticResponse = new GetRegionStatisticResponse();
            RegionStatisticEntity[]? result = null;
            if(request.Region.Count==0)
            {
                string[] allRegions = await _regionRepository.GetRegionsAsync(context.CancellationToken);
                result = await _ordersRepository.GetRegionsStatisticAsync(allRegions.ToList(), request.StartTime, context.CancellationToken);
            }
            else
                result = await _ordersRepository.GetRegionsStatisticAsync(request.Region.ToList(), request.StartTime, context.CancellationToken);

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
