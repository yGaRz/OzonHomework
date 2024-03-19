using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
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
            return Task.FromResult(RegionsIntString[id]);
        }
        public Task<int> GetIdByRegionNameAsync(string regionName, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return Task.FromResult(RegionsStringInt[regionName]);
        }
        public Task<string[]> GetRegionsAsync(CancellationToken token = default)
        {
            return Task.FromResult(RegionsIntString.Values.ToArray());
        }
    }
}
