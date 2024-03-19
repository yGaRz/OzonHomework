using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.DataAccess.DTO
{
    public record Order(
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
