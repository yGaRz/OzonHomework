using Ozon.Route256.Practice.CustomerGprcFile;
using Ozon.Route256.Practice.OrdersGrpcFile;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
namespace Ozon.Route256.Practice.OrdersService.Application;

public interface IContractsMapper
{
    string ToContractRegion(RegionDto region);
    Address ToContractAddress(Domain.Address address);
    Order ToContractOrder(Domain.Order order);
    Order ToContractOrderDto(OrderDto order);
}
