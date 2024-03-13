namespace Ozon.Route256.Practice.CustomerService.ClientBalancing;

public sealed record DbEndpoint(
    string HostAndPort, 
    DbReplicaType DbReplica,
    int[] Buckets);