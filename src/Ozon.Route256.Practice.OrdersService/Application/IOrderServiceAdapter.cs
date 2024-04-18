using Ozon.Route256.Practice.OrdersGrpcFile;
using Ozon.Route256.Practice.OrdersService.Application.Commands;
using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Application;
public interface IOrderServiceAdapter
{
    Task<List<string>> GetRegions(CancellationToken token);
    Task CreateOrder(PreOrderDto preOrder, CancellationToken token);
    Task<RegionDto> GetRegion(int id, CancellationToken token);
}
