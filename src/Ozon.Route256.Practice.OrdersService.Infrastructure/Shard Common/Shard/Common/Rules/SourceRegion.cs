using Ozon.Route256.Practice.OrdersService.Infrastructure.Models.Enums;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard.Common.Rules;

public record SourceRegion(int region_id, OrderSourceEnum source);
