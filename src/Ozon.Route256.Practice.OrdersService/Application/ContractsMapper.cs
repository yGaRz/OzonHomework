using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Application;

public class ContractsMapper : IContractsMapper
{
    public string ToContractRegion(RegionDto region) => region.Name;
}
