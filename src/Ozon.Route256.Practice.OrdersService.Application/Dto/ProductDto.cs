namespace Ozon.Route256.Practice.OrdersService.Application.Dto;

public record ProductDto(
    long Id,
    string Name,
    int Quantity,
    double Price,
    double Weight
);
