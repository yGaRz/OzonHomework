using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Mappers;

internal interface IDataReadMapper
{
    List<RegionDto> RegionsDalToDto(RegionDal[] regions);
    RegionDto RegionDalToDto(RegionDal region);
}
