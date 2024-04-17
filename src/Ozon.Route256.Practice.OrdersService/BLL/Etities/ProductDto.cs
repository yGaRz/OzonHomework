namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

public record ProductDto(
    long Id,
    string Name,
    int Quantity,
    double Price,
    double Weight
);
