using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using System.Collections.Concurrent;

namespace Ozon.Route256.Practice.OrdersService.DataAccess
{
    public class RegionRepository : IRegionRepository
    {
        private static readonly ConcurrentDictionary<int,string> RegionsIntString = new ();
        private static readonly ConcurrentDictionary<string, int> RegionsStringInt = new();
        public Task CreateRegionAsync(RegionEntity region, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            if(RegionsIntString.TryGetValue(region.Id,out _))
                return Task.FromException(new Exception($"Region with id={region.Id} already exists"));
            else if(RegionsStringInt.TryGetValue(region.Name, out _))
                return Task.FromException(new Exception($"Region with name={region.Name} already exists"));
            else
            {
                RegionsIntString.TryAdd(region.Id, region.Name);
                RegionsStringInt.TryAdd(region.Name, region.Id);
                return Task.CompletedTask;
            }
        }
        public Task<string> GetNameByIdRegionAsync(int id, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            if (RegionsIntString.TryGetValue(id, out var res))
                return Task.FromResult(res);
            else
                throw new NotFoundException($"Region with name={id} is not found");
        }
        public Task<int> GetIdByRegionNameAsync(string regionName, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            if (RegionsStringInt.TryGetValue(regionName, out var res))
                return Task.FromResult(res);
            else
                throw new NotFoundException($"Region with name={regionName} is not found");
        }
        public Task<string[]> GetRegionsAsync(CancellationToken token = default)
        {
            return Task.FromResult(RegionsIntString.Values.ToArray());
        }
        public Task<bool> IsRegionInRepository(string regionName, CancellationToken cancellationToken = default)
        {
            if (RegionsStringInt.TryGetValue(regionName, out _))
                return Task.FromResult(true);
            else
                return Task.FromResult(false);
        }

        public Task<bool> IsRegionInRepository(string[] regionName, CancellationToken cancellationToken = default)
        {
            foreach(var r in regionName)
                if (!RegionsStringInt.TryGetValue(r, out _))
                    return Task.FromResult(false);
            return Task.FromResult(true);
        }
    }
}
