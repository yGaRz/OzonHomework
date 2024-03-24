using Ozon.Route256.Practice.GatewayService.Etities;

namespace Ozon.Route256.Practice.GatewayService.Models;

//uint32 page_number = 1;
//string name_customer = 2;
//Address address_customer = 3;
//string phone_number = 4;
//string region = 5;
//repeated Order orders=6;

public class CustomerOrdersListModel
{
    public uint PageIndex {  get; init; }
    public string CustomerName { get; init; } = "";
    public AddressEntity address { get; init; }
    public string Phone { get; init; } = "";
    public string Region { get; init; } = "";
    public List<Order> ListOrder { get; init; } = new List<Order>();
}
