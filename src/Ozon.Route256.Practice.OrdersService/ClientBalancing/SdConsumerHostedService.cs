using Grpc.Core;
using GrpcReplicaType = Ozon.Route256.Practice.Replica.Types.ReplicaType;

namespace Ozon.Route256.Practice.CustomerService.ClientBalancing;

public class SdConsumerHostedService : BackgroundService
{
    private const int SD_TIME_TO_DELAY_MS = 1000;
    private readonly SdService.SdServiceClient _client;
    private readonly ILogger<SdConsumerHostedService> _logger;
    private readonly IDbStore _dbStore;
    public SdConsumerHostedService(
        SdService.SdServiceClient client,
        ILogger<SdConsumerHostedService> logger,
        IDbStore dbStore)
    {
        _client = client;
        _logger = logger;
        _dbStore = dbStore;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var request = new DbResourcesRequest
                {
                    ClusterName = "cluster"
                };

                using var stream = 
                    _client.DbResources(request, cancellationToken: stoppingToken);

                await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
                {                   

                    _logger.LogInformation(
                        "Get a new data from SD. Timestamp {Timestamp}",
                        response.LastUpdated.ToDateTime());
                    var endpoints = GetEndpoints(response).ToList();
                    await _dbStore.UpdateEndpointAsync(endpoints);
                }
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "SD throw exception.");
                await Task.Delay(SD_TIME_TO_DELAY_MS, stoppingToken);
            }
        }
    }

    private static IEnumerable<DbEndpoint> GetEndpoints(DbResourcesResponse response) =>
        response.Replicas.Select(replica => new DbEndpoint(
            $"{replica.Host}:{replica.Port}",
            ToDbReplica(replica.Type),
            replica.Buckets.ToArray()));

    private static DbReplicaType ToDbReplica(GrpcReplicaType replicaType) =>
        replicaType switch
        {
            GrpcReplicaType.Master => DbReplicaType.Master,
            GrpcReplicaType.Async => DbReplicaType.Async,
            GrpcReplicaType.Sync => DbReplicaType.Sync,
            _ => throw new ArgumentOutOfRangeException(nameof(replicaType), replicaType, null)
        };
}