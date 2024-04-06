namespace Ozon.Route256.Practice.OrdersService.DAL.Models
{
    public record RegionStatisticDal(int regionId,
        int TotalCountOrders,
        double TotalSumOrders,
        double TotalWigthOrders,
        int TotalCustomers);
}
