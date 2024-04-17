namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

public record RegionStatisticDto(string RegionName, 
        int TotalCountOrders, 
        double TotalSumOrders, 
        double TotalWigthOrders, 
        int TotalCustomers);

