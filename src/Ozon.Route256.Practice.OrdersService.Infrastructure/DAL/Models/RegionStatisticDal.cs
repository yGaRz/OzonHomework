namespace Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Models
{
    public record RegionStatisticDal(int regionId,
        int TotalCountOrders,
        double TotalSumOrders,
        double TotalWigthOrders,
        int TotalCustomers);
}
