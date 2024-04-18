namespace Ozon.Route256.Practice.OrdersService.Application.Dto;
public record ProductDto(
    int Id,
    string Name,
    int Quantity,
    double Price,
    double Weight
);
