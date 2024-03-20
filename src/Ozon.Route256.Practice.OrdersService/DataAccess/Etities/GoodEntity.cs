namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

public record GoodEntity(
    long Id,
    string Name,
    int Quantity,
    decimal Price,
    uint Weight
);
