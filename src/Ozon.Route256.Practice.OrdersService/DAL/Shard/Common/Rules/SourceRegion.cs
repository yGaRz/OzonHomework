using Ozon.Route256.Practice.OrdersService.Models;

namespace Ozon.Route256.Practice.OrdersService.DAL.Shard.Common.Rules;

public record SourceRegion(int region_id, OrderSourceEnum source);
