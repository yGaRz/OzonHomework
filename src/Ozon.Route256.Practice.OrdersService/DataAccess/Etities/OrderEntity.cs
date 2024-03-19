namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities
{
    public record OrderEntity(
        ulong Id,
        uint CountGoods,
        double TotalSum,
        double TotalWigth,
        OrderSource Source,
        DateTime DateOrder,
        RegionEntity Region,
        OrderState State,
        string CustomerName,
        Address AddressCustomer,
        string PhoneNumber
    );
}
