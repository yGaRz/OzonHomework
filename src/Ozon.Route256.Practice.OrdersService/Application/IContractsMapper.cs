using Ozon.Route256.Practice.OrdersService.Application.Dto;
namespace Ozon.Route256.Practice.OrdersService.Application;

public interface IContractsMapper
{
    string ToContractRegion(RegionDto region);
}
