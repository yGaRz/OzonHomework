namespace Ozon.Route256.Practice.CustomerService.ClientBalancing;

public interface IDbStore
{
    Task UpdateEndpointAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints);

    DbEndpoint GetEndpointByBucket(int bucketId);

    int BucketsCount { get; }
}

public class DbStore : IDbStore
{
    private const int START_INDEX = 0;

    private DbEndpoint[] _endpoints = Array.Empty<DbEndpoint>();


    public Task UpdateEndpointAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints)
    {
        _endpoints = new DbEndpoint[dbEndpoints.Count];
        var bucketsCount = 0;

        foreach (var (dbEndpoint, index) in dbEndpoints.Zip(Enumerable.Range(START_INDEX, dbEndpoints.Count)))
        {
            _endpoints[index] = dbEndpoint;
            bucketsCount += dbEndpoint.Buckets.Length;
        }

        if (BucketsCount != 0 && BucketsCount != bucketsCount)
            throw new InvalidDataException("Bucket count changed");

        BucketsCount = bucketsCount;

        return Task.CompletedTask;
    }

    public DbEndpoint GetEndpointByBucket(
        int bucketId)
    {
        var result = _endpoints.FirstOrDefault(x => x.Buckets.Contains(bucketId));


        return result ?? throw new ArgumentOutOfRangeException($"There is not bucket {bucketId}");
    }

    public int BucketsCount { get; private set; }
}