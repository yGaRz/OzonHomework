using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Application.Queries.GetRegionsQuery;
public sealed class GetRegionsQuery : IRequest<List<RegionDto>> 
{ 
}
