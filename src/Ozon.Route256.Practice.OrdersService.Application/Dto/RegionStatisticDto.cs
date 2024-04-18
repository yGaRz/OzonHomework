namespace Ozon.Route256.Practice.OrdersService.Application.Dto;

public record RegionStatisticDto(string RegionName, 
        int TotalCountOrders, 
        double TotalSumOrders, 
        double TotalWigthOrders, 
        int TotalCustomers);

