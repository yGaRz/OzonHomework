﻿using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.RegionManager.Repository;
using System.Collections.Concurrent;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.RegionManager;
internal class RegionDatabaseInMemory : IRegionDatabase
{
    private static readonly ConcurrentDictionary<int, RegionDal> RegionsDictionary = new();
    private readonly IRegionRepository _regionRepositoryPg;
    public RegionDatabaseInMemory(IRegionRepository regionRepositoryPg)
    {
        _regionRepositoryPg = regionRepositoryPg;
    }

    public async Task<Task> CreateRegionAsync(RegionDal region, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        if (RegionsDictionary.TryGetValue(region.Id, out _))
            return Task.FromException(new Exception($"Region with id={region.Id} already exists"));
        else
        {
            await _regionRepositoryPg.Create(new RegionDal(region.Id, region.Name, region.Latitude, region.Longitude), token);
            RegionsDictionary.TryAdd(region.Id, region);
            return Task.CompletedTask;
        }
    }

    public async Task<bool> IsRegionsExistsAsync(string[] regionName, CancellationToken cancellationToken = default)
    {
        if (RegionsDictionary.Values.Where(x => regionName.Contains(x.Name)).ToArray().Length == regionName.Length)
            return true;
        else
        {
            await Update(cancellationToken);
            if (RegionsDictionary.Values.Where(x => regionName.Contains(x.Name)).ToArray().Length == regionName.Length)
                return true;
            return false;
        }
    }

    public async Task<RegionDal[]> GetRegionsEntityByIdAsync(int[] regionId, CancellationToken cancellationToken = default)
    {
        if (regionId.Length == 0)
        {
            await Update(cancellationToken);
            return await Task.FromResult(RegionsDictionary.Values.ToArray());
        }

        var result = RegionsDictionary.Values.Where(x => regionId.Contains(x.Id)).ToArray();
        if (result.Length != regionId.Length)
            await Update(cancellationToken);

        result = RegionsDictionary.Values.Where(x => regionId.Contains(x.Id)).ToArray();
        if (result.Length == regionId.Length)
            return await Task.FromResult(result);
        else
            throw new NotFoundException($"Region with name={regionId} is not found");
    }

    public async Task<RegionDal[]> GetRegionsEntityByNameAsync(string[] regionName, CancellationToken cancellationToken = default)
    {
        if (regionName.Length == 0)
            return await Task.FromResult(RegionsDictionary.Values.ToArray());
        var result = RegionsDictionary.Values.Where(x => regionName.Contains(x.Name)).ToArray();

        if (result.Length != regionName.Length)
            await Update(cancellationToken);

        result = RegionsDictionary.Values.Where(x => regionName.Contains(x.Name)).ToArray();
        if (result.Length == regionName.Length)
            return await Task.FromResult(result);
        else
            throw new NotFoundException($"Region with name={regionName} is not found");
    }

    public async Task<RegionDal> GetRegionEntityByIdAsync(int regionId, CancellationToken cancellationToken = default)
    {
        var result = RegionsDictionary.Values.Where(x => regionId == x.Id).FirstOrDefault();
        if (result == null)
            await Update(cancellationToken);

        result = RegionsDictionary.Values.Where(x => regionId == x.Id).FirstOrDefault();
        if (result != null)
            return await Task.FromResult(result);
        else
            throw new NotFoundException($"Region with name={regionId} is not found");
    }

    public async Task Update(CancellationToken token = default)
    {
        var regions = await _regionRepositoryPg.GetAll(token);
        foreach (var region in regions)
            RegionsDictionary.TryAdd(region.Id, new RegionDal(region.Id, region.Name, region.Latitude, region.Longitude));
    }

    public async Task<RegionDal> GetRegionEntityByNameAsync(string regionName, CancellationToken cancellationToken)
    {
        var result = RegionsDictionary.Values.Where(x => regionName == x.Name).FirstOrDefault();
        if (result == null)
            await Update(cancellationToken);

        result = RegionsDictionary.Values.Where(x => regionName == x.Name).FirstOrDefault();
        if (result != null)
            return await Task.FromResult(result);
        else
            throw new NotFoundException($"Region with name={regionName} is not found");
    }
}
