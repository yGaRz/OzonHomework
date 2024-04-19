namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Models;

internal record RegionStatisticDal(int regionId,
    int TotalCountOrders,
    double TotalSumOrders,
    double TotalWigthOrders,
    int TotalCustomers);
