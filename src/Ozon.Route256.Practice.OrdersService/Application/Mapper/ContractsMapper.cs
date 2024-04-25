using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.CustomerGprcFile;
using Ozon.Route256.Practice.OrdersGrpcFile;
using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Application;

public class ContractsMapper : IContractsMapper
{
    public string ToContractRegion(RegionDto region) 
        => region.Name;

    public Address ToContractAddress(Domain.Address address)
        => new Address() {
            Street = address.Street,
            City = address.City,
            Apartment = address.Apartment,
            Building = address.Building,
            Region = address.Region,
            Latitude = address.Coordinates.Latitude,
            Longitude = address.Coordinates.Longitude
        };
    
    public Order ToContractOrder(Domain.Order order)
        => new Order() {
            Id = order.Id,
            CountGoods = order.CountGoods,
            DateCreate = order.TimeCreate.ToUniversalTime().ToTimestamp(),
            OrderSource = (OrderSource)order.Source,
            OrderState = (OrderState)order.State,
            TotalSum = order.TotalPrice,
            TotalWeight = order.TotalWeigth
        };

    public Order ToContractOrderDto(OrderDto order)
        => new Order()
        {
            Id=order.id,
            CountGoods=order.countGoods,
            DateCreate=order.timeCreate.ToUniversalTime().ToTimestamp(),
            OrderSource=(OrderSource)order.source,
            OrderState=(OrderState)order.state,
            TotalSum=order.totalPrice,
            TotalWeight=order.totalWeigth
        };
}
