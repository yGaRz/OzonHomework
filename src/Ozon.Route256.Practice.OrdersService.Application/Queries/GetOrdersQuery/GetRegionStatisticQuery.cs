using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Application.Queries.GetOrdersQuery;
public class GetRegionStatisticQuery : IRequest<RegionStatisticDto[]>
{
    public List<string> Regions { get; }=new List<string>();
    public DateTime StartTime { get; init; }
}
