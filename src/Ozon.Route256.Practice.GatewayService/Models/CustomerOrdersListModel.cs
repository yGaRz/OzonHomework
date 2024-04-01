using Ozon.Route256.Practice.GatewayService.Etities;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class CustomerOrdersListModel
{
    public uint PageIndex {  get; init; }
    public string CustomerName { get; init; } = "";
    public AddressEntity address { get; init; } = null;
    public string Phone { get; init; } = "";
    public string Region { get; init; } = "";
    public List<Order> ListOrder { get; init; } = new List<Order>();
}
