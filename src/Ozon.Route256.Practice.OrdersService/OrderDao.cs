using Ozon.Route256.Practice.CustomerGprcFile;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models.Enums;
using System.Text.Json;
using System.Text.Json.Nodes;
namespace Ozon.Route256.Practice.OrdersService.Application.Dto;

public record OrderDao
{
    public long Id { get; init; }
    public int CustomerId { get; set; }
    public OrderSourceEnum Source { get; init; }
    public AddressDto Address { get; init; } = null;
    public OrderStateEnum State { get; set; }
    public DateTime TimeCreate { get; set; }
    public DateTime TimeUpdate { get; set; }
    public string Region { get; set; }
    public int CountGoods { get; init; }
    public double TotalPrice { get; init; }
    public double TotalWeigth { get; init; }
}


