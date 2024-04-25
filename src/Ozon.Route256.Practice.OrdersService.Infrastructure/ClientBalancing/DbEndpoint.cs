namespace Ozon.Route256.Practice.OrdersService.Infrastructure.ClientBalancing;

public sealed record DbEndpoint(
    string HostAndPort, 
    DbReplicaType DbReplica,
    int[] Buckets);