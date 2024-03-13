namespace Ozon.Route256.Practice.CustomerService.ClientBalancing;

public interface IDbStore
{
    Task UpdateEndpointAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints);
}

public class DbStore : IDbStore
{
    private const int START_INDEX = 0;

    private DbEndpoint[] _endpoints = Array.Empty<DbEndpoint>();
    
    public Task UpdateEndpointAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints)
    {
        _endpoints = new DbEndpoint[dbEndpoints.Count];

        foreach (var (dbEndpoint, index) in dbEndpoints.Zip(Enumerable.Range(START_INDEX, dbEndpoints.Count)))
        {
            _endpoints[index] = dbEndpoint;
        }
        
        return Task.CompletedTask;
    }
}