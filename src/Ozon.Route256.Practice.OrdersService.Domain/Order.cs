using Ozon.Route256.Practice.OrdersService.Domain.Core;
using Ozon.Route256.Practice.OrdersService.Domain.Enums;

namespace Ozon.Route256.Practice.OrdersService.Domain;
public sealed class Order : Entity<long>
{
    private Order(long id, int customerId, OrderSourceEnumDomain source,
        Address addressOrder, OrderStateEnumDomain state, DateTime timeCreate,
        DateTime timeUpdate, string region, int countGoods,
        double totalPrice, double totalWeigth) : base(id)
    {
        CustomerId = customerId;
        Source = source;
        AddressOrder = addressOrder;
        State = state;
        TimeCreate = timeCreate;
        TimeUpdate = timeUpdate;
        Region = region;
        CountGoods = countGoods;
        TotalPrice = totalPrice;
        TotalWeigth = totalWeigth;
    }

    public static Order CreateInstace(long id, int customerId, OrderSourceEnumDomain source,
        Address addressOrder, OrderStateEnumDomain state, DateTime timeCreate,
        DateTime timeUpdate, string region, int countGoods,
        double totalPrice, double totalWeigth)
    {
        return new Order(id, customerId, source, addressOrder, state, timeCreate, timeUpdate, region, countGoods, totalPrice, totalWeigth);
    }


    public int CustomerId { get; }
    public OrderSourceEnumDomain Source { get; }
    public Address AddressOrder { get; }
    public OrderStateEnumDomain State { get; }
    public DateTime TimeCreate { get; }
    public DateTime TimeUpdate { get; }
    public string Region { get; }
    public int CountGoods { get; }
    public double TotalPrice { get; }
    public double TotalWeigth { get; }

}
