namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

public record RegionStatisticEntity(string RegionName, 
        uint TotalCountOrders, 
        double TotalSumOrders, 
        double TotalWigthOrders, 
        uint TotalCustomers);

