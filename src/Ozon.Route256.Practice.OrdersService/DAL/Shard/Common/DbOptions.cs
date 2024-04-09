namespace Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;

public class DbOptions
{
    public string ClusterName { get; set; } = default!;
    public string DatabaseName { get; set; } = default!;
    public string User { get; set; } = default!;
    public string Password { get; set; } = default!;
}