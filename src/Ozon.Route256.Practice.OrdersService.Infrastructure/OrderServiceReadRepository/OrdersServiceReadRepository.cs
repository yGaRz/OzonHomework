﻿using Ozon.Route256.Practice.OrdersService.Application;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Application.Queries.GetRegionsQuery;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Mappers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.Orders;
using Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.RegionManager;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository;

internal sealed class OrdersServiceReadRepository : IOrdersServiceReadRepository
{
    private readonly IOrdersManager _ordersManager;
    private readonly IRegionDatabase _regionDatabase;
    private readonly IDataReadMapper _mapper;
    public OrdersServiceReadRepository(IOrdersManager ordersManager, IRegionDatabase regionDatabase, IDataReadMapper mapper)
    {
        _ordersManager = ordersManager;
        _regionDatabase = regionDatabase;
        _mapper = mapper;
    }

    public async Task<RegionDto> GetRegionById(GetRegionByIdQuery request, CancellationToken token)
    {
        var region = await _regionDatabase.GetRegionEntityByIdAsync(request.Id, token);
        return _mapper.RegionDalToDto(region);
         }
    public async Task<List<RegionDto>> GetRegions(GetRegionsQuery query, CancellationToken token)
    {
        var regions = await _regionDatabase.GetRegionsEntityByIdAsync(Array.Empty<int>(), token);
        return _mapper.RegionsDalToDto(regions);        
    }



}
